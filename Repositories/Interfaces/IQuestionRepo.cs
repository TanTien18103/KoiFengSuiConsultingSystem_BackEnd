using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IQuestionRepo
    {
        Task<Question> GetQuestionById(string questionId);
        Task<List<Question>> GetQuestions();
        Task<Question> CreateQuestion(Question question);
        Task<Question> UpdateQuestion(Question question);
        Task DeleteQuestion(string questionId);
    }
}
