namespace Glass.Imaging.FullFx
{
    using System.Drawing;
    using System.Windows;
    using Accord.Extensions.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using Core;
    using DotImaging;
    using Transform = Core.Transform;

    public static class ConversionExtensions
    {
        public static Rectangle ToWindowsRect(this Rect cropBounds)
        {
            return new Rectangle((int)cropBounds.Left, (int)cropBounds.Top, (int)cropBounds.Width, (int)cropBounds.Height);
        }
    }

    public class BitmapOperations : IBitmapOperations
    {
        public IImage Crop(IImage bitmap, Rect cropBounds)
        {
            var rect = new DotImaging.Primitives2D.Rectangle((int) cropBounds.Left, (int) cropBounds.Top, (int) cropBounds.Width, (int) cropBounds.Height);
            return bitmap.GetSubRect(rect);          
        }

        public IImage Rotate(IImage bitmap, double angle)
        {
            var a = bitmap.ToBgr().Lock().AsAForgeImage();
            return new RotateBicubic(angle).Apply(a).AsImage();
        }

        public IImage Transform(IImage bitmap, Transform transform)
        {
            var rotated = Rotate(bitmap, transform.Rotation);
            return Crop(rotated, transform.Bounds);
        }
    }
}