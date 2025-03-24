using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollAnswerRepository
{
    public interface IEnrollAnswerRepo
    {
        Task<EnrollAnswer> GetEnrollAnswerById(string enrollAnswerId);
        Task<List<EnrollAnswer>> GetEnrollAnswers();
        Task<EnrollAnswer> CreateEnrollAnswer(EnrollAnswer enrollAnswer);
        Task<EnrollAnswer> UpdateEnrollAnswer(EnrollAnswer enrollAnswer);
        Task DeleteEnrollAnswer(string enrollAnswerId);
        Task AddRangeEnrollAnswers(List<EnrollAnswer> enrollAnswers);
    }
}
