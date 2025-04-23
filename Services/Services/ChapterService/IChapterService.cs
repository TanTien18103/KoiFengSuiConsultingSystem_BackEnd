using Microsoft.AspNetCore.Http;
using Services.ApiModels;
using Services.ApiModels.Chapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ChapterService
{
    public interface IChapterService
    {
        Task<ResultModel> GetChapterById(string chapterId);
        Task<ResultModel> GetChaptersByCourseId(string courseId);
        Task<ResultModel> CreateChapter(ChapterRequest chapter);
        Task<ResultModel> UpdateChapter(string chapterId, ChapterUpdateRequest chapter);
        Task<ResultModel> DeleteChapter(string chapterId);
    }
}
