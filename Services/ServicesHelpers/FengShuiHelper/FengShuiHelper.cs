using BusinessObjects.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.FengShuiHelper;

public class FengShuiHelper
{
    public static readonly Dictionary<string, Dictionary<string, double>> ElementColorPoints = new()
{
{ "Kim", new() { { "Trắng", 10 },{ "Xám", 10 }, { "Ghi", 10} , { "Vàng", 5 }, {"Nâu", 5} , { "XanhDương", -5 }, { "XanhLá", -5}, { "Đen", -8 }, { "Đỏ", -10 }, { "Hồng", -10 }, { "Cam", -8 }, { "Tím", -10 } } },
{ "Mộc", new() {  { "XanhLá", 10 }, { "XanhDương", 5 }, { "Đen", 5 }, { "Vàng", -5 }, { "Nâu", -5 }, { "Trắng", -10 }, { "Xám", -10 }, { "Ghi", -10 } } },
{ "Thủy", new() { { "Đen", 10 }, { "XanhDương", 10 }, { "Trắng", 5 }, { "Xám", 5 }, { "Ghi", 5}, { "Đỏ", -5 }, { "Hồng", -5 }, { "Cam", -5 },{ "Tím", -5 }, { "Vàng", -10 }, { "Nâu", -10 } } },
{ "Hỏa", new() { { "Đỏ", 10 }, { "Hồng", 10 }, { "Cam", 10 }, { "Tím", 10 }, { "XanhLá", 5 }, { "Trắng", -5 }, { "Xám", -5 }, { "Ghi", -5 }, { "Vàng", -10 }, { "Đen", -10 }, { "XanhDương", -10 } } },
{ "Thổ", new() { { "Vàng", 10 }, { "Nâu", 10 }, { "Đỏ", 5 }, { "Hồng", 5 }, { "Cam", 5 }, { "Tím", 5 }, { "XanhDương", -5 }, { "Đen", -5 }, { "Trắng", -10 }, { "XanhLá", -10 } } }
};
    public static readonly Dictionary<string, Dictionary<string, double>> ShapePoints = new()
{
{ "Kim", new() { { "Tròn", 8 }, { "Vuông", -5 }, { "Chữ nhật", -3 } } },
{ "Mộc", new() { { "Chữ nhật", 8 }, { "Tròn", -5 }, { "Vuông", -3 } } },
{ "Thủy", new() { { "Tự do", 8 }, { "Vuông", -5 }, { "Tròn", -3 } } },
{ "Hỏa", new() { { "Tam giác", 8 }, { "Chữ nhật", -5 }, { "Tròn", -3 } } },
{ "Thổ", new() { { "Vuông", 8 }, { "Tròn", -5 }, { "Tam giác", -3 } } }
};

    public static readonly Dictionary<string, Dictionary<string, double>> DirectionPoints = new()
{
{ "Kim", new() { { "Tây", 10 }, { "Đông", -5 }, { "Bắc", -3 } } },
{ "Mộc", new() { { "Đông", 10 }, { "Tây", -5 }, { "Nam", -3 } } },
{ "Thủy", new() { { "Bắc", 10 }, { "Nam", -5 }, { "Tây", -3 } } },
{ "Hỏa", new() { { "Nam", 10 }, { "Bắc", -5 }, { "Đông", -3 } } },
{ "Thổ", new() { { "Trung tâm", 10 }, { "Tây", -5 }, { "Đông", -3 } } }
};

    public static readonly Dictionary<int, string> fishCountToElement = new()
{
    { 1, "Thủy" }, { 6, "Thủy" },
    { 2, "Hỏa" }, { 7, "Hỏa" },
    { 3, "Mộc" }, { 8, "Mộc" },
    { 4, "Kim" }, { 9, "Kim" },
    { 5, "Thổ" }, { 10, "Thổ" } 
};

    public static readonly Dictionary<string, string> elementGenerates = new()
{
    { "Kim", "Thủy" },
    { "Thủy", "Mộc" },
    { "Mộc", "Hỏa" },
    { "Hỏa", "Thổ" },
    { "Thổ", "Kim" }
};

    public static readonly Dictionary<string, string> elementDestroys = new()
{
    { "Kim", "Mộc" },
    { "Thủy", "Hỏa" },
    { "Mộc", "Thổ" },
    { "Hỏa", "Kim" },
    { "Thổ", "Thủy" }
};

    public static double CalculateFishCountBonus(int fishCount, string userElement)
    {
        int modFishCount = fishCount % 10;
        if (modFishCount == 0) modFishCount = 10;


        if (FengShuiHelper.fishCountToElement.ContainsKey(modFishCount))
        {
            string fishElement = FengShuiHelper.fishCountToElement[modFishCount];

            if (fishElement == userElement) return 8;
            if (FengShuiHelper.elementGenerates[fishElement] == userElement) return 10;
            if (FengShuiHelper.elementDestroys[fishElement] == userElement) return -10;
        }
        return 0;
    }

    public static string GetCompatibilityMessage(double score, string userElement, int fishCount, string direction, string shape, Dictionary<string, double> colorRatios)
    {
        // Normalize inputs
        direction = direction?.Trim();
        shape = shape?.Trim();
        colorRatios = colorRatios?.ToDictionary(kvp => kvp.Key.Trim(), kvp => kvp.Value) ?? new Dictionary<string, double>();

        // Calculate individual scores
        double fishCountScore = CalculateFishCountBonus(fishCount, userElement);
        double directionScore = DirectionPoints.ContainsKey(userElement) && !string.IsNullOrEmpty(direction) && DirectionPoints[userElement].ContainsKey(direction) ? DirectionPoints[userElement][direction] : 0;
        double shapeScore = ShapePoints.ContainsKey(userElement) && !string.IsNullOrEmpty(shape) && ShapePoints[userElement].ContainsKey(shape) ? ShapePoints[userElement][shape] : 0;

        // Calculate weighted color score
        double colorScore = 0;
        double totalRatio = colorRatios.Values.Sum();
        if (totalRatio > 0 && ElementColorPoints.ContainsKey(userElement))
        {
            foreach (var colorRatio in colorRatios)
            {
                string color = colorRatio.Key;
                double ratio = colorRatio.Value / totalRatio; // Normalize ratio
                if (ElementColorPoints[userElement].ContainsKey(color))
                {
                    colorScore += ElementColorPoints[userElement][color] * ratio;
                }
            }
        }

        // Total score (already passed as parameter, but we can recompute for consistency)
        double totalScore = fishCountScore + directionScore + shapeScore + colorScore;

        // Fish count element and relationship
        int modFishCount = fishCount % 10;
        if (modFishCount == 0) modFishCount = 10;
        string fishElement = fishCountToElement.ContainsKey(modFishCount) ? fishCountToElement[modFishCount] : null;

        // Initialize message components
        var relationshipMessages = new List<string>();
        var effectMessages = new List<string>();
        var suggestionMessages = new List<string>();
        var recommendationMessages = new Dictionary<string, string>
        {
            { "Kim", ResponseMessageConstrantsPhongThuy.KimRecommendation },
            { "Mộc", ResponseMessageConstrantsPhongThuy.MocRecommendation },
            { "Thủy", ResponseMessageConstrantsPhongThuy.ThuyRecommendation },
            { "Hỏa", ResponseMessageConstrantsPhongThuy.HoaRecommendation },
            { "Thổ", ResponseMessageConstrantsPhongThuy.ThoRecommendation }
        };

        var directionRecommendations = new Dictionary<string, string>
        {
            { "Kim", ResponseMessageConstrantsPhongThuy.KimDirectionRecommendation },
            { "Mộc", ResponseMessageConstrantsPhongThuy.MocDirectionRecommendation },
            { "Thủy", ResponseMessageConstrantsPhongThuy.ThuyDirectionRecommendation },
            { "Hỏa", ResponseMessageConstrantsPhongThuy.HoaDirectionRecommendation },
            { "Thổ", ResponseMessageConstrantsPhongThuy.ThoDirectionRecommendation }
        };

        var shapeRecommendations = new Dictionary<string, string>
        {
            { "Kim", ResponseMessageConstrantsPhongThuy.KimShapeRecommendation },
            { "Mộc", ResponseMessageConstrantsPhongThuy.MocShapeRecommendation },
            { "Thủy", ResponseMessageConstrantsPhongThuy.ThuyShapeRecommendation },
            { "Hỏa", ResponseMessageConstrantsPhongThuy.HoaShapeRecommendation },
            { "Thổ", ResponseMessageConstrantsPhongThuy.ThoShapeRecommendation }
        };

        var colorRecommendations = new Dictionary<string, string>
        {
            { "Kim", ResponseMessageConstrantsPhongThuy.KimColorRecommendation },
            { "Mộc", ResponseMessageConstrantsPhongThuy.MocColorRecommendation },
            { "Thủy", ResponseMessageConstrantsPhongThuy.ThuyColorRecommendation },
            { "Hỏa", ResponseMessageConstrantsPhongThuy.HoaColorRecommendation },
            { "Thổ", ResponseMessageConstrantsPhongThuy.ThoColorRecommendation }
        };

        // Score message
        string scoreMessage = totalScore switch
        {
            < 20 => ResponseMessageConstrantsPhongThuy.VeryLowScore,
            < 40 => ResponseMessageConstrantsPhongThuy.LowScore,
            < 60 => ResponseMessageConstrantsPhongThuy.MediumScore,
            < 80 => ResponseMessageConstrantsPhongThuy.HighScore,
            _ => ResponseMessageConstrantsPhongThuy.VeryHighScore
        };

        // Fish count compatibility
        if (fishElement != null)
        {
            if (fishElement == userElement)
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.SameElementRelationship, fishElement, userElement, "số lượng cá"));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.SameElementEffect);
                suggestionMessages.Add(ResponseMessageConstrantsPhongThuy.SuggestionSame);
            }
            else if (elementGenerates[fishElement] == userElement)
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.GeneratingRelationship, fishElement, userElement, "số lượng cá"));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.GeneratingEffect);
                suggestionMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.SuggestionGenerating, fishElement));
            }
            else if (elementDestroys[fishElement] == userElement)
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.OvercomingRelationship, fishElement, userElement, "số lượng cá"));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.OvercomingEffect);
                suggestionMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.SuggestionOvercoming, userElement));
            }
            else
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.NoRelationship, fishElement, userElement, "số lượng cá"));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.NoRelationshipEffect);
                suggestionMessages.Add(ResponseMessageConstrantsPhongThuy.SuggestionNoRelation);
            }
        }
        else
        {
            relationshipMessages.Add(ResponseMessageConstrantsPhongThuy.UnknownRelationship);
            suggestionMessages.Add(ResponseMessageConstrantsPhongThuy.SuggestionNoRelation);
        }
        suggestionMessages.Add(recommendationMessages[userElement]);

        // Direction compatibility
        if (!string.IsNullOrEmpty(direction) && DirectionPoints[userElement].ContainsKey(direction))
        {
            double dirScore = DirectionPoints[userElement][direction];
            if (dirScore > 0)
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.FavorableDirection, direction, userElement));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.FavorableDirectionEffect);
                suggestionMessages.Add(ResponseMessageConstrantsPhongThuy.SuggestionFavorableDirection);
            }
            else
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.UnfavorableDirection, direction, userElement));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.UnfavorableDirectionEffect);
                suggestionMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.SuggestionUnfavorableDirection, userElement));
            }
            suggestionMessages.Add(directionRecommendations[userElement]);
        }

        // Shape compatibility
        if (!string.IsNullOrEmpty(shape) && ShapePoints[userElement].ContainsKey(shape))
        {
            double shpScore = ShapePoints[userElement][shape];
            if (shpScore > 0)
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.FavorableShape, shape, userElement));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.FavorableShapeEffect);
                suggestionMessages.Add(ResponseMessageConstrantsPhongThuy.SuggestionFavorableShape);
            }
            else
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.UnfavorableShape, shape, userElement));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.UnfavorableShapeEffect);
                suggestionMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.SuggestionUnfavorableShape, userElement));
            }
            suggestionMessages.Add(shapeRecommendations[userElement]);
        }

        // Color compatibility
        if (colorRatios.Any() && totalRatio > 0)
        {
            string colorList = string.Join(", ", colorRatios.Select(kvp => $"{kvp.Key} ({kvp.Value * 100 / totalRatio:F1}%)"));
            if (colorScore > 0)
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.FavorableColor, colorList, userElement));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.FavorableColorEffect);
                suggestionMessages.Add(ResponseMessageConstrantsPhongThuy.SuggestionFavorableColor);
            }
            else
            {
                relationshipMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.UnfavorableColor, colorList, userElement));
                effectMessages.Add(ResponseMessageConstrantsPhongThuy.UnfavorableColorEffect);
                suggestionMessages.Add(string.Format(ResponseMessageConstrantsPhongThuy.SuggestionUnfavorableColor, userElement));
            }
            suggestionMessages.Add(colorRecommendations[userElement]);
        }

        // Combine messages
        string relationship = string.Join("\n", relationshipMessages);
        string detailedEffect = string.Join("\n\n", effectMessages);
        string suggestion = string.Join("\n", suggestionMessages);
        string tips = ResponseMessageConstrantsPhongThuy.FengShuiTips;

        return $"{scoreMessage}\n\n🔗 **Mối quan hệ phong thủy:**\n{relationship}\n\n📊 **Tác động phong thủy:**\n{detailedEffect}\n\n🧭 **Gợi ý điều chỉnh:**\n{suggestion}\n\n{tips}";
    }
}
