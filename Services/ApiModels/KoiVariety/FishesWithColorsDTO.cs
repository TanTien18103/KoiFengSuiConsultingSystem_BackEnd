using Services.ApiModels.Color;

namespace Services.ApiModels.KoiVariety
{
    public class FishesWithColorsDTO
    {
        public string Id { get; set; }
        public string VarietyName { get; set; }
        public string Description { get; set; }
        public List<ColorPercentageDto> Colors { get; set; } = new List<ColorPercentageDto>();
    }
}
