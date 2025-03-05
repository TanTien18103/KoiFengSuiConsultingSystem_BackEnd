namespace Services.ApiModels.KoiPond
{
    public class KoiPondResponse
    {
        public string KoiPondId { get; set; }
        public string PondName { get; set; }
        public string ShapeId { get; set; }
        public string ShapeName { get; set; }
        public int? Size { get; set; }
        public string Direction { get; set; }
    }
}
