namespace Glass.LeadTools.Recognition
{
    using ImageFilters;
    using Leadtools.Barcode;

    public class ScoreResult
    {
        public double Score { get; set; }
        public string Text { get; set; }
        public BarcodeImageType ImageType { get; set; }
        public IImageFilter ImageFilter { get; set; }

        public override string ToString()
        {
            return $"ImageType: {ImageType}, Strategy: {ImageFilter}";
        }
    }
}