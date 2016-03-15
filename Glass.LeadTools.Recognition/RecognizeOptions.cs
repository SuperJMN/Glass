namespace Glass.LeadTools.Recognition
{
    using System.Collections.Generic;
    using Leadtools.Barcode;

    public class RecognizeOptions
    {       
        public IEnumerable<BarcodeSymbology> BarcodeSymbologies { get; set; } =
           new[]
           {
                BarcodeSymbology.Code3Of9,
                BarcodeSymbology.Code93,
                BarcodeSymbology.QR,
                BarcodeSymbology.Datamatrix
           };

        public double SourceScaleForOcr { get; set; } = 0.3;
        public bool IsSourceScalingEnabledForOcr { get; set; } = true;
    }
}