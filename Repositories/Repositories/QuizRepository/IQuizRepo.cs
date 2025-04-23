using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.QuizRepository
{
    public interface IQuizRepo
    {
        Task<Quiz> GetQuizById(string quizId);
        Task<List<Quiz>> GetQuizzes();
        Task<List<Quiz>> GetQuizzesByMasterId(string masterId);
        Task<Quiz> CreateQuiz(Quiz quiz);
        Task<Quiz> UpdateQuiz(Quiz quiz);
        Task DeleteQuiz(string quizId); 
        Task<Quiz> GetQuizByCourseId(string courseId);
        Task<Quiz> CreateQuizWithQuestionsAndAnswers(Quiz quiz);
    }
}
