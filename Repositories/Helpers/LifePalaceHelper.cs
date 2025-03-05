namespace Repositories.Helpers
{
    public static class LifePalaceHelper
    {
        private static readonly string[] MaleLifePalace = { "", "Khảm", "Ly", "Cấn", "Đoài", "Càn", "Khôn", "Tốn", "Chấn", "Khôn" };
        private static readonly string[] FemaleLifePalace = { "", "Cấn", "Càn", "Đoài", "Cấn", "Ly", "Khảm", "Khôn", "Chấn", "Tốn" };

        public static string CalculateLifePalace(int year, bool isMale)
        {
            // Tính tổng các chữ số trong năm
            int sum = year.ToString().Sum(c => c - '0');
            
            // Chia cho 9 và lấy số dư
            int remainder = sum % 9;
            if (remainder == 0) remainder = 9;
            
            return isMale ? MaleLifePalace[remainder] : FemaleLifePalace[remainder];
        }
    }
} 