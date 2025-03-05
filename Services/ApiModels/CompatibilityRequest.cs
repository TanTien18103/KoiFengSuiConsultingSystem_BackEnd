namespace Services.ApiModels
{
    public class CompatibilityRequest
    {
        public Dictionary<string, double> ColorRatios { get; set; }
        public string PondShape { get; set; }
        public string PondDirection { get; set; }
        public int FishCount { get; set; }
    }
}
