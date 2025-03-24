using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class EnrollQuizDAO
    {
        private static volatile EnrollQuizDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private EnrollQuizDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static EnrollQuizDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EnrollQuizDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<EnrollQuiz> GetEnrollQuizByIdDao(string enrollQuizId)
        {
            return await _context.EnrollQuizzes.FindAsync(enrollQuizId);
        }

        public async Task<List<EnrollQuiz>> GetEnrollQuizzesDao()
        {
            return _context.EnrollQuizzes.ToList();
        }

        public async Task<EnrollQuiz> CreateEnrollQuizDao(EnrollQuiz enrollQuiz)
        {
            _context.EnrollQuizzes.Add(enrollQuiz);
            await _context.SaveChangesAsync();
            return enrollQuiz;
        }

        public async Task<EnrollQuiz> UpdateEnrollQuizDao(EnrollQuiz enrollQuiz)
        {
            _context.EnrollQuizzes.Update(enrollQuiz);
            await _context.SaveChangesAsync();
            return enrollQuiz;
        }

        public async Task DeleteEnrollQuizDao(string enrollQuizId)
        {
            var enrollQuiz = await GetEnrollQuizByIdDao(enrollQuizId);
            _context.EnrollQuizzes.Remove(enrollQuiz);
            await _context.SaveChangesAsync();
        }

        public async Task<EnrollQuiz> GetEnrollQuizByQuizIdAndParticipantIdDao(string quizid, string customerid)
        {
            return await _context.EnrollQuizzes
                .FirstOrDefaultAsync(c => c.QuizId == quizid && c.ParticipantId == customerid);
        }
    }
}
