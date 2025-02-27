using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IColorRepo
    {
        Task<Color> GetColorById(string colorId);
        Task<List<Color>> GetColors();
        Task<Color> CreateColor(Color color);
        Task<Color> UpdateColor(Color color);
        Task DeleteColor(string colorId);
    }
}
