namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;
    using Leadtools.ImageProcessing.Core;

    public class ExtendedFilter : IImageFilter
    {
        public BitmapSource Apply(BitmapSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new AutoColorLevelCommand { Type = AutoColorLevelCommandType.Contrast }.Run(r);
                new AutoBinarizeCommand { Factor = 2 }.Run(r);
                new MedianCommand(2).Run(r);

                return r.ToBitmapSource();
            }
        }

        public override string ToString()
        {
            return "AutoBinarizeImageFilter";
        }
    }
}