namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Core;

    internal class AutoBinarizeImageFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new AutoBinarizeCommand().Run(r);
                return r.ToBitmapSource();
            }
        }

        public override string ToString()
        {
            return "AutoBinarizeImageFilter";
        }
    }
}