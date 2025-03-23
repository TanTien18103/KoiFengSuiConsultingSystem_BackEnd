using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Quiz> GetQuizByIdDao(string quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Course)
                .Include(q => q.Courses)
                .Include(q => q.CreateByNavigation)
                .Include(q => q.EnrollQuizzes)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }


        public async Task<List<Quiz>> GetQuizzesDao()
        {
            return await _context.Quizzes
                .Include(q => q.Course)                   
                .Include(q => q.Courses)                  
                .Include(q => q.CreateByNavigation)       
                .Include(q => q.EnrollQuizzes)            
                .Include(q => q.Questions)                
                .ToListAsync();
        }

        public async Task<Quiz> GetQuizzesByCourseIdDao(string courseId)
        {
            return await _context.Quizzes
                .Include(q => q.Course)
                .Include(q => q.CreateByNavigation)
                .Where(q => q.CourseId == courseId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Quiz>> GetQuizzesByMasterIdDao(string masterId)
        {
            return await _context.Quizzes
                .Where(q => q.CreateBy == masterId)
                .ToListAsync();
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
            var coursesWithQuiz = await _context.Courses
                .Where(c => c.QuizId == quizid)
                .ToListAsync();

            if (coursesWithQuiz.Any())
            {
                foreach (var course in coursesWithQuiz)
                {
                    course.QuizId = null;
                }
            }

            var quiz = await GetQuizByIdDao(quizid);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
            }

            await _context.SaveChangesAsync();
        }
    }
}
