using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ChapterDAO
    {
        private static volatile ChapterDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private ChapterDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static ChapterDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ChapterDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Chapter> GetChapterByIdDao(string chapterId)
        {
            return await _context.Chapters.FindAsync(chapterId);
        }

        public async Task<List<Chapter>> GetChaptersByCourseIdDao(string courseId)
        {
            return _context.Chapters.Where(c => c.CourseId == courseId).ToList();
        }

        public async Task<Chapter> CreateChapterDao(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();
            return chapter;
        }

        public async Task<Chapter> UpdateChapterDao(Chapter chapter)
        {
            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();
            return chapter;
        }

        public async Task DeleteChapterDao(string chapterId)
        {
            var enrollChapters = _context.EnrollChapters.Where(e => e.ChapterId == chapterId).ToList();

            foreach (var enroll in enrollChapters)
            {
                var registerCourses = _context.RegisterCourses.Where(r => r.EnrollChapterId == enroll.EnrollChapterId);
                _context.RegisterCourses.RemoveRange(registerCourses);
            }

            await _context.SaveChangesAsync();

            _context.EnrollChapters.RemoveRange(enrollChapters);
            await _context.SaveChangesAsync(); 

            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter != null)
            {
                _context.Chapters.Remove(chapter);
                await _context.SaveChangesAsync();
            }
        }


    }
}
