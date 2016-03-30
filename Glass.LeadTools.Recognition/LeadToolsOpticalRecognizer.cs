namespace Glass.LeadTools.Recognition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Imaging.Core;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using ImagingExtensions;
    using Leadtools.Barcode;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;
    using MessagingToolkit.Barcode;

    public class LeadToolsOpticalRecognizer : IZoneBasedRecognitionService
    {
        private const string OcrEngineFolder = @"OcrAdvantageRuntime";

        private readonly RasterCodecs codecs = new RasterCodecs();
        private readonly BarcodeDecoder alternateBarcodeDecoder = new BarcodeDecoder();

        public RecognizeOptions Options { get; set; } = new RecognizeOptions();

        public QualityOptions QualityOptions { get; set; } = new QualityOptions();

        public LeadToolsOpticalRecognizer(ILeadToolsLicenseApplier licenseApplier)
        {
            licenseApplier.ApplyLicense();
            OcrEngine = (OcrEngine)OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
            OcrEngine.Startup(codecs, null, null, OcrEngineFolder);
        }

        private BarcodeEngine BarcodeEngine { get; } = new BarcodeEngine
        {
            Reader = { ImageType = BarcodeImageType.Picture, },
        };

        private OcrEngine OcrEngine { get; }

        public RecognizedPage Recognize(BitmapSource image, RecognitionConfiguration configuration)
        {
            var barcodeRecognitions = RecognizeBarcodes(image, configuration);
            var ocrRecognitions = PerformOcr(image, configuration);
            var recognizedZones = barcodeRecognitions.Concat(ocrRecognitions).ToList();

            PostProcess(recognizedZones);

            return new RecognizedPage(image, recognizedZones);
        }

        private static void PostProcess(IEnumerable<RecognizedZone> recognizedZones)
        {
            foreach (var recognizedZone in recognizedZones)
            {
                var processed = recognizedZone.ZoneConfig.TextualDataFilter.Filter(recognizedZone.RecognizedText);
                recognizedZone.RecognizedText = processed;
            }
        }

        private IEnumerable<RecognizedZone> PerformOcr(BitmapSource image, RecognitionConfiguration configuration)
        {
            var ocrZones = configuration.Zones.Where(z => z.Symbology == Symbology.Text).ToList();

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

        private IOcrPage CreateOcrPage(BitmapSource image, IEnumerable<ZoneConfiguration> configs)
        {
            var source = GetImageToOcr(image);
            var zones = GetZones(image.DpiX, image.DpiY, configs).ToList();

            var ocrPage = OcrEngine.CreatePage(source.ToRasterImage(), OcrImageSharingMode.None);

            foreach (var ocrZone in zones)
            {
                ocrPage.Zones.Add(ocrZone);
            }

            return ocrPage;
        }

        private ImageSource GetImageToOcr(BitmapSource image)
        {
            var scale = Options.SourceScaleForOcr;
            var isScalingEnabled = Options.IsSourceScalingEnabledForOcr;

            var finalBitmap = isScalingEnabled ? new TransformedBitmap(image, new ScaleTransform(scale, scale)) : image;

            using (var raster = finalBitmap.ToRasterImage())
            {
                return raster.OptimizeImageForOcr().ToBitmapSource();
            }
        }

        private IEnumerable<OcrZone> GetZones(double dpiX, double dpiY, IEnumerable<ZoneConfiguration> zones)
        {
            foreach (var field in zones)
            {
                var bounds = field.Bounds;

                var cropRect = bounds;
                var scaledZone = cropRect.ConvertFrom96pppToBitmapDpi(dpiX, dpiY);
                var scale = Options.SourceScaleForOcr;
                scaledZone.Scale(scale, scale);

                var ocrZone = CreateOcrZoneForField(scaledZone, field);

                yield return ocrZone;
            }
        }

        private static OcrZone CreateOcrZoneForField(Rect rectCrop, ZoneConfiguration zoneConfiguration)
        {
            var zoneConfig = zoneConfiguration;

            var readZone = new OcrZone
            {
                Name = zoneConfiguration.Id,
                Bounds = new LogicalRectangle(rectCrop.ToLeadRectRect()),
                CharacterFilters = GetCharacterFilters(zoneConfig),
                Language = GetLanguage(zoneConfig),
                ZoneType = GetZoneType(zoneConfig),
                IsEngineZone = false
            };

            return readZone;
        }

        private static string GetLanguage(ZoneConfiguration zoneConfiguration)
        {
            var filterType = zoneConfiguration.TextualDataFilter.FilterType;
            if (filterType == FilterType.Alpha || filterType == FilterType.AlphaOnly)
            {
                return "ES";
            }

            return null;
        }

        private static OcrZoneCharacterFilters GetCharacterFilters(ZoneConfiguration filterType)
        {
            switch (filterType.TextualDataFilter.FilterType)
            {
                case FilterType.Digits:
                    return OcrZoneCharacterFilters.Digit;
                case FilterType.AlphaOnly:
                    return OcrZoneCharacterFilters.Uppercase | OcrZoneCharacterFilters.Lowercase;
                case FilterType.Alpha:
                    return OcrZoneCharacterFilters.All;
                case FilterType.Number:
                    return OcrZoneCharacterFilters.Numbers | OcrZoneCharacterFilters.Punctuation;
                default:
                    return OcrZoneCharacterFilters.All;
            }
        }

        private static OcrZoneType GetZoneType(ZoneConfiguration fieldType)
        {
            switch (fieldType.Symbology)
            {
                case Symbology.Text:

                    return OcrZoneType.Text;

                case Symbology.Barcode:

                    return OcrZoneType.Barcode;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, "El tipo de zona de OCR no se puede asociar a un tipo de OcrZoneType");
            }
        }

        private IEnumerable<RecognizedZone> RecognizeBarcodes(BitmapSource image, RecognitionConfiguration configuration)
        {
            var barcodeZones = configuration.Zones.Where(z => z.Symbology == Symbology.Barcode);
            return from barcodeConfig in barcodeZones
                   let rect = barcodeConfig.Bounds
                   let barcode = ImagingContext.BitmapOperations.Crop(image, rect)
                   let text = GetStringFromBarcode(barcode, barcodeConfig)
                   select new RecognizedZone(image, barcodeConfig, text);
        }

        private string GetStringFromBarcode(BitmapSource image, ZoneConfiguration barcodeConfig)
        {
            var evaluator = barcodeConfig.TextualDataFilter.Evaluator;

            var leadReaderScores = GetScoresUsingLeadToolsDecoder(image, evaluator);
            var alternateScores = GetScoresUsingAlternateDecoder(image, evaluator);

            var results = leadReaderScores.Concat(alternateScores);

            var ordered = results.OrderByDescending(arg => arg.Score);
            var top = ordered.FirstOrDefault();

            return top?.Text;
        }

        private IEnumerable<ScoreResult> GetScoresUsingAlternateDecoder(BitmapSource image, IEvaluator evaluator)
        {
            var writeableBitmap = new WriteableBitmap(image);
            string text;
            try
            {
                var result = alternateBarcodeDecoder.Decode(writeableBitmap, new Dictionary<DecodeOptions, object>());
                text = result.Text;                
            }
            catch
            {
                text = null;
            }

            if (text == null)
            {
                yield break;
            }

            yield return new ScoreResult { Text = text, Score = evaluator.GetScore(text) };
        }

        private List<ScoreResult> GetScoresUsingLeadToolsDecoder(BitmapSource image, IEvaluator evaluator)
        {
            var coreReadOptions = QualityOptions.CoreReadOptions;

            var leadRect = new LogicalRectangle(0, 0, image.PixelWidth, image.PixelHeight, LogicalUnit.Pixel);

            var scores = new List<ScoreResult>();
            foreach (var strategy in QualityOptions.BarcodeStrategies)
            {
                BarcodeEngine.Reader.ImageType = strategy.ImageType;
                var transformedImage = strategy.ImageFilter.Apply(image);
                var barcodeDatas = BarcodeEngine.Reader.ReadBarcodes(
                    transformedImage.ToRasterImage(),
                    leadRect,
                    10,
                    Options.BarcodeSymbologies.ToArray(),
                    coreReadOptions);

                var results = barcodeDatas.Where(data => data.Value != null).Select(data => data.Value);

                foreach (var result in results)
                {
                    var barcodeText = result;
                    var score = evaluator.GetScore(barcodeText);
                    scores.Add(new ScoreResult { Score = score, Text = barcodeText, ImageFilter = strategy.ImageFilter, ImageType = strategy.ImageType });
                }
            }
            return scores;
        }
    }


}