using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Barcodes.Inlite
{
    using System.Collections.ObjectModel;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using global::Inlite.ClearImageNet;
    using Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;

    public class InliteBarcodeEngine : IImageToTextConverter
    {
        BarcodeReader barcodeReader = new BarcodeReader();

        public IEnumerable<RecognitionResult> Recognize(IImage bitmap, ZoneConfiguration config)
        {
            var writeableBitmap = new WriteableBitmap(bitmap);
            string text;
            try
            {
                //var bit = new 
                //var result = barcodeReader.Read(new BitmapImage(), new Dictionary<DecodeOptions, object>());
                //text = result.Text;
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
