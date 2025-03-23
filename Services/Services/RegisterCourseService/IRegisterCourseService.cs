using Services.ApiModels;
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
        //Task<ResultModel> UpdateUserQuiz(string quizid);
    }
}
