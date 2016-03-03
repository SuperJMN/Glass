namespace SIC.Services.OCR
{
    using System.Collections.Generic;
    using Data.SIC.Models;
    using Models;
    using System.Windows.Media.Imaging;
    public interface IOpticalIdentificationService
    {
        IEnumerable<RecognitionResult> PerformOcr(BitmapSource image, IEnumerable<FieldConfiguration> fieldsFromConfig);
        IEnumerable<RecognitionResult> IdentifyBarcodes(BitmapSource image, IEnumerable<FieldConfiguration> barcodeFields);
    }
}
