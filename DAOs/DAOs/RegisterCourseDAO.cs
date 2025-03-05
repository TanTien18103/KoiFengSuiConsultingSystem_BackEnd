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
        public static RegisterCourseDAO instance = null;
        private readonly KoiFishPondContext _context;

        public RegisterCourseDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static RegisterCourseDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RegisterCourseDAO();
                }
                return instance;
            }
        }

        public async Task<RegisterCourse> GetRegisterCourseByIdDao(string registerCourseId)
        {
            return await _context.RegisterCourses.FindAsync(registerCourseId);
        }

        public async Task<List<RegisterCourse>> GetRegisterCoursesDao()
        {
            return _context.RegisterCourses.ToList();
        }

        public async Task<RegisterCourse> CreateRegisterCourseDao(RegisterCourse registerCourse)
        {
            _context.RegisterCourses.Add(registerCourse);
            await _context.SaveChangesAsync();
            return registerCourse;
        }

        public async Task<RegisterCourse> UpdateRegisterCourseDao(RegisterCourse registerCourse)
        {
            _context.RegisterCourses.Update(registerCourse);
            await _context.SaveChangesAsync();
            return registerCourse;
        }

        public async Task DeleteRegisterCourseDao(string registerCourseId)
        {
            var registerCourse = await GetRegisterCourseByIdDao(registerCourseId);
            _context.RegisterCourses.Remove(registerCourse);
            await _context.SaveChangesAsync();
        }
    }
}
