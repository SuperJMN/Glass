namespace Glass.LeadTools.Recognition
{
    using Imaging;
    using ImagingExtensions.ImageFilters;
    using Leadtools.Barcode;

    public class BarcodeStrategy
    {
        public IImageFilter ImageFilter { get; set; }
        public BarcodeImageType ImageType { get; set; } 
    }
}