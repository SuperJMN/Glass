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
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using ImagingExtensions;
    using ImagingExtensions.ImageFilters;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;

    public class LeadToolsZoneBasedOcrService : IImageToTextConverter
    {
        private const string OcrEngineFolder = @"OcrAdvantageRuntime";

        private readonly RasterCodecs codecs = new RasterCodecs();
        private OcrEngine engine;

        public LeadToolsZoneBasedOcrService(ILeadToolsLicenseApplier licenseApplier)
        {
            licenseApplier.ApplyLicense();
        }

        public double SourceScaleForOcr { get; set; } = 0.3;
        public bool IsSourceScalingEnabledForOcr { get; set; } = true;

        private OcrEngine OcrEngine
        {
            get
            {
                if (engine == null)
                {
                    engine = (OcrEngine)OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
                    engine.Startup(codecs, null, null, OcrEngineFolder);                    
                }

                return engine;
            }
        }

        public IEnumerable<string> Recognize(BitmapSource bitmap, ZoneConfiguration barcodeConfig)
        {
            var rasterImage = GetImageToOcr(bitmap).ToRasterImage();
            using (var page = OcrEngine.CreatePage(rasterImage, OcrImageSharingMode.AutoDispose))
            {               
                var ocrZone = CreateOcrZoneForField(barcodeConfig);
                page.Zones.Add(ocrZone);
                page.Recognize(null);
                var text = page.GetText(0);

                var confidence = GetConfidence(page);
                
                yield return text;
            }
        }

        private double GetConfidence(IOcrPage page)
        {
            var recognizedCharacters = page.GetRecognizedCharacters();
            var findZoneCharacters = recognizedCharacters.FindZoneCharacters(0);
            return findZoneCharacters.Average(character => character.Confidence) / 100D;
        }

        public IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Text, FilterTypes = FilterType.All } };

        private static BitmapSource OptimizeImageForOcr(BitmapSource bitmap)
        {
            return new AutoContrastBitmapFilter().Apply(bitmap);
        }

        private BitmapSource GetImageToOcr(BitmapSource image)
        {
            var scale = SourceScaleForOcr;
            var isScalingEnabled = IsSourceScalingEnabledForOcr;

            var finalBitmap = isScalingEnabled ? new TransformedBitmap(image, new ScaleTransform(scale, scale)) : image;

            return OptimizeImageForOcr(finalBitmap);
        }

        private OcrZone CreateOcrZoneForField(ZoneConfiguration zoneConfiguration)
        {
            var zoneConfig = zoneConfiguration;

            var bounds = zoneConfig.Bounds;
            bounds.Scale(SourceScaleForOcr, SourceScaleForOcr);
            var leadRectRect = bounds.ToLeadRectRect();
            
            var readZone = new OcrZone
            {
                Bounds = new LogicalRectangle(leadRectRect),
                Name = zoneConfiguration.Id,
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
    }
}