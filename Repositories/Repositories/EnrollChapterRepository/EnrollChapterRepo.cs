using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollChapterRepository
{
    public class EnrollChapterRepo : IEnrollChapterRepo
    {
        public Task<EnrollChapter> GetEnrollChapterById(string enrollChapterId)
        {
            return EnrollChapterDAO.Instance.GetEnrollChapterByIdDao(enrollChapterId);
        }
        public Task<List<EnrollChapter>> GetEnrollChapters()
        {
            return EnrollChapterDAO.Instance.GetEnrollChaptersDao();
        }
        public Task<EnrollChapter> CreateEnrollChapter(EnrollChapter enrollChapter)
        {
            return EnrollChapterDAO.Instance.CreateEnrollChapterDao(enrollChapter);
        }
        public Task<EnrollChapter> UpdateEnrollChapter(EnrollChapter enrollChapter)
        {
            return EnrollChapterDAO.Instance.UpdateEnrollChapterDao(enrollChapter);
        }
        public Task DeleteEnrollChapter(string enrollChapterId)
        {
            return EnrollChapterDAO.Instance.DeleteEnrollChapterDao(enrollChapterId);
        }
    }
}
