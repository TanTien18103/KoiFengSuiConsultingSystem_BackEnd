namespace BusinessObjects.Enums
{
    public class AmDuongNienHelper
    {
        private static readonly Dictionary<int, (string CanChi, NguHanh NguHanh)> NamSinh = new()
        {
            // 1924-1983
            {1924, ("Giáp Tý", NguHanh.Kim)},
            {1925, ("Ất Sửu", NguHanh.Kim)},
            {1926, ("Bính Dần", NguHanh.Hỏa)},
            {1927, ("Đinh Mão", NguHanh.Hỏa)},
            {1928, ("Mậu Thìn", NguHanh.Mộc)},
            {1929, ("Kỷ Tỵ", NguHanh.Mộc)},
            {1930, ("Canh Ngọ", NguHanh.Thổ)},
            {1931, ("Tân Mùi", NguHanh.Thổ)},
            {1932, ("Nhâm Thân", NguHanh.Kim)},
            {1933, ("Quý Dậu", NguHanh.Kim)},
            {1934, ("Giáp Tuất", NguHanh.Hỏa)},
            {1935, ("Ất Hợi", NguHanh.Hỏa)},
            {1936, ("Bính Tý", NguHanh.Thủy)},
            {1937, ("Đinh Sửu", NguHanh.Thủy)},
            {1938, ("Mậu Dần", NguHanh.Thổ)},
            {1939, ("Kỷ Mão", NguHanh.Thổ)},
            {1940, ("Canh Thìn", NguHanh.Kim)},
            {1941, ("Tân Tỵ", NguHanh.Kim)},
            {1942, ("Nhâm Ngọ", NguHanh.Mộc)},
            {1943, ("Quý Mùi", NguHanh.Mộc)},
            {1944, ("Giáp Thân", NguHanh.Thủy)},
            {1945, ("Ất Dậu", NguHanh.Thủy)},
            {1946, ("Bính Tuất", NguHanh.Thổ)},
            {1947, ("Đinh Hợi", NguHanh.Thổ)},
            {1948, ("Mậu Tý", NguHanh.Hỏa)},
            {1949, ("Kỷ Sửu", NguHanh.Hỏa)},
            {1950, ("Canh Dần", NguHanh.Mộc)},
            {1951, ("Tân Mão", NguHanh.Mộc)},
            {1952, ("Nhâm Thìn", NguHanh.Thủy)},
            {1953, ("Quý Tỵ", NguHanh.Thủy)},
            {1954, ("Giáp Ngọ", NguHanh.Kim)},
            {1955, ("Ất Mùi", NguHanh.Kim)},
            {1956, ("Bính Thân", NguHanh.Hỏa)},
            {1957, ("Đinh Dậu", NguHanh.Hỏa)},
            {1958, ("Mậu Tuất", NguHanh.Mộc)},
            {1959, ("Kỷ Hợi", NguHanh.Mộc)},
            {1960, ("Canh Tý", NguHanh.Thổ)},
            {1961, ("Tân Sửu", NguHanh.Thổ)},
            {1962, ("Nhâm Dần", NguHanh.Kim)},
            {1963, ("Quý Mão", NguHanh.Kim)},
            {1964, ("Giáp Thìn", NguHanh.Hỏa)},
            {1965, ("Ất Tỵ", NguHanh.Hỏa)},
            {1966, ("Bính Ngọ", NguHanh.Thủy)},
            {1967, ("Đinh Mùi", NguHanh.Thủy)},
            {1968, ("Mậu Thân", NguHanh.Thổ)},
            {1969, ("Kỷ Dậu", NguHanh.Thổ)},
            {1970, ("Canh Tuất", NguHanh.Kim)},
            {1971, ("Tân Hợi", NguHanh.Kim)},
            {1972, ("Nhâm Tý", NguHanh.Mộc)},
            {1973, ("Quý Sửu", NguHanh.Mộc)},
            {1974, ("Giáp Dần", NguHanh.Thủy)},
            {1975, ("Ất Mão", NguHanh.Thủy)},
            {1976, ("Bính Thìn", NguHanh.Thổ)},
            {1977, ("Đinh Tỵ", NguHanh.Thổ)},
            {1978, ("Mậu Ngọ", NguHanh.Hỏa)},
            {1979, ("Kỷ Mùi", NguHanh.Hỏa)},
            {1980, ("Canh Thân", NguHanh.Mộc)},
            {1981, ("Tân Dậu", NguHanh.Mộc)},
            {1982, ("Nhâm Tuất", NguHanh.Thủy)},
            {1983, ("Quý Hợi", NguHanh.Thủy)},
            {1984, ("Giáp Tý", NguHanh.Kim)},
            {1985, ("Ất Sửu", NguHanh.Kim)},
            {1986, ("Bính Dần", NguHanh.Hỏa)},
            {1987, ("Đinh Mão", NguHanh.Hỏa)},
            {1988, ("Mậu Thìn", NguHanh.Mộc)},
            {1989, ("Kỷ Tỵ", NguHanh.Mộc)},
            {1990, ("Canh Ngọ", NguHanh.Thổ)},
            {1991, ("Tân Mùi", NguHanh.Thổ)},
            {1992, ("Nhâm Thân", NguHanh.Kim)},
            {1993, ("Quý Dậu", NguHanh.Kim)},
            {1994, ("Giáp Tuất", NguHanh.Hỏa)},
            {1995, ("Ất Hợi", NguHanh.Hỏa)},
            {1996, ("Bính Tý", NguHanh.Thủy)},
            {1997, ("Đinh Sửu", NguHanh.Thủy)},
            {1998, ("Mậu Dần", NguHanh.Thổ)},
            {1999, ("Kỷ Mão", NguHanh.Thổ)},
            {2000, ("Canh Thìn", NguHanh.Kim)},
            {2001, ("Tân Tỵ", NguHanh.Kim)},
            {2002, ("Nhâm Ngọ", NguHanh.Mộc)},
            {2003, ("Quý Mùi", NguHanh.Mộc)},
            {2004, ("Giáp Thân", NguHanh.Thủy)},
            {2005, ("Ất Dậu", NguHanh.Thủy)},
            {2006, ("Bính Tuất", NguHanh.Thổ)},
            {2007, ("Đinh Hợi", NguHanh.Thổ)},
            {2008, ("Mậu Tý", NguHanh.Hỏa)},
            {2009, ("Kỷ Sửu", NguHanh.Hỏa)},
            {2010, ("Canh Dần", NguHanh.Mộc)},
            {2011, ("Tân Mão", NguHanh.Mộc)},
            {2012, ("Nhâm Thìn", NguHanh.Thủy)},
            {2013, ("Quý Tỵ", NguHanh.Thủy)},
            {2014, ("Giáp Ngọ", NguHanh.Kim)},
            {2015, ("Ất Mùi", NguHanh.Kim)},
            {2016, ("Bính Thân", NguHanh.Hỏa)},
            {2017, ("Đinh Dậu", NguHanh.Hỏa)},
            {2018, ("Mậu Tuất", NguHanh.Mộc)},
            {2019, ("Kỷ Hợi", NguHanh.Mộc)},
            {2020, ("Canh Tý", NguHanh.Thổ)},
            {2021, ("Tân Sửu", NguHanh.Thổ)},
            {2022, ("Nhâm Dần", NguHanh.Kim)},
            {2023, ("Quý Mão", NguHanh.Kim)},
            {2024, ("Giáp Thìn", NguHanh.Hỏa)},
            {2025, ("Ất Tỵ", NguHanh.Hỏa)},
            {2026, ("Bính Ngọ", NguHanh.Thủy)},
            {2027, ("Đinh Mùi", NguHanh.Thủy)},
            {2028, ("Mậu Thân", NguHanh.Thổ)},
            {2029, ("Kỷ Dậu", NguHanh.Thổ)},
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
                NguHanh.Mộc => "Mộc",
                NguHanh.Thủy => "Thủy",
                NguHanh.Hỏa => "Hỏa",
                NguHanh.Thổ => "Thổ",
                _ => throw new ArgumentException("Ngũ hành không hợp lệ")
            };
        }
    }
}