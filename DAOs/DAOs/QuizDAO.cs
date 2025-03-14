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
        private static volatile QuizDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private QuizDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static QuizDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new QuizDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Quiz> GetQuizByIdDao(string quiz)
        {
            return await _context.Quizzes.FindAsync(quiz);
        }

        public async Task<List<Quiz>> GetQuizzesDao()
        {
            return _context.Quizzes.ToList();
        }

        public async Task<Quiz> CreateQuizDao(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<Quiz> UpdateQuizDao(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task DeleteQuizDao(string quizid)
        {
            var quiz = await GetQuizByIdDao(quizid);
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }
}
