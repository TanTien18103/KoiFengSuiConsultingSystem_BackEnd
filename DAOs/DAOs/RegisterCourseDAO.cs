using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class RegisterCourseDAO
    {
        private static volatile RegisterCourseDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private RegisterCourseDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static RegisterCourseDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RegisterCourseDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<RegisterCourse> GetRegisterCourseByIdDao(string registerCourseId)
        {
            return await _context.RegisterCourses.FindAsync(registerCourseId);
        }

        public async Task<List<RegisterCourse>> GetRegisterCoursesDao()
        {
            return await _context.RegisterCourses.ToListAsync();
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

        public async Task<RegisterCourse> GetRegisterCourseByCourseIdAndCustomerIdDao(string courseId, string customerid)
        {
            return await _context.RegisterCourses
                .FirstOrDefaultAsync(rc => rc.CourseId == courseId && rc.CustomerId == customerid);
        }
        
        public async Task<RegisterCourse> UpdateRegisterCourseRatingDao(string enrollCourseId, decimal rating)
        {
            var registerCourse = await _context.RegisterCourses.FindAsync(enrollCourseId);
            if (registerCourse == null)
                return null;
                
            registerCourse.Rating = rating;
            registerCourse.UpdateDate = TimeHepler.SystemTimeNow;
            
            _context.RegisterCourses.Update(registerCourse);
            await _context.SaveChangesAsync();
            
            return registerCourse;
        }
        
        public async Task<List<RegisterCourse>> GetRegisterCoursesByCourseIdDao(string courseId)
        {
            return await _context.RegisterCourses
                .Where(rc => rc.CourseId == courseId && rc.Rating.HasValue)
                .ToListAsync();
        }

        public async Task<RegisterCourse> GetRegisterCourseByEnrollQuizIdDao(string enrollQuizId)
        {
            return await _context.RegisterCourses
                .FirstOrDefaultAsync(rc => rc.EnrollQuizId == enrollQuizId);
        }
    }
}
