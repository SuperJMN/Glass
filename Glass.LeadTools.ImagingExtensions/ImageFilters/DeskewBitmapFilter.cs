namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Core;

    public class DeskewBitmapFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            using (var r = image.ToBgr().ToBitmapSource().ToRasterImage())
            {
                new DeskewCommand() { Flags = DeskewCommandFlags.RotateBicubic }.Run(r);
                return r.ToImage();
            }
        }

        public override string ToString()
        {
            return "DeskewBitmapFilter";
        }
    }
}