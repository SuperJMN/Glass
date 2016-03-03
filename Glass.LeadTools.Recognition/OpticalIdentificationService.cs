namespace SIC.Services.OCR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Core;
    using Data.SIC.Models;
    using Leadtools;
    using Leadtools.Barcode;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;
    using Logging;
    using PostProcessing;

    public class OpticalIdentificationService : IOpticalIdentificationService
    {
        private const string OcrEngineFolder = @"OcrAdvantageRuntime";
        private readonly BarcodeEngine barcodeEngine = new BarcodeEngine();

        private readonly BarcodeSymbology[] barcodesTypes =
        {
            BarcodeSymbology.Code3Of9, BarcodeSymbology.Code93, BarcodeSymbology.QR,
            BarcodeSymbology.Datamatrix
        };

        private readonly RasterCodecs codecs = new RasterCodecs();
        private readonly OcrEngine engine;
        private readonly ILoggingService loggingService;
        private readonly IOcrPostProcessor postProcessor;

        public OpticalIdentificationService(IOcrPostProcessor postProcessor, ILoggingService loggingService)
        {
            this.postProcessor = postProcessor;
            this.loggingService = loggingService;
            engine = (OcrEngine) OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
            engine.Startup(codecs, null, null, OcrEngineFolder);
            barcodeEngine.Reader.ImageType = BarcodeImageType.ScannedDocument;
        }

        public IEnumerable<RecognitionResult> PerformOcr(BitmapSource image, IEnumerable<FieldConfiguration> fieldsFromConfig)
        {
            var recognitionResults = PerformOcrAgainstFields(image, fieldsFromConfig);

            foreach (var result in recognitionResults)
            {
                var finalText = ApplyBusinessRestrictions(result.RecognizedText, result.FieldConfiguration);
                var finalTextForLoggin = finalText.Replace(Environment.NewLine, " ");

                loggingService.AddShortLog(
                    string.Format(
                        "\t[{2}]: Texto reconocido por OCR=\"{0}\", Text Procesado=\"{1}\"",
                        finalTextForLoggin,
                        finalText,
                        result.FieldConfiguration.Name));

                yield return new RecognitionResult(result.FieldConfiguration, finalText, result.Image);
            }
        }

        public IEnumerable<RecognitionResult> IdentifyBarcodes(BitmapSource image, IEnumerable<FieldConfiguration> barcodeFields)
        {
            return from bfc in barcodeFields
                let rect = bfc.GetBounds()
                let text = GetStringFromBarcode(image, rect)
                let bitmapSource = GenerateFrozenCrop(image, rect)
                select new RecognitionResult(bfc, ApplyBusinessRestrictions(text, bfc), bitmapSource);
        }

        private string ApplyBusinessRestrictions(string originalText, FieldConfiguration fieldConfiguration)
        {
            var processedText = postProcessor.Process(originalText, fieldConfiguration);

            if (originalText == null)
            {
                return null;
            }

            var finalText = processedText
                .RemoveDiacritics()
                .ToUpper()
                .Truncate(fieldConfiguration.MaxLength);

            return finalText;
        }

        private IEnumerable<RecognitionResult> PerformOcrAgainstFields(BitmapSource image, IEnumerable<FieldConfiguration> fields)
        {
            var ocrPage = CreateOcrPage(image, fields);

            ocrPage.Recognize(null);

            return GetRecognitionResults(image, fields, ocrPage);
        }

        private static IEnumerable<RecognitionResult> GetRecognitionResults(BitmapSource image, IEnumerable<FieldConfiguration> fields, IOcrPage ocrPage)
        {
            foreach (var zone in ocrPage.Zones.Where(zone => !zone.IsEngineZone))
            {
                var associatedField = fields.Single(f => string.Equals(f.Id.Value.ToString(), zone.Name));

                var ocrResult = ocrPage.GetText(zone.Id).TrimEnd();

                var size = new Size(zone.Bounds.Width, zone.Bounds.Height);
                var location = new Point(zone.Bounds.X, zone.Bounds.Y);
                var rect = new Rect(location, size);
                var bitmap = GenerateFrozenCrop(image, rect);

                yield return new RecognitionResult(associatedField, ocrResult, bitmap);
            }
        }

        public static BitmapSource GenerateFrozenCrop(BitmapSource bmp, Rect rect)
        {
            var crop = bmp.Crop(rect.ConverToRawPixels(bmp.DpiX, bmp.DpiY));
            crop.Freeze();
            return crop;
        }

        private string GetStringFromBarcode(ImageSource image, Rect rect)
        {
            var leadRect = new LogicalRectangle(rect.X, rect.Y, rect.Width, rect.Height, LogicalUnit.Pixel);

            var strategies = new List<IStrategy> { new NoProcessStrategy(), new AutoColorStrategy(), };
            var result = (from strategy in strategies
                select strategy.Apply(image)
                into processed
                select barcodeEngine.Reader.ReadBarcode(processed.ToRasterImage(), leadRect, barcodesTypes)
                into barcodeData
                where barcodeData != null
                select barcodeData.Value).FirstOrDefault();
            
            return result;
        }

        private IOcrPage CreateOcrPage(ImageSource image, IEnumerable<FieldConfiguration> fields)
        {
            var optimizeImageForOcr = image.ToRasterImage().OptimizeImageForOcr();

            var ocrPage = engine.CreatePage(optimizeImageForOcr, OcrImageSharingMode.None);
            var fieldsToRecognize = fields.Where(f => !f.IsCadaCode && string.IsNullOrEmpty(f.FixedValue));

            foreach (var field in fieldsToRecognize)
            {
                var bounds = field.GetBounds();
                var cropRect = bounds.ConverToRawPixels(optimizeImageForOcr.XResolution, optimizeImageForOcr.YResolution);
                var ocrZone = CreateOcrZoneForField(cropRect, field);

                ocrPage.Zones.Add(ocrZone);
            }

            return ocrPage;
        }

        private static OcrZone CreateOcrZoneForField(Rect rectCrop, FieldConfiguration fieldConfiguration)
        {
            var readZone = new OcrZone
            {
                Name = fieldConfiguration.Id.Value.ToString(),
                Bounds = new LogicalRectangle(rectCrop.ToLeadRectRect()),
                CharacterFilters = GetCharacterFilters(fieldConfiguration.FieldType),
                Language = GetLanguage(fieldConfiguration.FieldType),
                ZoneType = GetZoneType(fieldConfiguration.FieldType),
                IsEngineZone = false,
                ForeColor = new RasterColor(0, 0, 0),
                BackColor = new RasterColor(255, 255, 255),
                TextStyle = OcrTextStyle.Heading,                
            };

            return readZone;
        }

        private static string GetLanguage(FieldType fieldType)
        {
            if (fieldType == FieldType.Alphabetic || fieldType == FieldType.Alphanumeric)
            {
                return "ES";
            }

            return null;
        }

        private static OcrZoneCharacterFilters GetCharacterFilters(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Numeric:
                    return OcrZoneCharacterFilters.Digit;
                case FieldType.Alphabetic:
                    return OcrZoneCharacterFilters.Uppercase | OcrZoneCharacterFilters.Lowercase;
                case FieldType.Alphanumeric:
                    return OcrZoneCharacterFilters.All;
                default:
                    return OcrZoneCharacterFilters.All;
            }
        }

        private static OcrZoneType GetZoneType(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Numeric:
                case FieldType.Alphabetic:
                case FieldType.Alphanumeric:

                    return OcrZoneType.Text;

                case FieldType.Barcode:

                    return OcrZoneType.Barcode;

                default:
                    throw new ArgumentOutOfRangeException("fieldType", fieldType, null);
            }
        }
    }
}