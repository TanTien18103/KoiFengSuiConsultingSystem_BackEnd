using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.ApiModels.Certificate;
using Services.ApiModels.RegisterCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.RegisterCourseService
{
    public interface IRegisterCourseService
    {
        Task<ResultModel> UpdateUserCourseStatus(string chapterId);
        Task<ResultModel> UpdateUserQuiz(string quizid, RegisterQuizRequest registerQuizRequest);
        Task<ResultModel> GetEnrollChaptersByEnrollCourseId(string enrollCourseId);
        Task<ResultModel> GetEnrollCourseById(string id);

        //Certificate
        Task<ResultModel> GetCertificateById(string certificateId);
        Task<ResultModel> GetAllCertificates();
        Task<ResultModel> GetCertificatesByCourseId(string courseId);
        Task<ResultModel> GetCertificateByCustomerId();
        Task<ResultModel> CreateCertificate(CertificateRequest certificateRequest);
    }
}
