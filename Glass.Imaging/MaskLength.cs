namespace Glass.Imaging
{
    internal class MaskLength
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public MaskLength(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public MaskLength(int fixedLength)
        {
            FixedLength = fixedLength;
        }

        public int FixedLength { get; private set; }
    }
}