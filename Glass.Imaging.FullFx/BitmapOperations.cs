namespace Glass.Imaging.FullFx
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Core;
    using Transform = Core.Transform;

    public class BitmapOperations : IBitmapOperations
    {
        public BitmapSource Crop(BitmapSource bitmap, Rect cropBounds)
        {
            cropBounds = cropBounds.ConvertFrom96pppToBitmapDpi(bitmap.DpiX, bitmap.DpiY);
            var cropped = new CroppedBitmap(bitmap, cropBounds.ToInt32Rect());
            return cropped;
        }

        public BitmapSource Rotate(BitmapSource bitmap, double angle)
        {
            var copy = new TransformedBitmap(bitmap, new RotateTransform(angle));
            return copy;
        }

        public BitmapSource Transform(BitmapSource bitmap, Transform transform)
        {
            var rotated = Rotate(bitmap, transform.Rotation);
            var cropInPixels = transform.Bounds.ConverToRawPixels(bitmap.DpiX, bitmap.DpiY);
            return Crop(rotated, cropInPixels);
        }


    }
}