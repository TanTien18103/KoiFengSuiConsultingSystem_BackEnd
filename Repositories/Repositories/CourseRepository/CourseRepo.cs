using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.CourseRepository
{
    public class CourseRepo : ICourseRepo
    {
        public Task<Course> GetCourseById(string courseId)
        {
            return CourseDAO.Instance.GetCourseByIdDao(courseId);
        }
        public Task<List<Course>> GetCourses()
        {
            return CourseDAO.Instance.GetCoursesDao();
        }
        public Task<Course> CreateCourse(Course course)
        {
            return CourseDAO.Instance.CreateCourseDao(course);
        }
        public Task<Course> UpdateCourse(Course course)
        {
            return CourseDAO.Instance.UpdateCourseDao(course);
        }
        public Task DeleteCourse(string courseId)
        {
            return CourseDAO.Instance.DeleteCourseDao(courseId);
        }
        public Task<List<Course>> GetCoursesByMasterId(string masterId)
        {
            return CourseDAO.Instance.GetCoursesByMasterIdDao(masterId);
        }
    }
}
