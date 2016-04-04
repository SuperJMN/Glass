﻿namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Core;
    using PostProcessing;

    public class CompositeOpticalRecognizer : IZoneBasedRecognitionService
    {
        private readonly IEnumerable<IImageToTextConverter> engines;

        public CompositeOpticalRecognizer(IEnumerable<IImageToTextConverter> engines)
        {
            this.engines = engines;
        }

        public RecognizedPage Recognize(BitmapSource image, RecognitionConfiguration configuration)
        {
            var results = GetRecognizedZones(image, configuration).ToList();
            PostProcess(results);
            return new RecognizedPage(image, results);
        }

        private IEnumerable<RecognizedZone> GetRecognizedZones(BitmapSource bitmap, RecognitionConfiguration configuration)
        {
            foreach (var zoneConfiguration in configuration.Zones)
            {
                var applicableEngines = engines.Where(e => IsValidTarget(zoneConfiguration, e)).ToList();

                var zoneBitmap = ImagingContext.BitmapOperations.Crop(bitmap, zoneConfiguration.Bounds);
                zoneBitmap.Freeze();

                if (applicableEngines.Any())
                {
                    var resultsFromAllEngines = from engine in applicableEngines
                        select engine.Recognize(zoneBitmap, zoneConfiguration);

                    var flatResults = resultsFromAllEngines.SelectMany(t => t);

                    var result = new OpticalResultSelector().Select(flatResults, zoneConfiguration);
                    yield return new RecognizedZone(zoneBitmap, zoneConfiguration, result?.Text);
                }
            }
        }

        private static bool IsValidTarget(ZoneConfiguration configuration, IImageToTextConverter engine)
        {
            return engine.ImageTargets.Any(target => target.Symbology == configuration.Symbology && target.FilterTypes.HasFlag(configuration.TextualDataFilter.FilterType));
        }

        private static void PostProcess(IEnumerable<RecognizedZone> recognizedZones)
        {
            foreach (var recognizedZone in recognizedZones)
            {
                var processed = recognizedZone.ZoneConfig.TextualDataFilter.Filter(recognizedZone.RecognizedText);
                recognizedZone.RecognizedText = processed;
            }
        }
    }
}