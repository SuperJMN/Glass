namespace Glass.LeadTools.Recognition.ImageFilters
{
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using ImagingExtensions;
    using Leadtools;
    using Leadtools.Codecs;
    using Leadtools.ImageProcessing.Color;
    using Leadtools.ImageProcessing.Core;

    internal class ExtendedFilter : IImageFilter
    {
        public ImageSource Apply(ImageSource image)
        {
            using (var r = image.ToRasterImage())
            {
                new AutoColorLevelCommand() { Type = AutoColorLevelCommandType.Contrast }.Run(r);
                new AutoBinarizeCommand { Factor = 2 }.Run(r);
                new MedianCommand(2).Run(r);

                using (var c = new RasterCodecs())
                {
                    var path = ((BitmapImage)((((CroppedBitmap)image)).Source)).UriSource.OriginalString;
                    var name = Path.GetFileName(path);
                    using (var f = new FileStream(name, FileMode.Create))
                    {
                        c.Save(r, f, RasterImageFormat.Jpeg, 24);
                    }
                }

                return r.ToBitmapSource();
            }
        }

        public override string ToString()
        {
            return "AutoBinarizeImageFilter";
        }
    }
}