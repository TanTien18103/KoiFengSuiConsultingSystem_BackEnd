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
    public class QuestionRepo : IQuestionRepo
    {
        private readonly QuestionDAO _questionDAO;

        public QuestionRepo(QuestionDAO questionDAO)
        {
            _questionDAO = questionDAO;
        }

        public async Task<Question> GetQuestionById(string questionId)
        {
            return await _questionDAO.GetQuestionById(questionId);
        }
        public async Task<Question> CreateQuestion(Question question)
        {
            return await _questionDAO.CreateQuestion(question);
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            return await _questionDAO.UpdateQuestion(question);
        }

        public async Task DeleteQuestion(string questionId)
        {
            await _questionDAO.DeleteQuestion(questionId);
        }

        public async Task<List<Question>> GetQuestions()
        {
            return await _questionDAO.GetQuestions();
        }
    }
}
