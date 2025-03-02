namespace KoiFengSuiConsultingSystem.Respone
{
    public class FengShuiResult
    {
        public Dictionary<string, int> AdjustedColorRatios { get; set; }
        public int Quantity { get; set; }
        public string PondShape { get; set; }
        public string PondDirection { get; set; }
        public string Destiny { get; set; }
        public double CompatibilityScore { get; set; }
    }
}
