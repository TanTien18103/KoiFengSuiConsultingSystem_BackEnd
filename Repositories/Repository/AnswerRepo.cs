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
    public class AnswerRepo : IAnswerRepo
    {
        private readonly AnswerDAO _answerDAO;

        public AnswerRepo(AnswerDAO answerDAO)
        {
            _answerDAO = answerDAO;
        }


        public async Task<Answer> GetAnswerById(string answerId)
        {
            return await _answerDAO.GetAnswerById(answerId);
        }
        public async Task<Answer> CreateAnswer(Answer answer)
        {
            return await _answerDAO.CreateAnswer(answer);
        }

        public async Task<Answer> UpdateAnswer(Answer answer)
        {
            return await _answerDAO.UpdateAnswer(answer);
        }

        public async Task DeleteAnswer(string answerId)
        {
            await _answerDAO.DeleteAnswer(answerId);
        }

        public async Task<List<Answer>> GetAnswersByQuestionId(string questionId)
        {
            return await _answerDAO.GetAnswersByQuestionId(questionId);
        }
    }
}
