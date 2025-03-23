using Services.ApiModels;
using Services.ApiModels.Answer;
using Services.ApiModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AnswerService
{
    public interface IAnswerService
    {
        Task<ResultModel> GetAnswerById(string answerId);
        Task<ResultModel> CreateAnswer(string questionid,AnswerRequest answer);
        Task<ResultModel> UpdateAnswer(string answerid, AnswerRequest answer);
        Task<ResultModel> DeleteAnswer(string answerId);
    }
}
