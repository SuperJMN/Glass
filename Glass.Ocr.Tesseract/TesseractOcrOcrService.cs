namespace Glass.Imaging.Recognition.Tesseract
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Filters;
    using global::Tesseract;
    using Generators;
    using PostProcessing;
    using ZoneConfigurations;

    public class TesseractOcrOcrService : OcrService
    {
        private readonly TesseractEngine engine;
     
        public TesseractOcrOcrService()
        {   
            engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default);
        }

        public override double SourceScaleForOcr => 0.3;
        public override bool IsSourceScalingEnabledForOcr => true;

        public override IEnumerable<IBitmapBatchGenerator> BitmapGenerators { get; } = new Collection<IBitmapBatchGenerator>
        {
            new ThresholdGenerator(),
        };

        public override IEnumerable<RecognitionResult> Recognize(BitmapSource bitmap, ZoneConfiguration config)
        {
            bitmap = ScaleIfEnabled(bitmap);

            foreach (var inputBitmap in BitmapGenerators.SelectMany(generator => generator.Generate(bitmap)))
            {
                var bytes = ConvertToTiffByteArray(inputBitmap);

                SetVariablesAccordingToConfig(engine, config);

                using (var img = Pix.LoadTiffFromMemory(bytes))
                {
                    using (var page = engine.Process(img, PageSegMode.SingleBlock))
                    {
                        var text = config.TextualDataFilter.GetBestMatchFromRaw(page.GetText());

                        var confidence = page.GetMeanConfidence() * 0.9;
                        yield return new RecognitionResult(text, confidence);
                    }
                }

            }
        }

        private static void SetVariablesAccordingToConfig(TesseractEngine engine, ZoneConfiguration barcodeConfig)
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

        public override IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Text, FilterTypes = FilterType.All } };

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