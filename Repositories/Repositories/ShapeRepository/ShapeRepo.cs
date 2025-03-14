using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.ShapeRepository
{
    public class ShapeRepo : IShapeRepo
    {
        public Task<Shape> GetShapeById(string shapeId)
        {
            return ShapeDAO.Instance.GetShapeByIdDao(shapeId);
        }
        public Task<List<Shape>> GetShapes()
        {
            return ShapeDAO.Instance.GetShapesDao();
        }
        public Task<Shape> CreateShape(Shape shape)
        {
            return ShapeDAO.Instance.CreateShapeDao(shape);
        }
        public Task<Shape> UpdateShape(Shape shape)
        {
            return ShapeDAO.Instance.UpdateShapeDao(shape);
        }
        public Task DeleteShape(string shapeId)
        {
            return ShapeDAO.Instance.DeleteShapeDao(shapeId);
        }
    }
}
