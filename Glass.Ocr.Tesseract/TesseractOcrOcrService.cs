namespace Glass.Ocr.Tesseract
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using global::Tesseract;
    using Imaging;
    using Imaging.Filters;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using LeadTools.ImagingExtensions.ImageFilters;

    public class TesseractOcrOcrService : IImageToTextConverter
    {
        private readonly TesseractEngine engine;
        public double SourceScaleForOcr { get; set; } = 0.3;
        public bool IsSourceScalingEnabledForOcr { get; set; } = false;

        public IEnumerable<IImageFilter> ImageFilters { get; set; }

        public TesseractOcrOcrService()
        {
            ImageFilters = new List<IImageFilter>
            {
                new AutoContrastFilterFree(),
            };

            engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default);
        }

        public IEnumerable<string> Recognize(BitmapSource bitmap, ZoneConfiguration barcodeConfig)
        {
            var finalBitmap = IsSourceScalingEnabledForOcr ? new TransformedBitmap(bitmap, new ScaleTransform(SourceScaleForOcr, SourceScaleForOcr)) : bitmap;

            foreach (var filter in ImageFilters)
            {
                var inputBitmap = filter.Apply(finalBitmap);
                var bytes = ConvertToTiffByteArray(inputBitmap);

                SetVariablesAccordingToConfig(engine, barcodeConfig);

                using (var img = Pix.LoadTiffFromMemory(bytes))
                {
                    using (var page = engine.Process(img, PageSegMode.SingleBlock))
                    {
                        if (IsAlpha(barcodeConfig))
                        {
                            yield return page.GetText();
                        }
                        else
                        {
                            if (page.GetMeanConfidence() > 0.80)
                            {
                                yield return page.GetText();
                            }
                        }
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