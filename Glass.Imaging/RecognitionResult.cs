namespace Glass.Imaging
{
    public class RecognitionResult
    {
        public RecognitionResult(string text, double confidence)
        {
            Text = text;
            Confidence = confidence;
        }

        public string Text { get; set; }
        public double Confidence { get; set; }
    }
}