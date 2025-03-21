using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.QuestionRepository
{
    public class QuestionRepo : IQuestionRepo
    {
        public Task<Question> GetQuestionById(string questionId)
        {
            return QuestionDAO.Instance.GetQuestionByIdDao(questionId);
        }
        public Task<List<Question>> GetQuestions()
        {
            return QuestionDAO.Instance.GetQuestionsDao();
        }
        public Task<Question> CreateQuestion(Question question)
        {
            return QuestionDAO.Instance.CreateQuestionDao(question);
        }
        public Task<Question> UpdateQuestion(Question question)
        {
            return QuestionDAO.Instance.UpdateQuestionDao(question);
        }
        public Task DeleteQuestion(string questionId)
        {
            return QuestionDAO.Instance.DeleteQuestionDao(questionId);
        }

        public Task<List<Question>> GetQuestionsByQuizId(string quizid)
        {
            return QuestionDAO.Instance.GetQuestionsByQuizId(quizid);
        }
    }
}
