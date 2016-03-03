﻿namespace Glass.LeadTools.ImagingExtensions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
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

        public static RasterImage OptimizeImageForOcr(this RasterImage image)
        {
            var clone = image.Clone();
            //new AutoBinarizeCommand(0, AutoBinarizeCommandFlags.UseAutoThreshold).Run(clone);
            //new ColorResolutionCommand { BitsPerPixel = 1 }.Run(clone);
            //new MinimumCommand { Dimension = 4 }.Run(clone);

            new AutoColorLevelCommand().Run(clone);

            return clone;
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

        public static ImageSource ToBitmapSource(this RasterImage rasterImage)
        {
            var convertToSource = RasterImageConverter.ConvertToSource(rasterImage, ConvertToSourceOptions.None);
            convertToSource.Freeze();
            return convertToSource;
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

        public static ImageSource Resize(this ImageSource image, Size maxSize)
        {
            var rasterImage = ToRasterImage(image);
            var currentSize = new Size(rasterImage.ImageSize.Width, rasterImage.ImageSize.Height);
            var newSize = currentSize.PreserveAspectRatioWithinBounds(maxSize);

            var destImage = new RasterImage(
                RasterMemoryFlags.Conventional,
                (int)newSize.Width,
                (int)newSize.Height,
                rasterImage.BitsPerPixel,
                rasterImage.Order,
                rasterImage.ViewPerspective,
                rasterImage.GetPalette(),
                IntPtr.Zero,
                0);

            var cmd = new ResizeCommand(destImage, RasterSizeFlags.Bicubic);
            cmd.Run(rasterImage);
            return ToBitmapSource(destImage);
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