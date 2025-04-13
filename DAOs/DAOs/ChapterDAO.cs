using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
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
            return await _context.Chapters.Where(c => c.CourseId == courseId).ToListAsync();
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
            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null)
            {
                return; 
            }
            
            var enrollChapters = await _context.EnrollChapters
                                               .Where(e => e.ChapterId == chapterId)
                                               .ToListAsync();
            if (enrollChapters.Any())
            {
                var enrollChapterIds = enrollChapters.Select(e => e.EnrollChapterId).ToList();
                _context.EnrollChapters.RemoveRange(enrollChapters);
            }
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }

       
    }
}
