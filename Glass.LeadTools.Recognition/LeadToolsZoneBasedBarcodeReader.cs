namespace Glass.LeadTools.Recognition
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using ImagingExtensions;
    using ImagingExtensions.ImageFilters;
    using Leadtools.Barcode;
    using Leadtools.Forms;

    public class LeadToolsZoneBasedBarcodeReader : IImageToTextConverter
    {
        public readonly IEnumerable<BarcodeStrategy> BarcodeStrategies = new[]
        {
            new BarcodeStrategy
            {
                BitmapFilter = new NoProcessBitmapFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },
            new BarcodeStrategy
            {
                BitmapFilter = new HistogramContrastBitmapFilter(),
                ImageType = BarcodeImageType.Picture
            },
            new BarcodeStrategy
            {
                BitmapFilter = new HistogramContrastBitmapFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },
            new BarcodeStrategy
            {
                BitmapFilter = new ExtendedFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },
            new BarcodeStrategy
            {
                BitmapFilter = new AutoContrastBitmapFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            }
        };

        public LeadToolsZoneBasedBarcodeReader(ILeadToolsLicenseApplier licenseApplier)
        {
            licenseApplier.ApplyLicense();
        }

        public BarcodeReadOptions[] CoreReadOptions { get; set; } = {
            new OneDBarcodeReadOptions
            {
                AllowPartialRead = false,
                SearchDirection = BarcodeSearchDirection.HorizontalAndVertical
            }
        };

        public IEnumerable<BarcodeSymbology> BarcodeSymbologies { get; set; } =
            new[]
            {
                BarcodeSymbology.Code3Of9,
                BarcodeSymbology.Code93,
                BarcodeSymbology.QR,
                BarcodeSymbology.Datamatrix
            };

        public IEnumerable<RecognitionResult> Recognize(IImage bitmap, ZoneConfiguration config)
        {
            var coreReadOptions = CoreReadOptions;

            var leadRect = new LogicalRectangle(0, 0, bitmap.Width, bitmap.Height, LogicalUnit.Pixel);

            var unitsOfWork = from strategy in BarcodeStrategies
                                   let filteredImage = strategy.BitmapFilter.Apply(bitmap)
                                   select new { ImageType = strategy.ImageType, FilteredImage = filteredImage };

            var selectMany = unitsOfWork.SelectMany(u => GetText(leadRect, coreReadOptions, u.FilteredImage, u.ImageType));
            return selectMany.Select(s => new RecognitionResult(s, 1));            
        }

        private IEnumerable<string> GetText(LogicalRectangle leadRect, BarcodeReadOptions[] coreReadOptions, IImage image, BarcodeImageType imageType)
        {
            var engine = new BarcodeEngine();
            engine.Reader.ImageType = imageType;
            var barcodeDatas = engine.Reader.ReadBarcodes(image.ToBgr().ToBitmapSource().ToRasterImage(), leadRect, 10, BarcodeSymbologies.ToArray(), coreReadOptions);
            var textForStrategy = barcodeDatas.Where(data => data.Value != null).Select(data => data.Value);
            return textForStrategy;
        }


        public IEnumerable<ImageTarget> ImageTargets => new Collection<ImageTarget> { new ImageTarget { Symbology = Symbology.Barcode, FilterTypes = FilterType.All } };
    }
}