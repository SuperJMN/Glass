namespace Glass.Imaging
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using DotImaging;
    using PostProcessing;

    public class CompositeOpticalRecognizer : IZoneBasedRecognitionService
    {
        private readonly IEnumerable<IImageToTextConverter> engines;

        public CompositeOpticalRecognizer(IEnumerable<IImageToTextConverter> engines)
        {
            this.engines = engines;
        }

        public RecognizedPage Recognize(IImage image, RecognitionConfiguration configuration)
        {
            var results = GetRecognizedZones(image, configuration).ToList();
            return new RecognizedPage(image, results);
        }

        private IEnumerable<RecognizedZone> GetRecognizedZones(IImage bitmap, RecognitionConfiguration configuration)
        {
            foreach (var zoneConfiguration in configuration.Zones)
            {
                var applicableEngines = engines.Where(e => IsValidTarget(zoneConfiguration, e)).ToList();

                var zoneBitmap = ImagingContext.BitmapOperations.Crop(bitmap, zoneConfiguration.Bounds);
                //var zoneBitmap = bitmap;


                if (applicableEngines.Any())
                {
                    var resultsFromAllEngines = from engine in applicableEngines
                        select engine.Recognize(zoneBitmap, zoneConfiguration);

                    var flatResults = resultsFromAllEngines.SelectMany(t => t).ToList();

                    var result = OpticalResultSelector.ChooseBest(flatResults, zoneConfiguration);
                    yield return new RecognizedZone(zoneBitmap, zoneConfiguration, result);
                }
            }
        }

        private static bool IsValidTarget(ZoneConfiguration configuration, IImageToTextConverter engine)
        {
            return engine.ImageTargets.Any(target => target.Symbology == configuration.Symbology && target.FilterTypes.HasFlag(configuration.TextualDataFilter.FilterType));
        }
    }
}