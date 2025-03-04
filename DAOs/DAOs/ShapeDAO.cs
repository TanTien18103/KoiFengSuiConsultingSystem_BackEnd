using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ShapeDAO
    {
        private readonly KoiFishPondContext _context;

        public ShapeDAO()
        {
            _context = new KoiFishPondContext();
        }

        public async Task<Shape> GetShapeById(string shapeId)
        {
            return await _context.Shapes.FindAsync(shapeId);
        }

        public async Task<List<Shape>> GetShapes()
        {
            return await _context.Shapes.ToListAsync();
        }

        public async Task<Shape> CreateShape(Shape shape)
        {
            _context.Shapes.Add(shape);
            await _context.SaveChangesAsync();
            return shape;
        }

        public async Task<Shape> UpdateShape(Shape shape)
        {
            _context.Shapes.Update(shape);
            await _context.SaveChangesAsync();
            return shape;
        }

        public async Task DeleteShape(string shapeId)
        {
            var shape = await GetShapeById(shapeId);
            _context.Shapes.Remove(shape);
            await _context.SaveChangesAsync();
        }
    }
}
