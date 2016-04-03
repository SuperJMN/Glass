namespace Glass.Ocr.Tesseract
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using global::Tesseract;
    using Imaging;
    using Imaging.Filters;
    using Imaging.Generators;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;

    public class TesseractOcrOcrService : IImageToTextConverter
    {
        private readonly TesseractEngine engine;
        public double SourceScaleForOcr { get; set; } = 0.3;
        public bool IsSourceScalingEnabledForOcr { get; set; } = false;

        public IEnumerable<IBitmapFilter> ImageFilters { get; set; }

        public TesseractOcrOcrService()
        {
            ImageFilters = new List<IBitmapFilter>
            {
                new OtsuThresholdFilterFree(),
                new ThresholdFilter(80),
            };

            BitmapGenerators = new List<IBitmapBatchGenerator> { new OtsuGenerator() };

            engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default);
        }

        public IEnumerable<IBitmapBatchGenerator> BitmapGenerators { get; set; }

        public IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config)
        {
            bitmap = new DeskewFilter().Apply(bitmap);

            foreach (var inputBitmap in BitmapGenerators.SelectMany(generator => generator.Generate(bitmap)))
            {
                var finalBitmap = IsSourceScalingEnabledForOcr ? new TransformedBitmap(inputBitmap, new ScaleTransform(SourceScaleForOcr, SourceScaleForOcr)) : inputBitmap;
                var bytes = ConvertToTiffByteArray(finalBitmap);

                SetVariablesAccordingToConfig(engine, config);

                using (var img = Pix.LoadTiffFromMemory(bytes))
                {
                    using (var page = engine.Process(img, PageSegMode.SingleBlock))
                    {
                        var text = config.TextualDataFilter.Filter(page.GetText());

                        var confidence = page.GetMeanConfidence();
                        yield return new RecognitionResult(text, confidence);
                    }
                }

            }
        }

        private static bool IsAlpha(ZoneConfiguration barcodeConfig)
        {
            return barcodeConfig.TextualDataFilter.FilterType == FilterType.Alpha ||
                   barcodeConfig.TextualDataFilter.FilterType == FilterType.AlphaOnly;
        }

        private void SetVariablesAccordingToConfig(TesseractEngine engine, ZoneConfiguration barcodeConfig)
        {
            if (barcodeConfig.TextualDataFilter.FilterType == FilterType.Alpha)
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-");
            }

            if (barcodeConfig.TextualDataFilter.FilterType == FilterType.AlphaOnly)
            {
                engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ-");
            }

            if (barcodeConfig.TextualDataFilter.FilterType == FilterType.Digits)
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789");
            }

            if (barcodeConfig.TextualDataFilter.FilterType == FilterType.Number)
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789,.");
            }
        }

        public IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Text, FilterTypes = FilterType.All } };

        private static byte[] ConvertToTiffByteArray(BitmapSource bitmap)
        {
            var encoder = new TiffBitmapEncoder();
            var memoryStream = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);

            var bytes = ReadFully(memoryStream);
            return bytes;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}