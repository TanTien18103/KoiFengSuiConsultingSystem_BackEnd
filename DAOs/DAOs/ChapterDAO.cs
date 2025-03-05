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
        public static ChapterDAO instance = null;
        private readonly KoiFishPondContext _context;

        public ChapterDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static ChapterDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChapterDAO();
                }
                return instance;
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
            var chapter = await GetChapterByIdDao(chapterId);
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }

    }
}
