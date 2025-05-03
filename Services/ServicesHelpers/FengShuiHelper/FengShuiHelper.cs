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

    public static string GetCompatibilityMessage(double score, string userElement, int fishCount)
    {
        int modFishCount = fishCount % 10;
        if (modFishCount == 0) modFishCount = 10;

        string fishElement = fishCountToElement.ContainsKey(modFishCount) ? fishCountToElement[modFishCount] : null;
        string relationship = "";
        string detailedEffect = "";
        string suggestion = "";
        string tips = "";

        string scoreMessage = score switch
        {
            < 20 => ResponseMessageConstrantsPhongThuy.VeryLowScore,
            < 40 => ResponseMessageConstrantsPhongThuy.LowScore,
            < 60 => ResponseMessageConstrantsPhongThuy.MediumScore,
            < 80 => ResponseMessageConstrantsPhongThuy.HighScore,
            _ => ResponseMessageConstrantsPhongThuy.VeryHighScore
        };

        // Map user element to specific recommendation message
        var recommendationMessages = new Dictionary<string, string>
        {
            { "Kim", ResponseMessageConstrantsPhongThuy.KimRecommendation },
            { "Mộc", ResponseMessageConstrantsPhongThuy.MocRecommendation },
            { "Thủy", ResponseMessageConstrantsPhongThuy.ThuyRecommendation },
            { "Hỏa", ResponseMessageConstrantsPhongThuy.HoaRecommendation },
            { "Thổ", ResponseMessageConstrantsPhongThuy.ThoRecommendation }
        };

        if (fishElement != null)
        {
            if (fishElement == userElement)
            {
                relationship = string.Format(ResponseMessageConstrantsPhongThuy.SameElementRelationship, fishElement, userElement);
                detailedEffect = ResponseMessageConstrantsPhongThuy.SameElementEffect;
                suggestion = ResponseMessageConstrantsPhongThuy.SuggestionSame;
            }
            else if (elementGenerates[fishElement] == userElement)
            {
                relationship = string.Format(ResponseMessageConstrantsPhongThuy.GeneratingRelationship, fishElement, userElement);
                detailedEffect = ResponseMessageConstrantsPhongThuy.GeneratingEffect;
                suggestion = string.Format(ResponseMessageConstrantsPhongThuy.SuggestionGenerating, fishElement);
            }
            else if (elementDestroys[fishElement] == userElement)
            {
                relationship = string.Format(ResponseMessageConstrantsPhongThuy.OvercomingRelationship, fishElement, userElement);
                detailedEffect = ResponseMessageConstrantsPhongThuy.OvercomingEffect;
                suggestion = string.Format(ResponseMessageConstrantsPhongThuy.SuggestionOvercoming, userElement) + "\n" + recommendationMessages[userElement];
            }
            else
            {
                relationship = string.Format(ResponseMessageConstrantsPhongThuy.NoRelationship, fishElement, userElement);
                detailedEffect = ResponseMessageConstrantsPhongThuy.NoRelationshipEffect;
                suggestion = ResponseMessageConstrantsPhongThuy.SuggestionNoRelation + "\n" + recommendationMessages[userElement];
            }

            tips = ResponseMessageConstrantsPhongThuy.FengShuiTips;
        }
        else
        {
            relationship = ResponseMessageConstrantsPhongThuy.UnknownRelationship;
            suggestion = ResponseMessageConstrantsPhongThuy.SuggestionNoRelation + "\n" + recommendationMessages[userElement];
        }

        return $"{scoreMessage}\n\n{relationship}\n\n📊 **Tác động phong thủy:**\n{detailedEffect}\n\n🧭 **Gợi ý điều chỉnh:**\n{suggestion}\n\n{tips}";
    }
}
