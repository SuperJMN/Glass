namespace Glass.LeadTools.Recognition.Strategies
{
    using System.Windows.Media;

    internal interface IStrategy
    {
        ImageSource Apply(ImageSource image);
    }
}