namespace SIC.Services.OCR
{
    using System.Windows.Media;
    using Data.SIC.Models;

    public class RecognitionResult
    {
        public RecognitionResult(FieldConfiguration fieldConfiguration, string recognizedText, ImageSource image)
        {
            FieldConfiguration = fieldConfiguration;
            RecognizedText = recognizedText;
            Image = image;
        }

        public ImageSource Image { get; set; }

        public string RecognizedText { get; set; }
        public FieldConfiguration FieldConfiguration { get; set; }
    }
}