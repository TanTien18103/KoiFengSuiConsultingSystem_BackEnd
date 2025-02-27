using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class EnrollChapterRepo : IEnrollChapterRepo
    {
        private readonly EnrollChapterDAO _enrollChapterDAO;

        public EnrollChapterRepo(EnrollChapterDAO enrollChapterDAO)
        {
            _enrollChapterDAO = enrollChapterDAO;
        }

        public async Task<EnrollChapter> GetEnrollChapterById(string enrollChapterId)
        {
            return await _enrollChapterDAO.GetEnrollChapterById(enrollChapterId);
        }

        public async Task<EnrollChapter> CreateEnrollChapter(EnrollChapter enrollChapter)
        {
            return await _enrollChapterDAO.CreateEnrollChapter(enrollChapter);
        }

        public async Task<EnrollChapter> UpdateEnrollChapter(EnrollChapter enrollChapter)
        {
            return await _enrollChapterDAO.UpdateEnrollChapter(enrollChapter);
        }

        public async Task DeleteEnrollChapter(string enrollChapterId)
        {
            await _enrollChapterDAO.DeleteEnrollChapter(enrollChapterId);
        }

        public async Task<List<EnrollChapter>> GetEnrollChapters()
        {
            return await _enrollChapterDAO.GetEnrollChapters();
        }
    }
}
