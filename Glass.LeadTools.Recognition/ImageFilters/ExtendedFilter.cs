namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Color;
    using Leadtools.ImageProcessing.Core;

    internal class ExtendedFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
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