namespace Glass.Imaging.Recognition.Tests.DataProviders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using DotImaging;

    public abstract class TestCases : IEnumerable<TestCase>
    {
        protected TestCases(string path)
        {
            pathToFiles = path;
        }

        private readonly string pathToFiles;

        private IEnumerable<TestCase> data
        {
            get
            {
                return from path in Directory.GetFiles(pathToFiles)
                    let filename = Path.GetFileNameWithoutExtension(path)
                    let expected = filename
                    select new TestCase { Bitmap = LoadImage(path), Expected = expected };
            }
        }

        private static IImage LoadImage(string s)
        {
            return s.LoadColor();
        }

        public IEnumerator<TestCase> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}