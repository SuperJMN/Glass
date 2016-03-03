namespace Glass.Imaging
{
    using System.Text.RegularExpressions;

    public static class Regexes
    {
        public static readonly Regex AlphaOnly = new Regex(@"^[\p{L}_\s]+$", RegexOptions.Compiled);
        public static readonly Regex Alphanumeric = new Regex(@"^[\p{L}_\s0-9,.;:-]+$", RegexOptions.Compiled);
        public static readonly Regex NumbersOnly = new Regex("^[a-zA-Z0-9_]*$", RegexOptions.Compiled);
        public static readonly Regex MaskLength = new Regex(@"{(\d+),(\d+)}|{(\d+)}", RegexOptions.Compiled);        
    }
}