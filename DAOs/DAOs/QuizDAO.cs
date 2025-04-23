using BusinessObjects.Constants;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
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
                .Include(q => q.EnrollQuizzes)
                .Include(q => q.Questions)
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

        public async Task<Quiz> CreateQuizWithQuestionsAndAnswersDao(Quiz quiz)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == quiz.CourseId);
                    if (course == null)
                    {
                        throw new AppException(ResponseCodeConstants.NOT_FOUND, $"Không tìm thấy khóa học với ID: {quiz.CourseId}", StatusCodes.Status404NotFound);
                    }

                    var existingQuiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.CourseId == quiz.CourseId);
                    if (existingQuiz != null)
                    {
                        throw new AppException(ResponseCodeConstants.EXISTED, $"Khóa học với ID {quiz.CourseId} đã có quiz.", StatusCodes.Status400BadRequest);
                    }

                    var masterExists = await _context.Masters.AnyAsync(m => m.MasterId == quiz.CreateBy);
                    if (!masterExists)
                    {
                        throw new AppException(ResponseCodeConstants.NOT_FOUND, $"Không tìm thấy master với ID: {quiz.CreateBy}", StatusCodes.Status404NotFound);
                    }

                    await _context.Quizzes.AddAsync(quiz);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return quiz;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new AppException(ResponseCodeConstants.FAILED, $"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}", StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}
