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
        public  Task<Answer> GetAnswerById(string answerId)
        {
            return AnswerDAO.Instance.GetAnswerByIdDao(answerId);
        }
        public Task<Answer> CreateAnswer(Answer answer)
        {
            return AnswerDAO.Instance.CreateAnswerDao(answer);
        }

        public Task<Answer> UpdateAnswer(Answer answer)
        {
            return AnswerDAO.Instance.UpdateAnswerDao(answer);
        }

        public Task DeleteAnswer(string answerId)
        {
            return AnswerDAO.Instance.DeleteAnswerDao(answerId);
        }

        public Task<List<Answer>> GetAnswersByQuestionId(string questionId)
        {
            return AnswerDAO.Instance.GetAnswersByQuestionIdDao(questionId);
        }
    }
}
