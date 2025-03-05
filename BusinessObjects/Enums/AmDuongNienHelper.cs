namespace BusinessObjects.Enums
{
    public class AmDuongNienHelper
    {
        private static readonly Dictionary<int, (string CanChi, NguHanh NguHanh)> NamSinh = new()
        {
            // 1924-1983
            {1924, ("Giáp Tý", NguHanh.Kim)},
            {1925, ("Ất Sửu", NguHanh.Kim)},
            {1926, ("Bính Dần", NguHanh.Hoa)},
            {1927, ("Đinh Mão", NguHanh.Hoa)},
            {1928, ("Mậu Thìn", NguHanh.Moc)},
            {1929, ("Kỷ Tỵ", NguHanh.Moc)},
            {1930, ("Canh Ngọ", NguHanh.Tho)},
            {1931, ("Tân Mùi", NguHanh.Tho)},
            {1932, ("Nhâm Thân", NguHanh.Kim)},
            {1933, ("Quý Dậu", NguHanh.Kim)},
            {1934, ("Giáp Tuất", NguHanh.Hoa)},
            {1935, ("Ất Hợi", NguHanh.Hoa)},
            {1936, ("Bính Tý", NguHanh.Thuy)},
            {1937, ("Đinh Sửu", NguHanh.Thuy)},
            {1938, ("Mậu Dần", NguHanh.Tho)},
            {1939, ("Kỷ Mão", NguHanh.Tho)},
            {1940, ("Canh Thìn", NguHanh.Kim)},
            {1941, ("Tân Tỵ", NguHanh.Kim)},
            {1942, ("Nhâm Ngọ", NguHanh.Moc)},
            {1943, ("Quý Mùi", NguHanh.Moc)},
            {1944, ("Giáp Thân", NguHanh.Thuy)},
            {1945, ("Ất Dậu", NguHanh.Thuy)},
            {1946, ("Bính Tuất", NguHanh.Tho)},
            {1947, ("Đinh Hợi", NguHanh.Tho)},
            {1948, ("Mậu Tý", NguHanh.Hoa)},
            {1949, ("Kỷ Sửu", NguHanh.Hoa)},
            {1950, ("Canh Dần", NguHanh.Moc)},
            {1951, ("Tân Mão", NguHanh.Moc)},
            {1952, ("Nhâm Thìn", NguHanh.Thuy)},
            {1953, ("Quý Tỵ", NguHanh.Thuy)},
            {1954, ("Giáp Ngọ", NguHanh.Kim)},
            {1955, ("Ất Mùi", NguHanh.Kim)},
            {1956, ("Bính Thân", NguHanh.Hoa)},
            {1957, ("Đinh Dậu", NguHanh.Hoa)},
            {1958, ("Mậu Tuất", NguHanh.Moc)},
            {1959, ("Kỷ Hợi", NguHanh.Moc)},
            {1960, ("Canh Tý", NguHanh.Tho)},
            {1961, ("Tân Sửu", NguHanh.Tho)},
            {1962, ("Nhâm Dần", NguHanh.Kim)},
            {1963, ("Quý Mão", NguHanh.Kim)},
            {1964, ("Giáp Thìn", NguHanh.Hoa)},
            {1965, ("Ất Tỵ", NguHanh.Hoa)},
            {1966, ("Bính Ngọ", NguHanh.Thuy)},
            {1967, ("Đinh Mùi", NguHanh.Thuy)},
            {1968, ("Mậu Thân", NguHanh.Tho)},
            {1969, ("Kỷ Dậu", NguHanh.Tho)},
            {1970, ("Canh Tuất", NguHanh.Kim)},
            {1971, ("Tân Hợi", NguHanh.Kim)},
            {1972, ("Nhâm Tý", NguHanh.Moc)},
            {1973, ("Quý Sửu", NguHanh.Moc)},
            {1974, ("Giáp Dần", NguHanh.Thuy)},
            {1975, ("Ất Mão", NguHanh.Thuy)},
            {1976, ("Bính Thìn", NguHanh.Tho)},
            {1977, ("Đinh Tỵ", NguHanh.Tho)},
            {1978, ("Mậu Ngọ", NguHanh.Hoa)},
            {1979, ("Kỷ Mùi", NguHanh.Hoa)},
            {1980, ("Canh Thân", NguHanh.Moc)},
            {1981, ("Tân Dậu", NguHanh.Moc)},
            {1982, ("Nhâm Tuất", NguHanh.Thuy)},
            {1983, ("Quý Hợi", NguHanh.Thuy)},
            {1984, ("Giáp Tý", NguHanh.Kim)},
            {1985, ("Ất Sửu", NguHanh.Kim)},
            {1986, ("Bính Dần", NguHanh.Hoa)},
            {1987, ("Đinh Mão", NguHanh.Hoa)},
            {1988, ("Mậu Thìn", NguHanh.Moc)},
            {1989, ("Kỷ Tỵ", NguHanh.Moc)},
            {1990, ("Canh Ngọ", NguHanh.Tho)},
            {1991, ("Tân Mùi", NguHanh.Tho)},
            {1992, ("Nhâm Thân", NguHanh.Kim)},
            {1993, ("Quý Dậu", NguHanh.Kim)},
            {1994, ("Giáp Tuất", NguHanh.Hoa)},
            {1995, ("Ất Hợi", NguHanh.Hoa)},
            {1996, ("Bính Tý", NguHanh.Thuy)},
            {1997, ("Đinh Sửu", NguHanh.Thuy)},
            {1998, ("Mậu Dần", NguHanh.Tho)},
            {1999, ("Kỷ Mão", NguHanh.Tho)},
            {2000, ("Canh Thìn", NguHanh.Kim)},
            {2001, ("Tân Tỵ", NguHanh.Kim)},
            {2002, ("Nhâm Ngọ", NguHanh.Moc)},
            {2003, ("Quý Mùi", NguHanh.Moc)},
            {2004, ("Giáp Thân", NguHanh.Thuy)},
            {2005, ("Ất Dậu", NguHanh.Thuy)},
            {2006, ("Bính Tuất", NguHanh.Tho)},
            {2007, ("Đinh Hợi", NguHanh.Tho)},
            {2008, ("Mậu Tý", NguHanh.Hoa)},
            {2009, ("Kỷ Sửu", NguHanh.Hoa)},
            {2010, ("Canh Dần", NguHanh.Moc)},
            {2011, ("Tân Mão", NguHanh.Moc)},
            {2012, ("Nhâm Thìn", NguHanh.Thuy)},
            {2013, ("Quý Tỵ", NguHanh.Thuy)},
            {2014, ("Giáp Ngọ", NguHanh.Kim)},
            {2015, ("Ất Mùi", NguHanh.Kim)},
            {2016, ("Bính Thân", NguHanh.Hoa)},
            {2017, ("Đinh Dậu", NguHanh.Hoa)},
            {2018, ("Mậu Tuất", NguHanh.Moc)},
            {2019, ("Kỷ Hợi", NguHanh.Moc)},
            {2020, ("Canh Tý", NguHanh.Tho)},
            {2021, ("Tân Sửu", NguHanh.Tho)},
            {2022, ("Nhâm Dần", NguHanh.Kim)},
            {2023, ("Quý Mão", NguHanh.Kim)},
            {2024, ("Giáp Thìn", NguHanh.Hoa)},
            {2025, ("Ất Tỵ", NguHanh.Hoa)},
            {2026, ("Bính Ngọ", NguHanh.Thuy)},
            {2027, ("Đinh Mùi", NguHanh.Thuy)},
            {2028, ("Mậu Thân", NguHanh.Tho)},
            {2029, ("Kỷ Dậu", NguHanh.Tho)},
            {2030, ("Canh Tuất", NguHanh.Kim)}
        };

        public static (string CanChi, NguHanh NguHanh) GetNguHanh(int year)
        {
            if (NamSinh.TryGetValue(year, out var result))
            {
                return result;
            }
            throw new ArgumentException($"Không tìm thấy thông tin cho năm {year}");
        }

        public static string GetNguHanhName(NguHanh nguHanh)
        {
            return nguHanh switch
            {
                NguHanh.Kim => "Kim",
                NguHanh.Moc => "Mộc",
                NguHanh.Thuy => "Thủy",
                NguHanh.Hoa => "Hỏa",
                NguHanh.Tho => "Thổ",
                _ => throw new ArgumentException("Ngũ hành không hợp lệ")
            };
        }
    }
}