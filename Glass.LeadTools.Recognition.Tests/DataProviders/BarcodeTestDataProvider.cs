namespace Glass.LeadTools.Recognition.Tests
{
    using DataProviders;

    internal class BarcodeTestDataProvider : TestFilesProvider
    {
        protected override char IgnoreChar
        {
            get
            {
                return '-';
            }
        }

        public BarcodeTestDataProvider() : base("Barcodes")
        {
        }
    }
}