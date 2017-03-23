namespace Glass.LeadTools.ImagingExtensions
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DotImaging;
    using Leadtools;
    using Leadtools.Codecs;
    using Leadtools.ImageProcessing;
    using Leadtools.ImageProcessing.Color;
    using Leadtools.Windows.Media;
    using Transform = Imaging.Core.Transform;

    public static class ImageUtils
    {
        private static readonly RasterCodecs Codecs = new RasterCodecs();

        static ImageUtils()
        {
            Codecs.Options.Jpeg.Save.QualityFactor = 25;
        }



        public static LeadRect ToLeadRectRect(this Rect rectCrop)
        {
            return new LeadRect((int)rectCrop.X, (int)rectCrop.Y, (int)rectCrop.Width, (int)rectCrop.Height);
        }

        public static RasterImage Tranform(this RasterImage originalImage, Transform transformProperties)
        {
            var image = originalImage.Clone();

            Transform(transformProperties, image);

            return image;
        }

        public static RasterImage OptimizeImageForBarcode(this RasterImage image)
        {
            var clone = image.Clone();
            new AutoColorLevelCommand().Run(clone);
            return clone;
        }

        private static void Transform(Transform transformProperties, RasterImage result)
        {
            var cropCommand = new CropCommand();
            var rotateCommand = new RotateCommand { Angle = (int)(transformProperties.Rotation * 100) };

            var rectCrop = new LeadRect(
                (int)transformProperties.Bounds.Left,
                (int)transformProperties.Bounds.Top,
                (int)transformProperties.Bounds.Width,
                (int)transformProperties.Bounds.Height);

            cropCommand.Rectangle = rectCrop;

            rotateCommand.Run(result);
            if (!transformProperties.Bounds.IsEmpty)
            {
                cropCommand.Run(result);
            }
        }

        public static IImage ToImage(this RasterImage rasterImage)
        {
            var convertToSource = RasterImageConverter.ConvertToSource(rasterImage, ConvertToSourceOptions.None);
            convertToSource.Freeze();
            return (IImage) convertToSource;
        }

        public static RasterImage ToRasterImage(this ImageSource rasterImage)
        {
            return RasterImageConverter.ConvertFromSource(rasterImage, ConvertFromSourceOptions.None);
        }

        public static void SaveAsJpeg(this ImageSource image, string path)
        {
            var rasterImage = ToRasterImage(image);
            Codecs.Save(rasterImage, path, RasterImageFormat.Jpeg, 24);
        }

        public static Size PreserveAspectRatioWithinBounds(this Size size, Size bounds)
        {
            var aspectRactio = size.Width / size.Height;

            var fitWidth = new Size(bounds.Width, bounds.Width / aspectRactio);
            var fitHeight = new Size(bounds.Height * aspectRactio, bounds.Height);

            return bounds.Contains(fitWidth) ? fitWidth : fitHeight;
        }

        public static bool Contains(this Size outer, Size inner)
        {
            return outer.Width >= inner.Width && outer.Height >= inner.Height;
        }
    }
}