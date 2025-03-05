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
        public static EnrollChapterDAO instance = null;
        private readonly KoiFishPondContext _context;

        public EnrollChapterDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static EnrollChapterDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnrollChapterDAO();
                }
                return instance;
            }
        }

        public async Task<EnrollChapter> GetEnrollChapterByIdDao(string enrollChapterId)
        {
            return await _context.EnrollChapters.FindAsync(enrollChapterId);
        }

        public async Task<List<EnrollChapter>> GetEnrollChaptersDao()
        {
            return _context.EnrollChapters.ToList();
        }

        public async Task<EnrollChapter> CreateEnrollChapterDao(EnrollChapter enrollChapter)
        {
            _context.EnrollChapters.Add(enrollChapter);
            await _context.SaveChangesAsync();
            return enrollChapter;
        }

        public async Task<EnrollChapter> UpdateEnrollChapterDao(EnrollChapter enrollChapter)
        {
            _context.EnrollChapters.Update(enrollChapter);
            await _context.SaveChangesAsync();
            return enrollChapter;
        }

        public async Task DeleteEnrollChapterDao(string enrollChapterId)
        {
            var enrollChapter = await GetEnrollChapterByIdDao(enrollChapterId);
            _context.EnrollChapters.Remove(enrollChapter);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EnrollChapter>> GetEnrollChaptersByChapterIdDao(string chapterId)
        {
            return _context.EnrollChapters.Where(ec => ec.ChapterId == chapterId).ToList();
        }

    }
}
