namespace Glass.LeadTools.Recognition.Tests
{
    using DataProviders;

    internal class NumericTestDataProvider : TestFilesProvider
    {
        protected override char IgnoreChar
        {
            get
            {
                return '!';
            }
        }

        public NumericTestDataProvider() : base("Barcodes")
        {
        }
    }
}