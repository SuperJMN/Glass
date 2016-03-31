namespace Glass.Imaging.PostProcessing
{
    using System;

    [Flags]
    public enum FilterType
    {
        None = 0,
        AlphaOnly = 1,
        Alpha = 2,
        Digits = 4, 
        Number = 16,
        All = AlphaOnly | Alpha | Digits | Number,
    }
}