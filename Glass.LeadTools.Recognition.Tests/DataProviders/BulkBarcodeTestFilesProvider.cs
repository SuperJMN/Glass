namespace Glass.Imaging.Recognition.Tests.DataProviders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using DotImaging;

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
        
        private static IImage LoadImage(string s)
        {
            return s.LoadColor();
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