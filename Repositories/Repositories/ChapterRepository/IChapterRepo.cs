using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.ChapterRepository
{
    public interface IChapterRepo
    {
        Task<Chapter> GetChapterById(string chapterId);
        Task<List<Chapter>> GetChaptersByCourseId(string courseId);
        Task<Chapter> CreateChapter(Chapter chapter);
        Task<Chapter> UpdateChapter(Chapter chapter);
        Task DeleteChapter(string chapterId);
    }
}
