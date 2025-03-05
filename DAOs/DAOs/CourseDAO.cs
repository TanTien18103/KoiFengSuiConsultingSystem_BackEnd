using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CourseDAO
    {
        public static CourseDAO instance = null;
        private readonly KoiFishPondContext _context;

        public CourseDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static CourseDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CourseDAO();
                }
                return instance;
            }
        }

        public async Task<Course> GetCourseByIdDao(string courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }

        public async Task<List<Course>> GetCoursesDao()
        {
            return _context.Courses.ToList();
        }

        public async Task<Course> CreateCourseDao(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseDao(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task DeleteCourseDao(string courseId)
        {
            var course = await GetCourseByIdDao(courseId);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

    }
}
