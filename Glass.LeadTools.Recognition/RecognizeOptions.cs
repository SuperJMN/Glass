namespace Glass.LeadTools.Recognition
{
    using System.Collections;
    using System.Collections.Generic;
    using Leadtools.Barcode;

    public class RecognizeOptions
    {
        public double PreprocessScale { get; set; } = 0.3;
        public bool ScaleOcrInput { get; set; } = true;

        public IEnumerable<BarcodeSymbology> BarcodesTypes { get; set; } =
            new[]
            {
                BarcodeSymbology.Code3Of9,
                BarcodeSymbology.Code93,
                BarcodeSymbology.QR,
                BarcodeSymbology.Datamatrix
            };
    }
}