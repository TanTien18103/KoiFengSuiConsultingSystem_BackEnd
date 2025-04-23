using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.QuizRepository
{
    public class QuizRepo : IQuizRepo
    {
        public Task<Quiz> GetQuizById(string quizId)
        {
            return QuizDAO.Instance.GetQuizByIdDao(quizId);
        }
        public Task<List<Quiz>> GetQuizzes()
        {
            return QuizDAO.Instance.GetQuizzesDao();
        }
        public Task<Quiz> CreateQuiz(Quiz quiz)
        {
            return QuizDAO.Instance.CreateQuizDao(quiz);
        }
        public Task<Quiz> UpdateQuiz(Quiz quiz)
        {
            return QuizDAO.Instance.UpdateQuizDao(quiz);
        }
        public Task DeleteQuiz(string quizId)
        {
            return QuizDAO.Instance.DeleteQuizDao(quizId);
        }
        public Task<List<Quiz>> GetQuizzesByMasterId(string masterId)
        {
            return QuizDAO.Instance.GetQuizzesByMasterIdDao(masterId);
        }

        public Task<Quiz> GetQuizByCourseId(string courseId)
        {
            return QuizDAO.Instance.GetQuizzesByCourseIdDao(courseId);
        }
        public Task<Quiz> CreateQuizWithQuestionsAndAnswers(Quiz quiz)
        {
            return QuizDAO.Instance.CreateQuizWithQuestionsAndAnswersDao(quiz);
        }
    }
}
