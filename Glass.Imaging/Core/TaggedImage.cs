namespace Glass.Imaging.Core
{
    using System.Windows.Media;

    public class TaggedImage
    {
        public ImageSource Image { get; set; }
        public string Tag { get; set; }

        public TaggedImage(ImageSource image, string tag)
        {
            Image = image;
            Tag = tag;
        }
    }
}