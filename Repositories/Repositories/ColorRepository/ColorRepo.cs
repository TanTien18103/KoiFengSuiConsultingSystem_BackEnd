using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.ColorRepository
{
    public class ColorRepo : IColorRepo
    {
        public Task<Color> GetColorById(string colorId)
        {
            return ColorDAO.Instance.GetColorByIdDao(colorId);
        }
        public Task<List<Color>> GetColors()
        {
            return ColorDAO.Instance.GetColorsDao();
        }
        public Task<Color> CreateColor(Color color)
        {
            return ColorDAO.Instance.CreateColorDao(color);
        }
        public Task<Color> UpdateColor(Color color)
        {
            return ColorDAO.Instance.UpdateColorDao(color);
        }
        public Task DeleteColor(string colorId)
        {
            return ColorDAO.Instance.DeleteColorDao(colorId);
        }
    }
}
