using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CourseDAO
    {
        private static volatile CourseDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private CourseDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static CourseDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CourseDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Course> GetCourseByMasterIdDao(string masterId)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.CreateBy == masterId);
        }

        public async Task<Course> GetCourseIdByChapterIdDao(string chapterId)
        {
            var chapter = await _context.Chapters.FindAsync(chapterId);
            var course = await _context.Courses.FindAsync(chapter.CourseId);
            return course;
        }


        public async Task<Course> GetCourseByIdDao(string courseId)
        {
            return await _context.Courses
                .Include(x => x.CreateByNavigation)
                .Include(x => x.Category)
                .Include(x => x.Chapters)
                .Include(x => x.RegisterCourses)
                .Include(x => x.Quizzes).ThenInclude(x => x.Questions)
                .FirstOrDefaultAsync(x => x.CourseId == courseId);
        }

        public async Task<List<Course>> GetCoursesDao()
        {
            return await _context.Courses
                .Include(x => x.CreateByNavigation)
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<Course> CreateCourseDao(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseDao(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task DeleteCourseDao(string courseId)
        {
            var course = await GetCourseByIdDao(courseId);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Course>> GetCoursesByMasterIdDao(string masterId)
        {
            return _context.Courses
                .Include(x => x.Category)
                .Where(c => c.CreateBy == masterId).ToList();
        }

        public async Task<List<Course>> GetCoursesByIdsDao(List<string> courseIds)
        {
            return await _context.Courses
                .Include(x => x.CreateByNavigation)
                .Include(x => x.Category)
                .Where(c => courseIds.Contains(c.CourseId))
                .ToListAsync();
        }
        public async Task<bool> CheckCourseExistsDao(string courseId)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == courseId);
        }

        public async Task<string> GetEnrollCourseIdDao(string courseId, string customerId)
        {
            var enrollCourse = await _context.RegisterCourses
                .FirstOrDefaultAsync(rc => rc.CourseId == courseId && rc.CustomerId == customerId);
            return enrollCourse?.EnrollCourseId;
        }

        public async Task<Course> UpdateCourseRatingDao(string courseId, decimal newRating = 0)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return null;

            var registerCourses = await _context.RegisterCourses
                .AsNoTracking()
                .Include(x => x.Course)
                .Where(rc => rc.CourseId == courseId && rc.Rating.HasValue)
                .ToListAsync();

            if (registerCourses.Any())
            {
                var validRatings = registerCourses.Where(rc => rc.Rating.HasValue).Select(rc => rc.Rating.Value).ToList();
                if (validRatings.Any())
                {
                    var averageRating = Math.Round(validRatings.Average(), 1);
                    course.Rating = averageRating;
                }
                else if (newRating > 0)
                {
                    course.Rating = newRating;
                }
                else 
                {
                    course.Rating = null;
                }
            }
            else if (newRating > 0)
            {
                course.Rating = newRating;
            }
            else 
            {
                course.Rating = null;
            }

            var registrationCount = await _context.RegisterCourses
                .Where(rc => rc.CourseId == courseId)
                .CountAsync();

            if (course.Rating.HasValue && course.Rating >= 4.5m && registrationCount > 5)
            {
                course.IsBestSeller = true;
            }
            else
            {
                course.IsBestSeller = false;
            }

            course.UpdateAt = DateTime.Now;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }
    }
}
