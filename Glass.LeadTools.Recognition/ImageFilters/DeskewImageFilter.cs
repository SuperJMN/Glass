namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Core;

    internal class DeskewImageFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new DeskewCommand().Run(r);
                return r.ToBitmapSource();
            }
        }

        public override string ToString()
        {
            return "DeskewImageFilter";
        }
    }
}