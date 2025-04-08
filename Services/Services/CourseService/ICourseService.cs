using BusinessObjects.Enums;
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
        Task<ResultModel> UpdateCourse(string id, CourseUpdateRequest course);
        Task<ResultModel> UpdateCourseStatus(string id, CourseStatusEnum status);
        Task<ResultModel> DeleteCourse(string courseId);
        Task<ResultModel> GetCoursesByMaster();
        Task<ResultModel> GetIsBestSellerCourses();
        Task<ResultModel> GetCoursesByCategoryId(string id);
        Task<ResultModel> SortByRating();
        Task<ResultModel> GetCourseByIdForMobile(string courseId);
        Task<ResultModel> GetPurchasedCourses();
    }
}
