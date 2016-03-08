namespace Glass.LeadTools.Recognition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging.Core;
    using Imaging.PostProcessing;
    using ImagingExtensions;
    using Leadtools;
    using Leadtools.Barcode;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;

    public class LeadToolsOpticalRecognizer : IZoneBasedRecognitionService
    {
        private readonly IOcrPostProcessor postProcessor;
        private readonly ISingleLinePolicy barcodeScorePolicy;
        private readonly RasterCodecs codecs = new RasterCodecs();
        private const string OcrEngineFolder = @"OcrAdvantageRuntime";

        public LeadToolsOpticalRecognizer(ILeadToolsLicenseApplier licenseApplier, IOcrPostProcessor postProcessor, ISingleLinePolicy barcodeScorePolicy)
        {
            this.postProcessor = postProcessor;
            this.barcodeScorePolicy = barcodeScorePolicy;
            licenseApplier.ApplyLicense();
            OcrEngine = (OcrEngine)OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
            OcrEngine.Startup(codecs, null, null, OcrEngineFolder);
        }

        private BarcodeEngine BarcodeEngine { get; } = new BarcodeEngine();

        private BarcodeSymbology[] BarcodesTypes { get; } =
            {
                BarcodeSymbology.Code3Of9, BarcodeSymbology.Code93, BarcodeSymbology.QR,
                BarcodeSymbology.Datamatrix
            };

        public OcrEngine OcrEngine { get; set; }

        public RecognizedPage Recognize(BitmapSource image, RecognitionConfiguration configuration)
        {
            var barcodeRecognitions = RecognizeBarcodes(image, configuration);
            var ocrRecognitions = PerformOcr(image, configuration);

            var recognizedZones = barcodeRecognitions.Concat(ocrRecognitions).ToList();

            PostProcess(recognizedZones);

            return new RecognizedPage(image, recognizedZones);
        }

        private void PostProcess(IEnumerable<RecognizedZone> recognizedZones)
        {
            foreach (var recognizedZone in recognizedZones)
            {
                var processed = postProcessor.Process(recognizedZone.RecognizedText, recognizedZone.ZoneConfig);
                recognizedZone.RecognizedText = processed;
            }
        }

        private IEnumerable<RecognizedZone> PerformOcr(BitmapSource image, RecognitionConfiguration configuration)
        {
            var ocrZones = configuration.Zones.Where(z => z.SmartZoneType != SmartZoneType.Barcode);

            if (!ocrZones.Any())
            {
                return new List<RecognizedZone>();
            }

            var ocrPage = CreateOcrPage(image, ocrZones);
            ocrPage.Recognize(null);
            return GetRecognitionResults(image, ocrZones, ocrPage);
        }

        private static IEnumerable<RecognizedZone> GetRecognitionResults(BitmapSource image, IEnumerable<ZoneConfiguration> ocrZones, IOcrPage ocrPage)
        {
            return from zone in ocrPage.Zones.Where(zone => !zone.IsEngineZone)
                   let configuration = ocrZones.Single(f => string.Equals(f.Id, zone.Name))
                   let text = ocrPage.GetText(zone.Id)
                   select new RecognizedZone(image, configuration, text);
        }

        private IOcrPage CreateOcrPage(BitmapSource image, IEnumerable<ZoneConfiguration> zones)
        {
            var optimizeImageForOcr = image.ToRasterImage().OptimizeImageForOcr();

            var ocrPage = OcrEngine.CreatePage(optimizeImageForOcr, OcrImageSharingMode.None);

            foreach (var field in zones)
            {
                var bounds = field.Bounds;
                var cropRect = bounds.ConverToRawPixels(optimizeImageForOcr.XResolution, optimizeImageForOcr.YResolution);
                var ocrZone = CreateOcrZoneForField(cropRect, field);

                ocrPage.Zones.Add(ocrZone);
            }

            return ocrPage;
        }

        private static OcrZone CreateOcrZoneForField(Rect rectCrop, ZoneConfiguration zoneConfiguration)
        {
            var zt = zoneConfiguration.SmartZoneType;

            var readZone = new OcrZone
            {
                Name = zoneConfiguration.Id,
                Bounds = new LogicalRectangle(rectCrop.ToLeadRectRect()),
                CharacterFilters = GetCharacterFilters(zt),
                Language = GetLanguage(zt),
                ZoneType = GetZoneType(zt),
                IsEngineZone = false,
                ForeColor = new RasterColor(0, 0, 0),
                BackColor = new RasterColor(255, 255, 255),
                TextStyle = OcrTextStyle.Heading,
            };

            return readZone;
        }

        private static string GetLanguage(SmartZoneType fieldType)
        {
            //if (fieldType == SmartZoneType.Alpha || fieldType == SmartZoneType.AlphaOnly)
            //{
            //    return "ES";
            //}

            return null;
        }

        private static OcrZoneCharacterFilters GetCharacterFilters(SmartZoneType fieldType)
        {
            switch (fieldType)
            {
                case SmartZoneType.Digits:
                    return OcrZoneCharacterFilters.Digit;
                case SmartZoneType.AlphaOnly:
                    return OcrZoneCharacterFilters.Uppercase | OcrZoneCharacterFilters.Lowercase;
                case SmartZoneType.Alpha:
                    return OcrZoneCharacterFilters.All;
                case SmartZoneType.Number:
                    return OcrZoneCharacterFilters.Numbers | OcrZoneCharacterFilters.Punctuation;
                default:
                    return OcrZoneCharacterFilters.All;
            }
        }

        private static OcrZoneType GetZoneType(SmartZoneType fieldType)
        {
            switch (fieldType)
            {
                case SmartZoneType.Digits:
                case SmartZoneType.Alpha:
                case SmartZoneType.AlphaOnly:
                case SmartZoneType.Number:

                    return OcrZoneType.Text;

                case SmartZoneType.Barcode:

                    return OcrZoneType.Barcode;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, "El tipo de zona de OCR no se puede asociar a un tipo de OcrZoneType");
            }
        }

        private IEnumerable<RecognizedZone> RecognizeBarcodes(BitmapSource image, RecognitionConfiguration configuration)
        {
            var barcodeZones = configuration.Zones.Where(z => z.SmartZoneType == SmartZoneType.Barcode);
            return from barcodeConfig in barcodeZones
                   let rect = barcodeConfig.Bounds
                   let text = GetStringFromBarcode(image, rect, barcodeConfig)
                   select new RecognizedZone(image, barcodeConfig, text);
        }

        private string GetStringFromBarcode(ImageSource image, Rect rect, ZoneConfiguration barcodeConfig)
        {
            var leadRect = new LogicalRectangle(rect.X, rect.Y, rect.Width, rect.Height, LogicalUnit.Pixel);

            var strategies = new List<IStrategy> { new NoProcessStrategy(), new AutoColorStrategy() };

            return GetBestBarcodeMatch(image, barcodeConfig, strategies, leadRect);
        }

        private string GetBestBarcodeMatch(ImageSource image, ZoneConfiguration barcodeConfig, IEnumerable<IStrategy> strategies, LogicalRectangle leadRect)
        {
            var query = from str in strategies
                        let img = str.Apply(image)
                        let barcodeData = BarcodeEngine.Reader.ReadBarcode(img.ToRasterImage(), leadRect, BarcodesTypes)
                        let barcodeText = barcodeData?.Value
                        let score = barcodeScorePolicy.GetScore(barcodeText, barcodeConfig)
                        select new { Score = score, Text = barcodeText };

            var top = query.OrderBy(arg => arg.Score).First();
            return top.Text;
        }
    }


}