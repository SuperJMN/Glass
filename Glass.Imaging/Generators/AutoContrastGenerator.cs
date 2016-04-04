namespace Glass.Imaging.Generators
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using Filters;
    using Imaging;

    public class ContrastStrechGeneractor : IBitmapBatchGenerator
    {
        public IEnumerable<BitmapSource> Generate(BitmapSource image)
        {
            yield return new ContrastStrechFilter().Apply(image);
        }
    }
}