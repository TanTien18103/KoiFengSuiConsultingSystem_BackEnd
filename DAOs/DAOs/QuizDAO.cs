using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class QuizDAO
    {
        private readonly KoiFishPondContext _context;

        public QuizDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Quiz> GetQuizById(string quiz)
        {
            return await _context.Quizzes.FindAsync(quiz);
        }

        public async Task<List<Quiz>> GetQuizzes()
        {
            return _context.Quizzes.ToList();
        }

        public async Task<Quiz> CreateQuiz(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<Quiz> UpdateQuiz(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task DeleteQuiz(string quizid)
        {
            var quiz = await GetQuizById(quizid);
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }
}
