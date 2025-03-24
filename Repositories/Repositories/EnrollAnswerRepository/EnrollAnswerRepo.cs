using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollAnswerRepository
{
    public class EnrollAnswerRepo : IEnrollAnswerRepo
    {
        public Task<EnrollAnswer> GetEnrollAnswerById(string enrollAnswerId)
        {
            return EnrollAnswerDAO.Instance.GetEnrollAnswerByIdDao(enrollAnswerId);
        }
        public Task<List<EnrollAnswer>> GetEnrollAnswers()
        {
            return EnrollAnswerDAO.Instance.GetEnrollAnswersDao();
        }
        public Task<EnrollAnswer> CreateEnrollAnswer(EnrollAnswer enrollAnswer)
        {
            return EnrollAnswerDAO.Instance.CreateEnrollAnswerDao(enrollAnswer);
        }
        public Task<EnrollAnswer> UpdateEnrollAnswer(EnrollAnswer enrollAnswer)
        {
            return EnrollAnswerDAO.Instance.UpdateEnrollAnswerDao(enrollAnswer);
        }
        public Task DeleteEnrollAnswer(string enrollAnswerId)
        {
            return EnrollAnswerDAO.Instance.DeleteEnrollAnswerDao(enrollAnswerId);
        }

        public Task AddRangeEnrollAnswers(List<EnrollAnswer> enrollAnswers)
        {
            return EnrollAnswerDAO.Instance.AddRangeEnrollAnswersDao(enrollAnswers);
        }
    }
}
