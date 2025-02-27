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
        private readonly ChapterDAO _chapterDAO;

        public ChapterRepo(ChapterDAO chapterDAO)
        {
            _chapterDAO = chapterDAO;
        }

        public async Task<Chapter> GetChapterById(string chapterId)
        {
            return await _chapterDAO.GetChapterById(chapterId);
        }

        public async Task<Chapter> CreateChapter(Chapter chapter)
        {
            return await _chapterDAO.CreateChapter(chapter);
        }

        public async Task<Chapter> UpdateChapter(Chapter chapter)
        {
            return await _chapterDAO.UpdateChapter(chapter);
        }

        public async Task DeleteChapter(string chapterId)
        {
            await _chapterDAO.DeleteChapter(chapterId);
        }

        public async Task<List<Chapter>> GetChaptersByCourseId(string courseId)
        {
            return await _chapterDAO.GetChaptersByCourseId(courseId);
        }
    }
}
