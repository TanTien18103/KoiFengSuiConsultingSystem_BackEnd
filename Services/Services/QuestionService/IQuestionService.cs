using Services.ApiModels;
using Services.ApiModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.QuestionService
{
    public interface IQuestionService
    {
        Task<ResultModel> GetQuestions();
        Task<ResultModel> GetQuestionById(string questionId);
        Task<ResultModel> GetQuestionsByQuizId(string quizId);
        Task<ResultModel> UpdateQuestion(string questionid, QuestionRequest questionRequest);
        Task<ResultModel> CreateQuestion(string quizId, QuestionRequest questionRequest);
        Task<ResultModel> DeleteQuestion(string questionId);
    }
}
