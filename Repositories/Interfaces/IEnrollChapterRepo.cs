using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IEnrollChapterRepo
    {
        Task<EnrollChapter> GetEnrollChapterById(string enrollChapterId);
        Task<List<EnrollChapter>> GetEnrollChapters();
        Task<EnrollChapter> CreateEnrollChapter(EnrollChapter enrollChapter);
        Task<EnrollChapter> UpdateEnrollChapter(EnrollChapter enrollChapter);
        Task DeleteEnrollChapter(string enrollChapterId);
    }
}
