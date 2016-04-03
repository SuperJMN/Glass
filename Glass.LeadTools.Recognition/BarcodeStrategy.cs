namespace Glass.LeadTools.Recognition
{
    using Imaging;
    using ImagingExtensions.ImageFilters;
    using Leadtools.Barcode;

    public class BarcodeStrategy
    {
        public IBitmapFilter BitmapFilter { get; set; }
        public BarcodeImageType ImageType { get; set; } 
    }
}