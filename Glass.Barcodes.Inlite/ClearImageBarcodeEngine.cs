using System.Collections.Generic;
using System.Linq;

namespace Glass.Barcodes.Inlite
{
    using System.Collections.ObjectModel;
    using DotImaging;
    using global::Inlite.ClearImageNet;
    using Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;

    public class ClearImageBarcodeEngine : IImageToTextConverter
    {
        private readonly BarcodeReader barcodeReader = new BarcodeReader
        {
            Code39 = true,
            Code128 = true,
            Code39basic = true,
            Codabar = true,
            Ean8 = true,
            Vertical = true,
            Matrix2of5 = true,
        };

        public IEnumerable<RecognitionResult> Recognize(IImage bitmap, ZoneConfiguration config)
        {            
            string text;
            try
            {
                var bmp = bitmap.ToGray().ToBitmap();
                var barcodes = barcodeReader.Read(bmp);
                text = barcodes.FirstOrDefault()?.Text;
            }
            catch
            {
                text = null;
            }

            yield return new RecognitionResult(text, 1D);
        }

        public IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Barcode, FilterTypes = FilterType.All } };
    }
}
