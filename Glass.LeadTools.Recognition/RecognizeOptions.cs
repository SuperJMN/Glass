namespace Glass.LeadTools.Recognition
{
    using Leadtools.Barcode;

    public class RecognizeOptions
    {
        public RecognizeOptions()
        {
        }

        public double PreprocessScale { get; set; } = 0.3;
        public bool ScaleOcrInput { get; set; } = true;

        public BarcodeSymbology[] BarcodesTypes { get; } =
            {
                BarcodeSymbology.Code3Of9,
                BarcodeSymbology.Code93,
                BarcodeSymbology.QR,
                BarcodeSymbology.Datamatrix
            };
    }
}