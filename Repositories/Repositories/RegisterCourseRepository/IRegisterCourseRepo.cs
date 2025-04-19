using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.RegisterCourseRepository
{
    public interface IRegisterCourseRepo
    {
        Task<RegisterCourse> GetRegisterCourseById(string registerCourseId);
        Task<List<RegisterCourse>> GetRegisterCourses();
        Task<RegisterCourse> CreateRegisterCourse(RegisterCourse registerCourse);
        Task<RegisterCourse> UpdateRegisterCourse(RegisterCourse registerCourse);
        Task DeleteRegisterCourse(string registerCourseId);
        Task<RegisterCourse> GetRegisterCourseByCourseIdAndCustomerId(string courseId, string customerid);
        Task<RegisterCourse> UpdateRegisterCourseRating(string enrollCourseId, decimal rating);
        Task<List<RegisterCourse>> GetRegisterCoursesByCourseId(string courseId);
        Task<RegisterCourse> GetRegisterCourseByEnrollQuizId(string enrollQuizId);
    }
}
