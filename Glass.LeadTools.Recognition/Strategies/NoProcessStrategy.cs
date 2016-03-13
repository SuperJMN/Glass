namespace Glass.LeadTools.Recognition.Strategies
{
    using System.Windows.Media;

    internal class NoProcessStrategy : IStrategy
    {
        public ImageSource Apply(ImageSource image)
        {
            return image.Clone();
        }
    }
}