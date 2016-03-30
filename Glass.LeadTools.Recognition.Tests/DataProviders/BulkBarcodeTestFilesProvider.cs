namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;

    public class BulkBarcodeTestFilesProvider : IEnumerable<object[]>
    {
        private IEnumerable<object[]> data {
            get
            {
                if (Directory.Exists("Full"))
                {
                    return from path in Directory.GetFiles("Full")
                        select new object[] {LoadImage(path) };
                }

                return new object[0][];
            }
        }
        
        private static BitmapSource LoadImage(string s)
        {
            return new BitmapImage(new Uri(s, UriKind.Relative));
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}