namespace Glass.LeadTools.ImagingExtensions.ImageFilters
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Imaging;
    using Leadtools.ImageProcessing.Color;
    using Leadtools.ImageProcessing.Core;

    public class ExtendedFilter : IBitmapFilter
    {
        public IImage Apply(IImage image)
        {
            using (var r = image.FromImageToRasterImage())
            {
                new AutoColorLevelCommand { Type = AutoColorLevelCommandType.Contrast }.Run(r);
                new AutoBinarizeCommand { Factor = 2 }.Run(r);
                new MedianCommand(2).Run(r);

                return r.ToImage();
            }
        }

        public override string ToString()
        {
            return "AutoBinarizeBitmapFilter";
        }
    }
}