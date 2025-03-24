using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollQuizRepository
{
    public interface IEnrollQuizRepo
    {
        Task<EnrollQuiz> GetEnrollQuizById(string enrollQuizId);
        Task<List<EnrollQuiz>> GetEnrollQuizzes();
        Task<EnrollQuiz> CreateEnrollQuiz(EnrollQuiz enrollQuiz);
        Task<EnrollQuiz> UpdateEnrollQuiz(EnrollQuiz enrollQuiz);
        Task DeleteEnrollQuiz(string enrollQuizId);
        Task<EnrollQuiz> GetEnrollQuizByQuizIdAndParticipantId(string quizid, string customerid);
    }
}
