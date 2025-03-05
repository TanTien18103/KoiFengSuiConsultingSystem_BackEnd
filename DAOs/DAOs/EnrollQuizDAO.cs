using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class EnrollQuizDAO
    {
        public static EnrollQuizDAO instance = null;
        private readonly KoiFishPondContext _context;

        public EnrollQuizDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static EnrollQuizDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnrollQuizDAO();
                }
                return instance;
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
    }
}
