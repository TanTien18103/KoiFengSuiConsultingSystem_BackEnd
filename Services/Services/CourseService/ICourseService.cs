using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CourseService
{
    public interface ICourseService
    {
        Task<ResultModel> GetCourseById(string courseId);
        Task<ResultModel> GetCourses();
        Task<ResultModel> CreateCourse(CourseRequest course);
        Task<ResultModel> UpdateCourse(string id, CourseRequest course);
        Task<ResultModel> DeleteCourse(string courseId);
        Task<ResultModel> GetCoursesByMaster();
    }
}
