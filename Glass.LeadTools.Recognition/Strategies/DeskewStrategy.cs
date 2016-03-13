namespace Glass.LeadTools.Recognition.Strategies
{
    using System.Windows.Media;
    using ImagingExtensions;
    using Leadtools.ImageProcessing.Core;

    internal class DeskewStrategy : IStrategy
    {
        public ImageSource Apply(ImageSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new DeskewCommand().Run(r);
                return r.ToBitmapSource();
            }
        }
    }
}