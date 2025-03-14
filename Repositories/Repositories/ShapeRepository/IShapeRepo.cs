using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.ShapeRepository
{
    public interface IShapeRepo
    {
        Task<Shape> GetShapeById(string shapeId);
        Task<List<Shape>> GetShapes();
        Task<Shape> CreateShape(Shape shape);
        Task<Shape> UpdateShape(Shape shape);
        Task DeleteShape(string shapeId);
    }
}
