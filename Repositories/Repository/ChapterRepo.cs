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
    public class ChapterRepo : IChapterRepo
    {
        public Task<Chapter> GetChapterById(string chapterId)
        {
            return ChapterDAO.Instance.GetChapterByIdDao(chapterId);
        }

        public Task<Chapter> CreateChapter(Chapter chapter)
        {
            return ChapterDAO.Instance.CreateChapterDao(chapter);
        }

        public Task<Chapter> UpdateChapter(Chapter chapter)
        {
            return ChapterDAO.Instance.UpdateChapterDao(chapter);
        }

        public Task DeleteChapter(string chapterId)
        {
            return ChapterDAO.Instance.DeleteChapterDao(chapterId);
        }

        public Task<List<Chapter>> GetChaptersByCourseId(string courseId)
        {
            return ChapterDAO.Instance.GetChaptersByCourseIdDao(courseId);
        }
    }
}
