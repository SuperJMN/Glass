using MessagingToolkit.Barcode.Common;

namespace Glass.Barcodes.MessagingToolkit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using global::MessagingToolkit.Barcode;
    using Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;

    public class MessagingToolkitZoneBasedBarcodeReader : IImageToTextConverter
    {
        private readonly BarcodeDecoder barcodeReader = new BarcodeDecoder();

        public IEnumerable<RecognitionResult> Recognize(IImage bitmap, ZoneConfiguration config)
        {
            var binaryBitmap  = new BinaryBitmap(new HybridBinarizer(new BitmapSourceLuminanceSource(bitmap.ToGray().ToBitmapSource())));

            var recognizer = Task.Run(() => barcodeReader.Decode(binaryBitmap, new Dictionary<DecodeOptions, object>()))
                .ToObservable();

            var result = recognizer
                .Timeout(TimeSpan.FromSeconds(2))
                .Catch<Result, TimeoutException>(arg => Observable.Return<Result>(null))
                .Catch<Result, NotFoundException>(arg => Observable.Return<Result>(null));

            var text = result.ToTask().Result?.Text;

            yield return new RecognitionResult(text, 1D);
        }

        public IEnumerable<ImageTarget> ImageTargets
            => new Collection<ImageTarget> {new ImageTarget {Symbology = Symbology.Barcode, FilterTypes = FilterType.All}};
    }
}