namespace Glass.LeadTools.Recognition.Strategies
{
    using System.Windows.Media;

    public interface IStrategy
    {
        ImageSource Apply(ImageSource image);
    }
}