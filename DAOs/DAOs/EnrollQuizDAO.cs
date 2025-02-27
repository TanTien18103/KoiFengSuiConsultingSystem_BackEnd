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
        private readonly KoiFishPondContext _context;

        public EnrollQuizDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<EnrollQuiz> GetEnrollQuizById(string enrollQuizId)
        {
            return await _context.EnrollQuizzes.FindAsync(enrollQuizId);
        }

        public async Task<List<EnrollQuiz>> GetEnrollQuizzes()
        {
            return _context.EnrollQuizzes.ToList();
        }

        public async Task<EnrollQuiz> CreateEnrollQuiz(EnrollQuiz enrollQuiz)
        {
            _context.EnrollQuizzes.Add(enrollQuiz);
            await _context.SaveChangesAsync();
            return enrollQuiz;
        }

        public async Task<EnrollQuiz> UpdateEnrollQuiz(EnrollQuiz enrollQuiz)
        {
            _context.EnrollQuizzes.Update(enrollQuiz);
            await _context.SaveChangesAsync();
            return enrollQuiz;
        }

        public async Task DeleteEnrollQuiz(string enrollQuizId)
        {
            var enrollQuiz = await GetEnrollQuizById(enrollQuizId);
            _context.EnrollQuizzes.Remove(enrollQuiz);
            await _context.SaveChangesAsync();
        }
    }
}
