﻿using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AnswerRepository
{
    public interface IAnswerRepo
    {
        Task<Answer> GetAnswerById(string answerId);
        Task<List<Answer>> GetAnswersByQuestionId(string questionId);
        Task<Answer> CreateAnswer(Answer answer);
        Task<Answer> UpdateAnswer(Answer answer);
        Task DeleteAnswer(string answerId);
        Task<List<Answer>> GetAnswersByQuestionIds(List<string> list);
    }
}
