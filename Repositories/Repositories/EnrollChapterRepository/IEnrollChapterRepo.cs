using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollChapterRepository
{
    public interface IEnrollChapterRepo
    {
        Task<EnrollChapter> GetEnrollChapterById(string enrollChapterId);
        Task<List<EnrollChapter>> GetEnrollChapters();
        Task<EnrollChapter> CreateEnrollChapter(EnrollChapter enrollChapter);
        Task<EnrollChapter> UpdateEnrollChapter(EnrollChapter enrollChapter);
        Task DeleteEnrollChapter(string enrollChapterId);
        Task<EnrollChapter> GetEnrollChapterByChapterId(string chapterId);
        Task<int> CountTotalChaptersByRegisterCourseId(string courseId);
        Task<int> CountCompletedChaptersByRegisterCourseId(string courseId);
    }
}
