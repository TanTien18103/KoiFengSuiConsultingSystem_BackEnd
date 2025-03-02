namespace KoiFengSuiConsultingSystem.Request
{
    public class CompatibilityRequest
    {
        public string UserElement { get; set; }
        public Dictionary<string, double> ColorRatios { get; set; }
        public string PondShape { get; set; }
        public string PondDirection { get; set; }
        public int FishCount { get; set; }
    }
}
