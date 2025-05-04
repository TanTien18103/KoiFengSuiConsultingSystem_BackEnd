using BusinessObjects.Constants;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.QuizRepository;
using Services.ApiModels;
using Services.ServicesHelpers.BunnyCdnService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.ServicesHelpers.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IBunnyCdnService _bunnyCdnService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMasterRepo _masterRepo;
        private readonly IQuizRepo _quizRepo;
        private readonly ICourseRepo _courseRepo;
        public UploadService(Cloudinary cloudinary, IBunnyCdnService bunnyCdnService, IHttpContextAccessor httpContextAccessor, IMasterRepo masterRepo, IQuizRepo quizRepo, ICourseRepo courseRepo)
        {
            _cloudinary = cloudinary;
            _bunnyCdnService = bunnyCdnService;
            _httpContextAccessor = httpContextAccessor;
            _masterRepo = masterRepo;
            _quizRepo = quizRepo;
            _courseRepo = courseRepo;
        }

        public string GetDocumentUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var url = _cloudinary.Api.Url.Secure(true).BuildUrl(publicId);
            return url;
        }

        public string GetPdfUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            return _cloudinary.Api.UrlImgUp
                .ResourceType("raw")
                .Secure(true)
                .BuildUrl(publicId);
        }

        public string GetImageUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var url = _cloudinary.Api.UrlImgUp
                .Secure(true)
                .Transform(new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto"))
                .BuildUrl(publicId);

            return url;
        }

        public string GetVideoUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var url = _cloudinary.Api.Url
                .Secure(true)
                .BuildUrl(publicId);

            return url;
        }

        public async Task<string> UploadDocumentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "documents",
                    PublicId = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{TimeHepler.SystemTimeNow.Ticks}"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception($"Lỗi khi upload tài liệu: {uploadResult.Error.Message}");

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload tài liệu: {ex.Message}");
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "images",
                    // Gợi ý: nếu crop không cần thiết thì có thể bỏ đi
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception($"Lỗi khi upload ảnh: {uploadResult.Error.Message}");

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload ảnh: {ex.Message}");
            }
        }

        public async Task<string> UploadPdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "pdfs",
                    Type = "upload",
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception($"Lỗi khi upload PDF: {uploadResult.Error.Message}");

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload PDF: {ex.Message}");
            }
        }

        //public async Task<string> UploadVideoAsync(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return null;

        //    try
        //    {
        //        using var stream = file.OpenReadStream();
        //        var uploadParams = new VideoUploadParams
        //        {
        //            File = new FileDescription(file.FileName, stream),
        //            Folder = "videos",
        //            PublicId = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.UtcNow.Ticks}"
        //        };

        //        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //        if (uploadResult.Error != null)
        //            throw new Exception($"Lỗi khi upload video: {uploadResult.Error.Message}");

        //        return uploadResult.SecureUrl.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Lỗi khi upload video: {ex.Message}");
        //    }
        //}
        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, "File is empty", StatusCodes.Status400BadRequest);

            try
            {
                return await _bunnyCdnService.UploadVideoAsync(file);
            }
            catch (Exception ex)
            {
                throw new AppException(ResponseCodeConstants.FAILED, $"Failed to upload video: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
        
        public async Task<Quiz> UploadExcelAsync(IFormFile file, string courseId)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".xls" && extension != ".xlsx")
                {
                    throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsForImport.FILE_INVALID, StatusCodes.Status400BadRequest);
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    throw new AppException(ResponseCodeConstants.UNAUTHORIZED, ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED, StatusCodes.Status401Unauthorized);
                }
                var masterId = await _masterRepo.GetMasterIdByAccountId(accountId);
                if (string.IsNullOrEmpty(masterId))
                {
                    throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsMaster.MASTER_NOT_FOUND, StatusCodes.Status404NotFound);
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (file == null || file.Length == 0)
                {
                    throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsForImport.NOT_FOUND, StatusCodes.Status404NotFound);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Quiz quiz = null;
                var existingQuizTitles = new HashSet<string>();

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252),
                        LeaveOpen = false
                    }))
                    {
                        Quiz currentQuiz = null;
                        Question currentQuestion = null;
                        bool isHeaderSkipped = false;

                        do
                        {
                            while (reader.Read())
                            {
                                if (!isHeaderSkipped)
                                {
                                    isHeaderSkipped = true;
                                    continue;
                                }

                                if (reader.GetValue(0) == null || string.IsNullOrWhiteSpace(reader.GetValue(0).ToString()))
                                {
                                    continue;
                                }

                                var recordType = reader.GetValue(0).ToString().ToLower();

                                switch (recordType)
                                {
                                    case "quiz":
                                        var quizTitle = reader.GetValue(1).ToString();

                                        if (existingQuizTitles.Contains(quizTitle))
                                        {
                                            throw new AppException(ResponseCodeConstants.EXISTED, ResponseMessageConstrantsForImport.EXISTED_QUIZ_TITLE, StatusCodes.Status400BadRequest);
                                        }

                                        var courseExists = await _courseRepo.CheckCourseExists(courseId);
                                        if (!courseExists)
                                        {
                                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsCourse.COURSE_NOT_FOUND, StatusCodes.Status404NotFound);
                                        }

                                        if (quiz != null)
                                        {
                                            throw new AppException(ResponseCodeConstants.BAD_REQUEST, "Chỉ được upload 1 Quiz mỗi lần.", StatusCodes.Status400BadRequest);
                                        }

                                        currentQuiz = new Quiz
                                        {
                                            QuizId = GenerateShortGuid(),
                                            Title = quizTitle,
                                            CourseId = courseId,
                                            CreateBy = masterId,
                                            Score = 100,
                                            CreateAt = TimeHepler.SystemTimeNow,
                                            UpdateAt = TimeHepler.SystemTimeNow,
                                            Questions = new List<Question>()
                                        };
                                        quiz = currentQuiz;
                                        existingQuizTitles.Add(quizTitle);
                                        break;

                                    case "question":
                                        if (currentQuiz == null)
                                        {
                                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsForImport.NO_QUIZ_FOR_QUES, StatusCodes.Status404NotFound);
                                        }

                                        var questionText = reader.GetValue(1).ToString();
                                        var questionType = reader.GetValue(2).ToString();

                                        currentQuestion = new Question
                                        {
                                            QuestionId = GenerateShortGuid(),
                                            QuizId = currentQuiz.QuizId,
                                            QuestionText = questionText,
                                            QuestionType = questionType,
                                            CreateAt = TimeHepler.SystemTimeNow,
                                            UpdateAt = TimeHepler.SystemTimeNow,
                                            Point = 0,
                                            Answers = new List<Answer>()
                                        };
                                        currentQuiz.Questions.Add(currentQuestion);
                                        break;

                                    case "answer":
                                        if (currentQuestion == null)
                                        {
                                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsForImport.NO_QUES_FOR_ANS, StatusCodes.Status404NotFound);
                                        }

                                        var optionText = reader.GetValue(1).ToString();
                                        var optionType = reader.GetValue(2).ToString();
                                        var isCorrect = bool.Parse(reader.GetValue(3).ToString());

                                        var answer = new Answer
                                        {
                                            AnswerId = GenerateShortGuid(),
                                            QuestionId = currentQuestion.QuestionId,
                                            OptionText = optionText,
                                            OptionType = optionType,
                                            CreateAt = TimeHepler.SystemTimeNow,
                                            UpdateDate = TimeHepler.SystemTimeNow,
                                            IsCorrect = isCorrect
                                        };
                                        currentQuestion.Answers.Add(answer);
                                        break;
                                }
                            }
                        } while (reader.NextResult());
                    }
                }

                if (quiz != null && quiz.Questions.Count > 0)
                {
                    var pointPerQuestion = Math.Round(100m / quiz.Questions.Count, 2);
                    foreach (var question in quiz.Questions)
                    {
                        question.Point = pointPerQuestion;
                    }
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (quiz != null)
                {
                    var createdQuiz = await _quizRepo.CreateQuizWithQuestionsAndAnswers(quiz);

                    var course = await _courseRepo.GetCourseById(courseId);
                    if (course != null)
                    {
                        course.QuizId = createdQuiz.QuizId;
                        await _courseRepo.UpdateCourse(course);
                    }

                    return createdQuiz;
                }
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsForImport.NO_DATA_TO_UPLOAD, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                throw new AppException(ResponseCodeConstants.FAILED, ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }
    }
}
