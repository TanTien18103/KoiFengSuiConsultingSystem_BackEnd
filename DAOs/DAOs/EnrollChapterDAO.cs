using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class EnrollChapterDAO
    {
        private static volatile EnrollChapterDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private EnrollChapterDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static EnrollChapterDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EnrollChapterDAO();
                        }
                    }
                }
                return _instance;
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

        public async Task<EnrollChapter> GetEnrollChapterByChapterIdDao(string chapterId)
        {
            return await _context.EnrollChapters.FirstOrDefaultAsync(ec => ec.ChapterId == chapterId);
        }

        public async Task<int> CountTotalChaptersByResgisterCourseIdDao(string enrollCourseId)
        {
            return await _context.EnrollChapters
                .Where(ec => ec.EnrollCourseId == enrollCourseId)
                .CountAsync();
        }

        public async Task<int> CountCompletedChaptersByResgisterCourseIdDao(string enrollCourseId)
        {
            return await _context.EnrollChapters
                .Where(ec => ec.EnrollCourseId == enrollCourseId && ec.Status == EnrollChapterStatusEnums.Done.ToString())
                .CountAsync();
        }

    }
}
