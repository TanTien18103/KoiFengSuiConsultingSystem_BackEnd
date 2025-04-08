using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.CourseRepository
{
    public interface ICourseRepo
    {
        Task<Course> GetCourseById(string courseId);
        Task<List<Course>> GetCourses();
        Task<Course> CreateCourse(Course course);
        Task<Course> UpdateCourse(Course course);
        Task DeleteCourse(string courseId);
        Task<List<Course>> GetCoursesByMasterId(string masterId);
        Task<Course> GetCourseByChapterId(string chapterId);
        Task<List<Course>> GetCoursesByIds(List<string> courseIds);
        Task<bool> CheckCourseExists(string courseId);
        Task<string> GetEnrollCourseId(string courseId, string customerId);
    }
}
