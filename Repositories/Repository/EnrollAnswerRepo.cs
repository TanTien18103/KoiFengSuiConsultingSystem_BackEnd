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
    public class EnrollAnswerRepo : IEnrollAnswerRepo
    {
        private readonly EnrollAnswerDAO _enrollAnswerDAO;

        public EnrollAnswerRepo(EnrollAnswerDAO enrollAnswerDAO)
        {
            _enrollAnswerDAO = enrollAnswerDAO;
        }

        public async Task<EnrollAnswer> GetEnrollAnswerById(string enrollAnswerId)
        {
            return await _enrollAnswerDAO.GetEnrollAnswerById(enrollAnswerId);
        }

        public async Task<EnrollAnswer> CreateEnrollAnswer(EnrollAnswer enrollAnswer)
        {
            return await _enrollAnswerDAO.CreateEnrollAnswer(enrollAnswer);
        }

        public async Task<EnrollAnswer> UpdateEnrollAnswer(EnrollAnswer enrollAnswer)
        {
            return await _enrollAnswerDAO.UpdateEnrollAnswer(enrollAnswer);
        }

        public async Task DeleteEnrollAnswer(string enrollAnswerId)
        {
            await _enrollAnswerDAO.DeleteEnrollAnswer(enrollAnswerId);
        }

        public async Task<List<EnrollAnswer>> GetEnrollAnswers()
        {
            return await _enrollAnswerDAO.GetEnrollAnswers();
        }
    }
}
