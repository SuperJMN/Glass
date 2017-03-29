using System.Collections.Generic;

namespace Glass.Barcodes.ZXing
{
    using System.Collections.ObjectModel;
    using DotImaging;
    using global::ZXing;
    using Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;

    public class ZXingBarcodeReader : IImageToTextConverter
    {
        public IEnumerable<RecognitionResult> Recognize(IImage image, ZoneConfiguration config)
        {
            var decoder = new BarcodeReader();
            var decodeResult = decoder.Decode(image.ToBgra().ToBitmap());
            if (decodeResult == null)
            {
                yield break;
            }

            yield return new RecognitionResult(decodeResult.Text, 1D);
        }

        public IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Barcode, FilterTypes = FilterType.All } };
    }
}
