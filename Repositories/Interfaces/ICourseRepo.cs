using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ICourseRepo
    {
        Task<Course> GetCourseById(string courseId);
        Task<List<Course>> GetCourses();
        Task<Course> CreateCourse(Course course);
        Task<Course> UpdateCourse(Course course);
        Task DeleteCourse(string courseId);
    }
}
