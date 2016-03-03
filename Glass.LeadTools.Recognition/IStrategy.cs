namespace Glass.LeadTools.Recognition
{
    using System.Windows.Media;

    internal interface IStrategy
    {
        ImageSource Apply(ImageSource image);
    }
}