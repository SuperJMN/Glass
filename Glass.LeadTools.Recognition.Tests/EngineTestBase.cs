namespace Glass.Imaging.Recognition.Tests
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Imaging;
    using Core;
    using FullFx;
    using ZoneConfigurations;
    using Xunit.Abstractions;

    public abstract class EngineTestBase
    {
        private readonly ITestOutputHelper output;

        protected EngineTestBase(ITestOutputHelper output)
        {
            this.output = output;
            ImagingContext.BitmapOperations = new BitmapOperations();
        }

        protected abstract IImageToTextConverter GetSut();

        protected static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }

        protected string ExtractFirstFiltered(BitmapSource bitmap, ITextualDataFilter filter, Symbology symbology)
        {
            var sut = GetSut();

            var bounds = new Rect(0, 0, bitmap.Width, bitmap.Height);
            var zoneConfiguration = new ZoneConfiguration { Bounds = bounds, TextualDataFilter = filter, Id = "", Symbology = symbology };

            var text = sut.Recognize(bitmap, zoneConfiguration).FirstOrDefault();
            var filtered = zoneConfiguration.TextualDataFilter.Filter(text);

            return filtered;
        }
    }
}