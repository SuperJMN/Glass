namespace Glass.LeadTools.Recognition
{
    using System.Collections.Generic;
    using ImageFilters;
    using Leadtools.Barcode;

    public class QualityOptions
    {
        public readonly IEnumerable<BarcodeStrategy> BarcodeStrategies = new[]
        {
            new BarcodeStrategy
            {
                ImageFilter = new NoProcessImageFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },
            new BarcodeStrategy
            {
                ImageFilter = new HistogramContrastImageFilter(),
                ImageType = BarcodeImageType.Picture

            },
            new BarcodeStrategy
            {
                ImageFilter = new HistogramContrastImageFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },
            new BarcodeStrategy
            {
                ImageFilter = new ExtendedFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },

            new BarcodeStrategy
            {
                ImageFilter = new AutoContrastImageFilter(),
                ImageType = BarcodeImageType.ScannedDocument
            },
        };

        public BarcodeReadOptions[] CoreReadOptions { get; } = {
            new OneDBarcodeReadOptions()
            {
                AllowPartialRead = false,
                SearchDirection = BarcodeSearchDirection.HorizontalAndVertical,
            },
        };
    }
}