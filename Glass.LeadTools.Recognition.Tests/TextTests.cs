namespace Glass.LeadTools.Recognition.Tests
{
    using System.Windows.Media.Imaging;
    using DataProviders.Text;
    using Imaging.ZoneConfigurations;
    using Imaging.ZoneConfigurations.Alphanumeric;
    using Imaging.ZoneConfigurations.Numeric;
    using Xunit;

    public class TextTests : OpticalRecognitionTestBase
    {
        [Theory]
        [ClassData(typeof(NumericTestDataProvider))]
        public void Numeric(BitmapSource image, string expected)
        {
            Assert.Equal(expected, Extract(image, new NumericStringFilter { MinLength = 6, MaxLength = 6, }, Symbology.Text));
        }

        [Theory]
        [ClassData(typeof(TextTestDataProvider))]
        public void AlphaNumeric(BitmapSource image, string expected)
        {
            Assert.Equal(expected, Extract(image, new AlphanumericStringFilter(), Symbology.Text));
        }        
    }
}