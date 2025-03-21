using Services.ApiModels;
using Services.ApiModels.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.QuizService
{
    public interface IQuizService
    {
        Task<ResultModel> GetQuizzes();
        Task<ResultModel> GetQuizById(string quizId);
        Task<ResultModel> GetQuizzesByMaster();
        Task<ResultModel> CreateQuiz(string courseId ,QuizRequest quiz);
        Task<ResultModel> UpdateQuiz(string courseId, QuizRequest quiz);
        Task<ResultModel> DeleteQuiz(string quizId);
    }
}
