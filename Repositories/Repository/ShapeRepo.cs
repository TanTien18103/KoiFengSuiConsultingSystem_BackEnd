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
    public class ShapeRepo : IShapeRepo
    {
        private readonly ShapeDAO _shapeDAO;

        public ShapeRepo(ShapeDAO shapeDAO)
        {
            _shapeDAO = shapeDAO;
        }

        public async Task<Shape> GetShapeById(string shapeId)
        {
            return await _shapeDAO.GetShapeById(shapeId);
        }

        public async Task<Shape> CreateShape(Shape shape)
        {
            return await _shapeDAO.CreateShape(shape);
        }

        public async Task<Shape> UpdateShape(Shape shape)
        {
            return await _shapeDAO.UpdateShape(shape);
        }

        public async Task DeleteShape(string shapeId)
        {
            await _shapeDAO.DeleteShape(shapeId);
        }

        public async Task<List<Shape>> GetShapes()
        {
            return await _shapeDAO.GetShapes();
        }
    }
}
