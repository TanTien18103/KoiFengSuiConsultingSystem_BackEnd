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

        public async Task<List<Quiz>> CreateQuizzesWithQuestionsAndAnswersDao(List<Quiz> quizzes)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var quiz in quizzes)
                    {
                        var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == quiz.CourseId);
                        if (!courseExists)
                        {
                            throw new AppException(ResponseCodeConstants.NOT_FOUND, $"Không tìm thấy khóa học với ID: {quiz.CourseId}", StatusCodes.Status404NotFound);
                        }

                        var masterExists = await _context.Masters.AnyAsync(m => m.MasterId == quiz.CreateBy);
                        if (!masterExists)
                        {
                            throw new AppException( ResponseCodeConstants.NOT_FOUND, $"Không tìm thấy master với ID: {quiz.CreateBy}", StatusCodes.Status404NotFound);
                        }
                    }

                    // Thêm Quizzes và related entities
                    await _context.Quizzes.AddRangeAsync(quizzes);
                    
                    await _context.SaveChangesAsync();
                    
                    // Commit transaction
                    await transaction.CommitAsync();
                    
                    return quizzes;
                }
                catch (Exception ex)
                {
                    // Rollback nếu có lỗi
                    await transaction.RollbackAsync();
                    
                    throw new AppException(ResponseCodeConstants.FAILED, $"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}", StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}
