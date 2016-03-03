namespace Glass.Imaging.Core
{
    using System.Windows;

    public class Transform
    {
        public Transform(Rect bounds, double rotation)
        {
            Bounds = bounds;
            Rotation = rotation;
        }

        public double Rotation { get; private set; }
        public Rect Bounds { get; private set; }
    }
}