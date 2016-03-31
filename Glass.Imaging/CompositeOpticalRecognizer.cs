namespace Glass.Imaging
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
                string text;

                if (applicableEngines.Any())
                {
                    var textsFromEngines = from engine in applicableEngines
                        select engine.Recognize(zoneBitmap, zoneConfiguration);

                    var texts = textsFromEngines.SelectMany(t => t);
                    var textualDataFilter = zoneConfiguration.TextualDataFilter;
                    var filteredTexts = texts.Select(s => textualDataFilter.Filter(s));

                    var scores = from t in filteredTexts
                                 let score = textualDataFilter.Evaluator.GetScore(t)
                        orderby score descending
                        select new {Text = t, Score = score};

                    text = scores.FirstOrDefault()?.Text;                    
                }
                else
                {
                    text = null;
                }

                yield return new RecognizedZone(zoneBitmap, zoneConfiguration, text);
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