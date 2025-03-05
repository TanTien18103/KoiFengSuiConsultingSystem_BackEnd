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
        public static ShapeDAO instance = null;
        private readonly KoiFishPondContext _context;

        public ShapeDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static ShapeDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ShapeDAO();
                }
                return instance;
            }
        }

        public async Task<Shape> GetShapeByIdDao(string shapeId)
        {
            return await _context.Shapes.FindAsync(shapeId);
        }

        public async Task<List<Shape>> GetShapesDao()
        {
            return await _context.Shapes.ToListAsync();
        }

        public async Task<Shape> CreateShapeDao(Shape shape)
        {
            _context.Shapes.Add(shape);
            await _context.SaveChangesAsync();
            return shape;
        }

        public async Task<Shape> UpdateShapeDao(Shape shape)
        {
            _context.Shapes.Update(shape);
            await _context.SaveChangesAsync();
            return shape;
        }

        public async Task DeleteShapeDao(string shapeId)
        {
            var shape = await GetShapeByIdDao(shapeId);
            _context.Shapes.Remove(shape);
            await _context.SaveChangesAsync();
        }
    }
}
