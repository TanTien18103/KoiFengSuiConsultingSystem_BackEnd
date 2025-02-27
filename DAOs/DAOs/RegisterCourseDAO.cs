using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class RegisterCourseDAO
    {
        private readonly KoiFishPondContext _context;

        public RegisterCourseDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<RegisterCourse> GetRegisterCourseById(string registerCourseId)
        {
            return await _context.RegisterCourses.FindAsync(registerCourseId);
        }

        public async Task<List<RegisterCourse>> GetRegisterCourses()
        {
            return _context.RegisterCourses.ToList();
        }

        public async Task<RegisterCourse> CreateRegisterCourse(RegisterCourse registerCourse)
        {
            _context.RegisterCourses.Add(registerCourse);
            await _context.SaveChangesAsync();
            return registerCourse;
        }

        public async Task<RegisterCourse> UpdateRegisterCourse(RegisterCourse registerCourse)
        {
            _context.RegisterCourses.Update(registerCourse);
            await _context.SaveChangesAsync();
            return registerCourse;
        }

        public async Task DeleteRegisterCourse(string registerCourseId)
        {
            var registerCourse = await GetRegisterCourseById(registerCourseId);
            _context.RegisterCourses.Remove(registerCourse);
            await _context.SaveChangesAsync();
        }
    }
}
