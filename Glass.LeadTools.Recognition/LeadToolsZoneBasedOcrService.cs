namespace Glass.LeadTools.Recognition
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using ImagingExtensions;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;

    public class LeadToolsZoneBasedOcrService : OcrService
    {
        private const string OcrEngineFolder = @"OcrAdvantageRuntime";

        private OcrEngine engine;

        public LeadToolsZoneBasedOcrService(ILeadToolsLicenseApplier licenseApplier)
        {
            licenseApplier.ApplyLicense();
        }

        public override double SourceScaleForOcr => 0.3;
        public override bool IsSourceScalingEnabledForOcr => true;

        public override IEnumerable<IBitmapBatchGenerator> BitmapGenerators { get; } = new Collection<IBitmapBatchGenerator>
        {
            new AutoColorLevelFilterGenerator(),
        };

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

        public override IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config)
        {
            bitmap = ScaleIfEnabled(bitmap);

            var bitmaps = BitmapGenerators.SelectMany(g => g.Generate(bitmap));
            var recognitions = bitmaps.SelectMany(bmp => RecognizeCore(config, bmp));
            return recognitions;
        }

        public IEnumerable<RecognitionResult> RecognizeScaleEachVariation(BitmapSource bitmap, ZoneConfiguration config)
        {
            var bitmaps = BitmapGenerators.SelectMany(g => g.Generate(bitmap));
            var scaled = bitmaps.Select(ScaleIfEnabled);
            var recognitions = scaled.SelectMany(bmp => RecognizeCore(config, bmp));
            return recognitions;
        }

        private IEnumerable<RecognitionResult> RecognizeCore(ZoneConfiguration config, BitmapSource bmp)
        {
            using (var page = OcrEngine.CreatePage(bmp.ToRasterImage(), OcrImageSharingMode.AutoDispose))
            {
                var ocrZone = CreateOcrZoneForField(bmp, config);
                page.Zones.Add(ocrZone);
                page.Recognize(null);
                var text = page.GetText(0);

                var confidence = GetConfidence(page);

                var filteredText = config.TextualDataFilter.GetBestMatchFromRaw(text);
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

        public override IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Text, FilterTypes = FilterType.All } };

        private OcrZone CreateOcrZoneForField(BitmapSource bitmap, ZoneConfiguration zoneConfiguration)
        {
            var leadRect = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight).ToLeadRectRect();

            var readZone = new OcrZone
            {
                Bounds = new LogicalRectangle(leadRect),
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
}