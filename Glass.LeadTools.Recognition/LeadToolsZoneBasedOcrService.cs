namespace Glass.LeadTools.Recognition
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Imaging.Core;
    using Imaging.Generators;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using ImagingExtensions;
    using ImagingExtensions.ImageFilters;
    using Leadtools;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;

    public class LeadToolsZoneBasedOcrService : IImageToTextConverter
    {
        private const string OcrEngineFolder = @"OcrAdvantageRuntime";

        private OcrEngine engine;

        public LeadToolsZoneBasedOcrService(ILeadToolsLicenseApplier licenseApplier)
        {
            licenseApplier.ApplyLicense();
        }

        private IEnumerable<IBitmapBatchGenerator> BitmapBatchGenerator { get; } = new List<IBitmapBatchGenerator>
        {
            new AutoShitGenerator(),
        };

        public double SourceScaleForOcr { get; set; } = 0.3;
        public bool IsSourceScalingEnabledForOcr { get; set; } = true;

        private OcrEngine OcrEngine
        {
            get
            {
                if (engine == null)
                {
                    engine = (OcrEngine)OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
                    engine.Startup(new RasterCodecs(), null, null, OcrEngineFolder);
                }

                return engine;
            }
        }

        public IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config)
        {
            var bitmaps = BitmapBatchGenerator.SelectMany(g => g.Generate(bitmap));
            var scaled = bitmaps.Select(ScaleIfEnabled);
            var recognitions = scaled.SelectMany(bmp => RecognizeCore(config, bmp));
            return recognitions;
        }

        private BitmapSource ScaleIfEnabled(BitmapSource bmp)
        {
            var scale = SourceScaleForOcr;
            var isScalingEnabled = IsSourceScalingEnabledForOcr;

            return isScalingEnabled ? new TransformedBitmap(bmp, new ScaleTransform(scale, scale)) : bmp;
        }

        private IEnumerable<RecognitionResult> RecognizeCore(ZoneConfiguration config, ImageSource bmp)
        {
            using (var page = OcrEngine.CreatePage(bmp.ToRasterImage(), OcrImageSharingMode.AutoDispose))
            {
                var ocrZone = CreateOcrZoneForField(config);
                page.Zones.Add(ocrZone);
                page.Recognize(null);
                var text = page.GetText(0);

                var confidence = GetConfidence(page);

                var filteredText = config.TextualDataFilter.Filter(text);
                yield return new RecognitionResult(filteredText, confidence);
            }
        }

        private static double GetConfidence(IOcrPage page)
        {
            var recognizedCharacters = page.GetRecognizedCharacters();
            var findZoneCharacters = recognizedCharacters.FindZoneCharacters(0);
            return findZoneCharacters
                .DefaultIfEmpty()
                .Average(character => character.Confidence) / 100D;
        }

        public IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Text, FilterTypes = FilterType.All } };

        private OcrZone CreateOcrZoneForField(ZoneConfiguration zoneConfiguration)
        {
            var bounds = zoneConfiguration.Bounds;
            bounds.Scale(SourceScaleForOcr, SourceScaleForOcr);
            var leadRectRect = bounds.ToLeadRectRect();

            var readZone = new OcrZone
            {
                Bounds = new LogicalRectangle(leadRectRect),
                Name = zoneConfiguration.Id,
                CharacterFilters = GetCharacterFilters(zoneConfiguration),
                Language = GetLanguage(zoneConfiguration),
                ZoneType = GetZoneType(zoneConfiguration),
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
    }

    public class AutoShitGenerator : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            yield return new AutoContrastBitmapFilter().Apply(image);
        }
    }
}