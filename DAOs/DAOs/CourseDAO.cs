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
        private readonly KoiFishPondContext _context;


        public CourseDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Course> GetCourseById(string courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }

        public async Task<List<Course>> GetCourses()
        {
            return _context.Courses.ToList();
        }

        public async Task<Course> CreateCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourse(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task DeleteCourse(string courseId)
        {
            var course = await GetCourseById(courseId);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

    }
}
