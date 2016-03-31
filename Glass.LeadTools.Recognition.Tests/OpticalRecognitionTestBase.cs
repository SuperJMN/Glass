namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Barcodes.MessagingToolkit;
    using Imaging;
    using Imaging.Core;
    using Imaging.FullFx;
    using Imaging.PostProcessing;
    using Imaging.ZoneConfigurations;
    using Ocr.Tesseract;

    public abstract class OpticalRecognitionTestBase
    {
        private CompositeOpticalRecognizer opticalRecognizer;
        private readonly ILeadToolsLicenseApplier licenseApplier = new LeadToolsLicenseApplier();

        protected OpticalRecognitionTestBase()
        {
            ImagingContext.BitmapOperations = new BitmapOperations();
        }

        protected CompositeOpticalRecognizer GetSut()
        {
            var ocrEngines = new List<IImageToTextConverter> { new TesseractOcrOcrService(), new LeadToolsZoneBasedOcrService(licenseApplier) };
            var barcodeEngines = new List<IImageToTextConverter> { new MessagingToolkitZoneBasedBarcodeReader(), new LeadToolsZoneBasedBarcodeReader(licenseApplier) };
            return opticalRecognizer ?? (opticalRecognizer = new CompositeOpticalRecognizer(ocrEngines.Concat(barcodeEngines)));

        }

        protected static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }

        public string Extract(BitmapSource bitmap, ITextualDataFilter filter, Symbology symbology)
        {
            var sut = GetSut();
            var recognizedPage = sut.Recognize(bitmap, RecognitionConfiguration.FromSingleImage(bitmap, filter, symbology));

            var uniqueZone = recognizedPage.RecognizedZones.First();
            return uniqueZone.RecognizedText;
        }
    }
}