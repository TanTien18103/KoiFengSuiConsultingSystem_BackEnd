using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class ColorRepo : IColorRepo
    {
        private readonly ColorDAO _colorDAO;

        public ColorRepo(ColorDAO colorDAO)
        {
            _colorDAO = colorDAO;
        }

        public async Task<Color> GetColorById(string colorId)
        {
            return await _colorDAO.GetColorById(colorId);
        }

        public async Task<Color> CreateColor(Color color)
        {
            return await _colorDAO.CreateColor(color);
        }

        public async Task<Color> UpdateColor(Color color)
        {
            return await _colorDAO.UpdateColor(color);
        }

        public async Task DeleteColor(string colorId)
        {
            await _colorDAO.DeleteColor(colorId);
        }

        public  async Task<List<Color>> GetColors()
        {
            return await _colorDAO.GetColors();
        }
    }
}
