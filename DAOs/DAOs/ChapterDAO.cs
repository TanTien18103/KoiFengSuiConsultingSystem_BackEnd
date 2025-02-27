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
        private readonly KoiFishPondContext _context;

        public ChapterDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Chapter> GetChapterById(string chapterId)
        {
            return await _context.Chapters.FindAsync(chapterId);
        }

        public async Task<List<Chapter>> GetChaptersByCourseId(string courseId)
        {
            return _context.Chapters.Where(c => c.CourseId == courseId).ToList();
        }

        public async Task<Chapter> CreateChapter(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();
            return chapter;
        }

        public async Task<Chapter> UpdateChapter(Chapter chapter)
        {
            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();
            return chapter;
        }

        public async Task DeleteChapter(string chapterId)
        {
            var chapter = await GetChapterById(chapterId);
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }

    }
}
