using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.ChapterRepository;
using Repositories.Repositories.CourseRepository;
using Services.ApiModels;
using Services.ApiModels.Chapter;
using Services.ApiModels.Course;
using Services.ServicesHelpers.BunnyCdnService;
using Services.ServicesHelpers.UploadService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.ChapterService
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepo _chapterRepo;
        private readonly IMapper _mapper;
        private readonly ICourseRepo _courseRepo;
        private readonly IUploadService _uploadService;
        private readonly IBunnyCdnService _bunnyCdnService;

        public ChapterService(IChapterRepo chapterRepo, IMapper mapper, ICourseRepo courseRepo, IUploadService uploadService, IBunnyCdnService bunnyCdnService)
        {
            _chapterRepo = chapterRepo;
            _mapper = mapper;
            _courseRepo = courseRepo;
            _uploadService = uploadService;
            _bunnyCdnService = bunnyCdnService;
        }


        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> GetChapterById(string chapterId)
        {
            var res = new ResultModel();

            try
            {
                var chapter = await _chapterRepo.GetChapterById(chapterId);
                if (chapter == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<ChapterRespone>(chapter);
                res.Message = ResponseMessageConstrantsChapter.CHAPTER_INFO_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetChaptersByCourseId(string courseId)
        {
            var res = new ResultModel();
            try
            {
                var chapters = await _chapterRepo.GetChaptersByCourseId(courseId);
                if (chapters == null || chapters.Count == 0)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<ChapterRespone>>(chapters);
                res.Message = ResponseMessageConstrantsChapter.CHAPTER_INFO_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
        public async Task<ResultModel> CreateChapter(ChapterRequest request)
        {
            var res = new ResultModel();

            try
            {
                if (request == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_INFO_INVALID;
                    return res;
                }

                if (string.IsNullOrEmpty(request.CourseId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsChapter.COURSE_ID_REQUIRED;
                    return res;
                }

                var course = await _courseRepo.GetCourseById(request.CourseId);
                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                var chapter = _mapper.Map<Chapter>(request);
                chapter.ChapterId = GenerateShortGuid();
                chapter.CreateDate = DateTime.UtcNow;
                chapter.Video = await _bunnyCdnService.UploadVideoAsync(request.Video);
                await _chapterRepo.CreateChapter(chapter);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = _mapper.Map<ChapterRespone>(chapter);
                res.Message = ResponseMessageConstrantsChapter.CHAPTER_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi tạo chương học: {ex.Message}";
                return res;
            }
        }
        public async Task<ResultModel> UpdateChapter(string chapterId, ChapterUpdateRequest chapter)
        {
            var res = new ResultModel();

            try
            {
                if (chapter == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_INFO_INVALID;
                    return res;
                }

                var chapterInfo = await _chapterRepo.GetChapterById(chapterId);
                if (chapterInfo == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_NOT_FOUND;
                    return res;
                }

                // Nếu người dùng truyền CourseId thì mới kiểm tra
                if (!string.IsNullOrWhiteSpace(chapter.CourseId))
                {
                    var course = await _courseRepo.GetCourseById(chapter.CourseId);
                    if (course == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                        return res;
                    }

                    chapterInfo.CourseId = chapter.CourseId;
                }

                if (!string.IsNullOrWhiteSpace(chapter.Title))
                {
                    chapterInfo.Title = chapter.Title;
                }

                if (!string.IsNullOrWhiteSpace(chapter.Description))
                {
                    chapterInfo.Description = chapter.Description;
                }

                if (chapter.Video != null)
                {
                    chapterInfo.Video = await _uploadService.UploadVideoAsync(chapter.Video);
                }

                await _chapterRepo.UpdateChapter(chapterInfo);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsChapter.CHAPTER_UPDATED_PROGRESS_SUCCESS;
                res.Data = _mapper.Map<ChapterRespone>(chapterInfo);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi cập nhật chương học: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> DeleteChapter(string chapterId)
        {
            var res = new ResultModel();

            try
            {
                var chapter = await _chapterRepo.GetChapterById(chapterId);
                if (chapter == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_NOT_FOUND;
                    return res;
                }

                await _chapterRepo.DeleteChapter(chapterId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsChapter.CHAPTER_DELETED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi xóa chương học: {innerException}";

                Console.WriteLine($"[ERROR] {ex}"); // Log full error
                return res;
            }


        }
    }
}
