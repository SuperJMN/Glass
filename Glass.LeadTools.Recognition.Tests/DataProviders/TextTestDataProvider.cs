namespace Glass.LeadTools.Recognition.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;

    internal class TextTestDataProvider : IEnumerable<object[]>
    {
        private readonly string pathToFiles = "Text";

        private IEnumerable<object[]> data
        {
            get
            {
                return from path in Directory.GetFiles(pathToFiles)
                       let filename = Path.GetFileNameWithoutExtension(path)
                       let expected = filename
                       select new object[] { LoadImage(path), expected };
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