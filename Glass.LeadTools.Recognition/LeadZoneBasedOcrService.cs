namespace LeadOcrRecognition
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using ImageProcessingCore.Core;
    using ImageProcessingCore.PostProcessing;
    using Leadtools.Barcode;
    using Leadtools.Codecs;
    using Leadtools.Forms;
    using Leadtools.Forms.Ocr;
    using Leadtools.Forms.Ocr.Advantage;

    public class LeadZoneBasedOcrService : IZoneBasedOcrService
    {
        private const string OcrEngineFolder = @"OCREngine";
        private readonly OcrEngine engine;
        private readonly BarcodeEngine barcodeEngineInstance = new BarcodeEngine();
        private readonly RasterCodecs codecs = new RasterCodecs();
        private readonly BarcodeSymbology[] barcodesTypes = { BarcodeSymbology.Code3Of9, BarcodeSymbology.Code93, BarcodeSymbology.QR, BarcodeSymbology.Datamatrix };

        public LeadZoneBasedOcrService()
        {
            engine = (OcrEngine)OcrEngineManager.CreateEngine(OcrEngineType.Advantage, false);
            engine.Startup(codecs, null, null, OcrEngineFolder);
            barcodeEngineInstance.Reader.ImageType = BarcodeImageType.ScannedDocument;
        }

        public RecognizedPage Recognize(ImageSource image, OcrConfiguration configuration)
        {
            var nonBarcode = RecognizeNonBarcodeZones(image, configuration.Zones.Where(z => z.SmartZoneType != SmartZoneType.Barcode).ToList());
            var barcode = RecognizeBarcodeZones(image, configuration.Zones.Where(s => s.SmartZoneType == SmartZoneType.Barcode).ToList());
            
            return new RecognizedPage(image, nonBarcode.Concat(barcode).ToList());
        }

        private IEnumerable<RecognizedZone> RecognizeBarcodeZones(ImageSource image, IEnumerable<ZoneConfiguration> zoneConfigurations)
        {
            var smartZoneConfigurations = zoneConfigurations.ToList();

            return smartZoneConfigurations.Select(zone => new RecognizedZone
            {
                ZoneConfig = zone,
                Image = GetThumbnail(image, zone.Bounds),
                RecognizedText = GetTextFromBarcode(image, zone.Bounds),                
            });
        }

        private string GetTextFromBarcode(ImageSource image, Rect bounds)
        {
            var spineImage = image.ToRasterImage();
            var leadRect = bounds.ToLeadRect();
            var logicalRectangle = new LogicalRectangle(leadRect);
            var barcodeData = barcodeEngineInstance.Reader.ReadBarcode(spineImage, logicalRectangle, barcodesTypes);
            var ocrResult = barcodeData == null ? string.Empty : barcodeData.Value;
            return ocrResult;
        }

        private IEnumerable<RecognizedZone> RecognizeNonBarcodeZones(ImageSource image, IEnumerable<ZoneConfiguration> zones)
        {
            if (!zones.Any())
            {
                yield break;
            }

            var ocrPage = CreateOcrPage(image, zones);

            ocrPage.Recognize(null);

            var id = 0;

            foreach (var zone in zones)
            {
                var imageSource = GetThumbnail(image, zone.Bounds);
                imageSource.Freeze();
                yield return new RecognizedZone
                {
                    RecognizedText = ocrPage.GetText(id),
                    Image = imageSource,
                    ZoneConfig = zone,
                };

                id++;
            }
        }

        private static ImageSource GetThumbnail(ImageSource image, Rect bounds)
        {
            var wb = (WriteableBitmap)image.Clone();
            var writeableBitmap = wb.Crop(bounds);
            writeableBitmap.Freeze();

            return writeableBitmap;
        }

        private IOcrPage CreateOcrPage(ImageSource image, IEnumerable<ZoneConfiguration> zones)
        {
            var ocrPage = engine.CreatePage(image.ToRasterImage(), OcrImageSharingMode.None);

            foreach (var field in zones)
            {
                var ocrZone = CreateOcrZone(field);

                ocrPage.Zones.Add(ocrZone);
            }

            return ocrPage;
        }

        private static OcrZone CreateOcrZone(ZoneConfiguration zone)
        {
            return new OcrZone
            {
                Bounds = new LogicalRectangle(zone.Bounds.ToLeadRect()),
                CharacterFilters = GetCharacterFilters(zone),
                Language = "ES",
                ZoneType =  zone.SmartZoneType == SmartZoneType.Barcode ? OcrZoneType.Barcode : OcrZoneType.Text,
            };
        }

        private static OcrZoneCharacterFilters GetCharacterFilters(ZoneConfiguration zone)
        {
            switch (zone.SmartZoneType)
            {
                case SmartZoneType.Barcode:
                    return OcrZoneCharacterFilters.All;
                case SmartZoneType.AlphaOnly:
                    return OcrZoneCharacterFilters.Alpha;
                case SmartZoneType.Alpha:
                    return OcrZoneCharacterFilters.All;
                case SmartZoneType.Numeric:
                    return OcrZoneCharacterFilters.Digit;
                default:
                    return OcrZoneCharacterFilters.All;
            }
        }
    }
}