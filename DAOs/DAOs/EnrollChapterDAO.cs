using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class EnrollChapterDAO
    {
        private readonly KoiFishPondContext _context;


        public EnrollChapterDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<EnrollChapter> GetEnrollChapterById(string enrollChapterId)
        {
            return await _context.EnrollChapters.FindAsync(enrollChapterId);
        }

        public async Task<List<EnrollChapter>> GetEnrollChapters()
        {
            return _context.EnrollChapters.ToList();
        }

        public async Task<EnrollChapter> CreateEnrollChapter(EnrollChapter enrollChapter)
        {
            _context.EnrollChapters.Add(enrollChapter);
            await _context.SaveChangesAsync();
            return enrollChapter;
        }

        public async Task<EnrollChapter> UpdateEnrollChapter(EnrollChapter enrollChapter)
        {
            _context.EnrollChapters.Update(enrollChapter);
            await _context.SaveChangesAsync();
            return enrollChapter;
        }

        public async Task DeleteEnrollChapter(string enrollChapterId)
        {
            var enrollChapter = await GetEnrollChapterById(enrollChapterId);
            _context.EnrollChapters.Remove(enrollChapter);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EnrollChapter>> GetEnrollChaptersByChapterId(string chapterId)
        {
            return _context.EnrollChapters.Where(ec => ec.ChapterId == chapterId).ToList();
        }

    }
}
