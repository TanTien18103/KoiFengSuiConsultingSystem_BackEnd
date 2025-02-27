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
    public class CourseRepo : ICourseRepo
    {
        private readonly CourseDAO _courseDAO;

        public CourseRepo(CourseDAO courseDAO)
        {
            _courseDAO = courseDAO;
        }

        public async Task<Course> GetCourseById(string courseId)
        {
            return await _courseDAO.GetCourseById(courseId);
        }

        public async Task<Course> CreateCourse(Course course)
        {
            return await _courseDAO.CreateCourse(course);
        }

        public async Task<Course> UpdateCourse(Course course)
        {
            return await _courseDAO.UpdateCourse(course);
        }

        public async Task DeleteCourse(string courseId)
        {
            await _courseDAO.DeleteCourse(courseId);
        }

        public async Task<List<Course>> GetCourses()
        {
            return await _courseDAO.GetCourses();
        }
    }
}
