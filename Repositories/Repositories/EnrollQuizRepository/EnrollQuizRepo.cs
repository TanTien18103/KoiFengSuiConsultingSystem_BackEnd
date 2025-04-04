﻿using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollQuizRepository
{
    public class EnrollQuizRepo : IEnrollQuizRepo
    {
        public Task<EnrollQuiz> GetEnrollQuizById(string enrollQuizId)
        {
            return EnrollQuizDAO.Instance.GetEnrollQuizByIdDao(enrollQuizId);
        }
        public Task<List<EnrollQuiz>> GetEnrollQuizzes()
        {
            return EnrollQuizDAO.Instance.GetEnrollQuizzesDao();
        }
        public Task<EnrollQuiz> CreateEnrollQuiz(EnrollQuiz enrollQuiz)
        {
            return EnrollQuizDAO.Instance.CreateEnrollQuizDao(enrollQuiz);
        }
        public Task<EnrollQuiz> UpdateEnrollQuiz(EnrollQuiz enrollQuiz)
        {
            return EnrollQuizDAO.Instance.UpdateEnrollQuizDao(enrollQuiz);
        }
        public Task DeleteEnrollQuiz(string enrollQuizId)
        {
            return EnrollQuizDAO.Instance.DeleteEnrollQuizDao(enrollQuizId);
        }

        public Task<EnrollQuiz> GetEnrollQuizByQuizIdAndParticipantId(string quizid, string customerid)
        {
            return EnrollQuizDAO.Instance.GetEnrollQuizByQuizIdAndParticipantIdDao(quizid, customerid);
        }
    }
}
