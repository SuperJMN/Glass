namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Leadtools.ImageProcessing.Core;

    public class DeskewBitmapFilter : IBitmapFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new DeskewCommand() { Flags = DeskewCommandFlags.RotateBicubic }.Run(r);
                return r.ToBitmapSource();
            }
        }

        public override string ToString()
        {
            return "DeskewBitmapFilter";
        }
    }
}