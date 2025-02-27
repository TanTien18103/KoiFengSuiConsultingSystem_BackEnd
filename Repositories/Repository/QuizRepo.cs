using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class QuizRepo : IQuizRepo
    {
        private readonly QuizDAO _quizDAO;

        public QuizRepo(QuizDAO quizDAO)
        {
            _quizDAO = quizDAO;
        }

        public async Task<Quiz> GetQuizById(string quizId)
        {
            return await _quizDAO.GetQuizById(quizId);
        }
        public async Task<Quiz> CreateQuiz(Quiz quiz)
        {
            return await _quizDAO.CreateQuiz(quiz);
        }

        public async Task<Quiz> UpdateQuiz(Quiz quiz)
        {
            return await _quizDAO.UpdateQuiz(quiz);
        }

        public async Task DeleteQuiz(string quizId)
        {
            await _quizDAO.DeleteQuiz(quizId);
        }

        public async Task<List<Quiz>> GetQuizzes()
        {
            return await _quizDAO.GetQuizzes();
        }
    }
}
