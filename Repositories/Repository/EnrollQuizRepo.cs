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
    public class EnrollQuizRepo : IEnrollQuizRepo
    {
        private readonly EnrollQuizDAO _enrollQuizDAO;

        public EnrollQuizRepo(EnrollQuizDAO enrollQuizDAO)
        {
            _enrollQuizDAO = enrollQuizDAO;
        }

        public async Task<EnrollQuiz> GetEnrollQuizById(string enrollQuizId)
        {
            return await _enrollQuizDAO.GetEnrollQuizById(enrollQuizId);
        }

        public async Task<EnrollQuiz> CreateEnrollQuiz(EnrollQuiz enrollQuiz)
        {
            return await _enrollQuizDAO.CreateEnrollQuiz(enrollQuiz);
        }

        public async Task<EnrollQuiz> UpdateEnrollQuiz(EnrollQuiz enrollQuiz)
        {
            return await _enrollQuizDAO.UpdateEnrollQuiz(enrollQuiz);
        }

        public async Task DeleteEnrollQuiz(string enrollQuizId)
        {
            await _enrollQuizDAO.DeleteEnrollQuiz(enrollQuizId);
        }

        public async Task<List<EnrollQuiz>> GetEnrollQuizzes()
        {
            return await _enrollQuizDAO.GetEnrollQuizzes();
        }
    }
}
