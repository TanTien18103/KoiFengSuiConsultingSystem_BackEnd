using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Tls;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.CertificateRepository;
using Repositories.Repositories.ChapterRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.EnrollAnswerRepository;
using Repositories.Repositories.EnrollCertRepository;
using Repositories.Repositories.EnrollChapterRepository;
using Repositories.Repositories.EnrollQuizRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.QuestionRepository;
using Repositories.Repositories.QuizRepository;
using Repositories.Repositories.RegisterCourseRepository;
using Services.ApiModels;
using Services.ApiModels.Certificate;
using Services.ApiModels.Contract;
using Services.ApiModels.EnrollCert;
using Services.ApiModels.EnrollChapter;
using Services.ApiModels.Quiz;
using Services.ApiModels.RegisterCourse;
using Services.ServicesHelpers.UploadService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using static Services.ApiModels.RegisterCourse.QuizResultResponse;
using static System.Net.Mime.MediaTypeNames;
using Account = CloudinaryDotNet.Account;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace Services.Services.RegisterCourseService
{
    public class RegisterCourseService : IRegisterCourseService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICourseRepo _courseRepo;
        private readonly IChapterRepo _chapterRepo;
        private readonly IQuizRepo _quizRepo;
        private readonly IQuestionRepo _questionRepo;
        private readonly IAnswerRepo _answerRepo;
        private readonly ICertificateRepo _certificateRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IRegisterCourseRepo _registerCourseRepo;
        private readonly IEnrollChapterRepo _enrollChapterRepo;
        private readonly IEnrollQuizRepo _enrollQuizRepo;
        private readonly IEnrollAnswerRepo _enrollAnswerRepo;
        private readonly IEnrollCertRepo _enrollCertRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUploadService _uploadService;
        private readonly ILogger<RegisterCourseService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMasterRepo _masterRepo;

        public RegisterCourseService(
            IOrderRepo orderRepo,
            IMapper mapper,
            ICourseRepo courseRepo,
            IChapterRepo chapterRepo,
            IQuizRepo quizRepo,
            IQuestionRepo questionRepo,
            IAnswerRepo answerRepo,
            ICertificateRepo certificateRepo,
            ICustomerRepo customerRepo,
            IRegisterCourseRepo registerCourseRepo,
            IEnrollChapterRepo enrollChapterRepo,
            IEnrollQuizRepo enrollQuizRepo,
            IEnrollAnswerRepo enrollAnswerRepo,
            IHttpContextAccessor contextAccessor,
            IUploadService uploadService,
            IEnrollCertRepo enrollCertRepo,
            IConfiguration configuration,
            ILogger<RegisterCourseService> logger,
            IWebHostEnvironment webHostEnvironment,
            IMasterRepo masterRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _courseRepo = courseRepo;
            _chapterRepo = chapterRepo;
            _quizRepo = quizRepo;
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _certificateRepo = certificateRepo;
            _customerRepo = customerRepo;
            _registerCourseRepo = registerCourseRepo;
            _enrollChapterRepo = enrollChapterRepo;
            _enrollQuizRepo = enrollQuizRepo;
            _enrollAnswerRepo = enrollAnswerRepo;
            _contextAccessor = contextAccessor;
            _uploadService = uploadService;
            _enrollCertRepo = enrollCertRepo;
            _configuration = configuration;
            _logger = logger;
            _environment = webHostEnvironment;
            _masterRepo = masterRepo;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> UpdateUserCourseStatus(string chapterId)
        {
            var res = new ResultModel();
            try
            {
                var identity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var customerid = await _customerRepo.GetCustomerIdByAccountId(accountId);
                if (customerid == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var course = await _courseRepo.GetCourseByChapterId(chapterId);
                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var RegisterCourse = await _registerCourseRepo.GetRegisterCourseByCourseIdAndCustomerId(course.CourseId, customerid);

                if (RegisterCourse == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsRegisterCourse.REGISTER_COURSE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var enrollChapter = await _enrollChapterRepo.GetEnrollChapterByChapterIdAndEnrollCourseId(chapterId, RegisterCourse.EnrollCourseId);
                if (enrollChapter == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (enrollChapter.Status.Equals(EnrollChapterStatusEnums.Done.ToString()))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsChapter.CHAPTER_ALREADY_COMPLETED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                enrollChapter.Status = EnrollChapterStatusEnums.Done.ToString();

                await _enrollChapterRepo.UpdateEnrollChapter(enrollChapter);

                var totalChapters = await _enrollChapterRepo.CountTotalChaptersByRegisterCourseId(RegisterCourse.EnrollCourseId);
                var completedChapters = await _enrollChapterRepo.CountCompletedChaptersByRegisterCourseId(RegisterCourse.EnrollCourseId);

                if (totalChapters > 0)
                {
                    RegisterCourse.Percentage = (completedChapters * 100) / totalChapters;
                }
                else
                {
                    RegisterCourse.Percentage = 0;
                }

                var UpdatedRegisterCourse = await _registerCourseRepo.UpdateRegisterCourse(RegisterCourse);

                if (UpdatedRegisterCourse == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsRegisterCourse.REGISTER_COURSE_UPDATE_FAILED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (UpdatedRegisterCourse.Percentage == 100)
                {
                    var quiz = await _quizRepo.GetQuizByCourseId(course.CourseId);
                    if (quiz == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        return res;
                    }

                    var enrollQuiz = new EnrollQuiz
                    {
                        EnrollQuizId = GenerateShortGuid(),
                        QuizId = quiz.QuizId,
                        ParticipantId = customerid,
                        Point = 0
                    };

                    var CreatedEnrollQuiz = await _enrollQuizRepo.CreateEnrollQuiz(enrollQuiz);

                    if (CreatedEnrollQuiz == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.Message = ResponseMessageConstrantQuiz.QUIZ_CREATE_FAILED;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        return res;
                    }

                    UpdatedRegisterCourse.EnrollQuizId = CreatedEnrollQuiz.EnrollQuizId;
                    await _registerCourseRepo.UpdateRegisterCourse(UpdatedRegisterCourse);

                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Message = ResponseMessageConstrantsCourse.PROCEED_TO_QUIZ_SUCCESS;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<RegisterCourseResponse>(RegisterCourse);
                res.Message = ResponseMessageConstrantsChapter.CHAPTER_UPDATED_PROGRESS_SUCCESS;
                return res;

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetEnrollCourseById(string id)
        {
            var res = new ResultModel();
            try
            {
                var enrollCourse = await _registerCourseRepo.GetRegisterCourseById(id);
                if (enrollCourse == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCourse.ENROLLEDCOURSE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<RegisterCourseResponse>(enrollCourse);
                res.Message = ResponseMessageConstrantsCourse.COURSE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> UpdateUserQuiz(string quizid, RegisterQuizRequest registerQuizRequest)
        {
            var res = new ResultModel();
            try
            {
                var identity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var customerid = await _customerRepo.GetCustomerIdByAccountId(accountId);
                if (customerid == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var enrollQuiz = await _enrollQuizRepo.GetEnrollQuizByQuizIdAndParticipantId(quizid, customerid);
                if (enrollQuiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (registerQuizRequest.AnswerIds == null || !registerQuizRequest.AnswerIds.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.INVALID_INPUT;
                    res.Message = ResponseMessageConstantsCommon.NO_DATA;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var questions = await _questionRepo.GetQuestionsByQuizId(enrollQuiz.QuizId);

                if (questions == null || !questions.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var validAnswers = await _answerRepo.GetAnswersByQuestionIds(questions.Select(q => q.QuestionId).ToList());

                if (validAnswers == null || !validAnswers.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var validAnswerIds = validAnswers.Select(a => a.AnswerId).ToHashSet();

                var userAnswerIds = registerQuizRequest.AnswerIds.Select(a => a.AnswerId).ToHashSet();

                var isValid = userAnswerIds.All(id => validAnswerIds.Contains(id));

                if (!isValid)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.INVALID_INPUT;
                    res.Message = ResponseMessageConstrantsAnswer.INVALID_ANSWER;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var correctAnswers = validAnswers.Where(a => a.IsCorrect == true).ToDictionary(a => a.AnswerId, a => a.QuestionId);

                var correctUserAnswers = userAnswerIds.Where(id => correctAnswers.ContainsKey(id)).ToList();

                List<EnrollAnswer> enrollAnswers = new List<EnrollAnswer>();

                decimal totalScore = 0;
                int correctCount = 0;

                foreach (var answerId in userAnswerIds)
                {
                    bool isCorrect = correctAnswers.ContainsKey(answerId);
                    string questionId = correctAnswers.ContainsKey(answerId) ? correctAnswers[answerId] : null;

                    enrollAnswers.Add(new EnrollAnswer
                    {
                        EnrollAnswerId = GenerateShortGuid(),
                        EnrollQuizId = enrollQuiz.EnrollQuizId,
                        AnswerId = answerId,
                        Correct = isCorrect
                    });

                    if (isCorrect && questionId != null)
                    {
                        var question = questions.FirstOrDefault(q => q.QuestionId == questionId);
                        if (question != null)
                        {
                            totalScore += question.Point ?? 0;
                            correctCount++;
                        }
                    }
                }

                await _enrollAnswerRepo.AddRangeEnrollAnswers(enrollAnswers);

                enrollQuiz.Point = totalScore;

                await _enrollQuizRepo.UpdateEnrollQuiz(enrollQuiz);

                var registerCourse = await _registerCourseRepo.GetRegisterCourseByEnrollQuizId(enrollQuiz.EnrollQuizId);
                var course = await _courseRepo.GetCourseById(registerCourse.CourseId);
                var masterid = course.CreateBy;
                if (masterid == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsMaster.MASTER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }
                if (registerCourse != null)
                {
                    var quiz = await _quizRepo.GetQuizById(enrollQuiz.QuizId);
                    if (quiz == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                        return res;
                    }

                    decimal quizScore = quiz.Score.GetValueOrDefault();
                    decimal passThreshold = quizScore * 0.8m;

                    // ⛔ Nếu học viên đã đạt >=80% trước đó, không cập nhật gì nữa
                    if (enrollQuiz.Point >= passThreshold)
                    {
                        res.IsSuccess = true;
                        res.StatusCode = StatusCodes.Status200OK;
                        res.ResponseCode = ResponseCodeConstants.SUCCESS;
                        res.Message = ResponseMessageConstrantQuiz.QUIZ_MEET_REQUIREMENT;
                        res.Data = new QuizResultResponse
                        {
                            QuizId = quizid,
                            ParticipantId = customerid,
                            TotalScore = (decimal)enrollQuiz.Point, // sử dụng điểm cũ
                            TotalQuestions = questions.Count,
                            CorrectAnswers = correctCount,
                        };
                        return res;
                    }

                    // Nếu điểm mới đạt ≥80%, tiến hành cấp chứng chỉ
                    if (totalScore >= passThreshold)
                    {
                        var customerInfo = await _customerRepo.GetCustomerById(customerid);
                        var masterInfo = await _masterRepo.GetByMasterId(masterid);

                        string certificateImageUrl = await GenerateCertificateImageAndUploadToCloudinary(
                            customerInfo.Account.FullName,
                            masterInfo.MasterName,
                            course.CourseName,
                            DateOnly.FromDateTime(TimeHepler.SystemTimeNow)
                        );

                        var certificate = new BusinessObjects.Models.Certificate
                        {
                            CertificateId = GenerateShortGuid(),
                            IssueDate = DateOnly.FromDateTime(TimeHepler.SystemTimeNow),
                            Description = $"Chứng nhận hoàn thành khóa học {course.CourseName}",
                            CertificateImage = certificateImageUrl,
                            CreateDate = TimeHepler.SystemTimeNow
                        };

                        await _certificateRepo.CreateCertificate(certificate);

                        var enrollCert = new EnrollCert
                        {
                            EnrollCertId = GenerateShortGuid(),
                            CustomerId = customerid,
                            CertificateId = certificate.CertificateId,
                            FinishDate = DateOnly.FromDateTime(TimeHepler.SystemTimeNow),
                            CreateDate = TimeHepler.SystemTimeNow,
                        };

                        await _enrollCertRepo.CreateEnrollCert(enrollCert);

                        registerCourse.EnrollCertId = enrollCert.EnrollCertId;
                        registerCourse.Status = RegisterCourseStatusEnums.Completed.ToString();
                        registerCourse.UpdateDate = TimeHepler.SystemTimeNow;
                        await _registerCourseRepo.UpdateRegisterCourse(registerCourse);

                        res.IsSuccess = true;
                        res.StatusCode = StatusCodes.Status200OK;
                        res.ResponseCode = ResponseCodeConstants.SUCCESS;
                        res.Message = ResponseMessageConstrantQuiz.CERTIFICATE_ALREADY_GRANTED;
                        res.Data = new QuizResultResponse
                        {
                            QuizId = quizid,
                            ParticipantId = customerid,
                            TotalScore = totalScore,
                            TotalQuestions = questions.Count,
                            CorrectAnswers = correctCount,
                        };
                        return res;
                    }
                }
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_SUBMITED_SUCCESS;
                res.Data = new QuizResultResponse
                {
                    QuizId = quizid,
                    ParticipantId = customerid,
                    TotalScore = totalScore,
                    TotalQuestions = questions.Count,
                    CorrectAnswers = correctCount,
                };
                return res;

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetEnrollChaptersByEnrollCourseId(string enrollCourseId)
        {
            var res = new ResultModel();
            try
            {
                var enrollChapters = await _enrollChapterRepo.GetEnrollChaptersByEnrollCourseId(enrollCourseId);
                if (enrollChapters == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsRegisterCourse.ENROLL_CHAPTERS_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsRegisterCourse.ENROLL_CHAPTERS_FOUND;
                res.Data = _mapper.Map<List<EnrollChapterResponse>>(enrollChapters);
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

        public async Task<ResultModel> GetCertificateById(string certificateId)
        {
            var res = new ResultModel();
            try
            {
                var certificate = await _certificateRepo.GetCertificateById(certificateId);
                if (certificate == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<CertificateResponse>(certificate);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetAllCertificates()
        {
            var res = new ResultModel();
            try
            {
                var certificates = await _certificateRepo.GetAllCertificates();
                if (certificates == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<CertificateResponse>>(certificates);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetCertificatesByCourseId(string courseId)
        {
            var res = new ResultModel();
            try
            {
                var certificates = await _certificateRepo.GetCertificatesByCourseId(courseId);
                if (certificates == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<CertificateResponse>>(certificates);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> CreateCertificate(ApiModels.Certificate.CertificateRequest certificateRequest)
        {
            var res = new ResultModel();
            try
            {
                var certificate = _mapper.Map<BusinessObjects.Models.Certificate>(certificateRequest);
                var uploadResult = await _uploadService.UploadImageAsync(certificateRequest.CertificateImage);
                if (uploadResult == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_IMAGE_UPLOAD_FAILED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                certificate.CertificateId = GenerateShortGuid();
                certificate.CreateDate = TimeHepler.SystemTimeNow;
                certificate.UpdateDate = TimeHepler.SystemTimeNow;
                certificate.CertificateImage = uploadResult;

                var createdCertificate = await _certificateRepo.CreateCertificate(certificate);

                if (createdCertificate == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_CREATE_FAILED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var courses = await _courseRepo.GetCourses();

                foreach (var course in courses)
                {
                    if (course.CertificateId == null)
                    {
                        course.CertificateId = createdCertificate.CertificateId;
                        course.UpdateAt = TimeHepler.SystemTimeNow;
                        var courseUpdate = await _courseRepo.UpdateCourse(course);
                        if (courseUpdate == null)
                        {
                            res.IsSuccess = false;
                            res.ResponseCode = ResponseCodeConstants.FAILED;
                            res.Message = ResponseMessageConstrantsCourse.COURSE_UPDATED_FAILED;
                            res.StatusCode = StatusCodes.Status400BadRequest;
                            return res;
                        }
                    }
                }
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<CertificateResponse>(createdCertificate);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_CREATE_SUCCESSFUL;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message ?? ex.Message;
                return res;
            }
        }
        public async Task<ResultModel> GetCertificateByCustomerId()
        {
            var res = new ResultModel();
            try
            {
                var identity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var customerid = await _customerRepo.GetCustomerIdByAccountId(accountId);
                if (customerid == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }
                var enrollCerts = await _enrollCertRepo.GetEnrollCertByCustomerId(customerid);

                if (enrollCerts == null || !enrollCerts.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var certificateIds = enrollCerts.Select(ec => ec.CertificateId).ToList();

                var certificates = await _certificateRepo.GetCertificatesByIds(certificateIds);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<CertificateResponse>>(certificates);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_FOUND;
                return res;

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetEnrollCertificateById(string id)
        {
            var res = new ResultModel();
            try
            {
                var cert = await _enrollCertRepo.GetEnrollCertById(id);
                if (cert == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<EnrollCertificateCurrentCustomerResponse>(cert);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetEnrollCertificateByCurrentCustomer()
        {
            var res = new ResultModel();
            try
            {
                var identity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var customerid = await _customerRepo.GetCustomerIdByAccountId(accountId);
                if (customerid == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var enrollCertificate = await _enrollCertRepo.GetEnrollCertByCustomerId(customerid);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<EnrollCertificateCurrentCustomerResponse>>(enrollCertificate);
                res.Message = ResponseMessageConstrantsCertificate.CERTIFICATE_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        private async Task<string> GenerateCertificateImageAndUploadToCloudinary(string studentName, string creatorName, string courseName, DateOnly issueDate)
        {
            try
            {
                var cloudinary = new Cloudinary(new Account(
                    _configuration["Cloudinary:CloudName"],
                    _configuration["Cloudinary:ApiKey"],
                    _configuration["Cloudinary:ApiSecret"]
                ));

                // Create certificate image with 16:9 ratio
                using (var bitmap = new Bitmap(1920, 1080))
                using (var graphics = Graphics.FromImage(bitmap))
                using (var memoryStream = new MemoryStream())
                {
                    // Set high quality rendering
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    // Create dark red background
                    using (var brush = new SolidBrush(Color.FromArgb(153, 0, 0))) // Dark red color
                    {
                        graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                    }

                    // Load and draw decorative elements
                    using (var logoImage = Image.FromFile(Path.Combine(_environment.WebRootPath, "images", "BitKoi-dark.png")))
                    using (var topCornerImage = Image.FromFile(Path.Combine(_environment.WebRootPath, "images", "gold_corner_top_right.png")))
                    using (var bottomCornerImage = Image.FromFile(Path.Combine(_environment.WebRootPath, "images", "gold_corner_bottom_left.png")))
                    using (var koiLotusImage = Image.FromFile(Path.Combine(_environment.WebRootPath, "images", "koi_lotus.png")))
                    {
                        // Draw logo in top left corner
                        graphics.DrawImage(logoImage, 60, 40, 180, 180);

                        // Draw decorative corner in top right
                        graphics.DrawImage(topCornerImage, bitmap.Width - 200, 40, 160, 160);

                        // Draw decorative corner in bottom left
                        graphics.DrawImage(bottomCornerImage, 40, bitmap.Height - 200, 160, 160);

                        // Draw koi lotus decoration in bottom left
                        float koiLotusRatio = 500f / 187f;
                        float desiredWidth = 400;
                        float calculatedHeight = desiredWidth / koiLotusRatio;
                        graphics.DrawImage(koiLotusImage, 40, bitmap.Height - calculatedHeight - 40, desiredWidth, calculatedHeight);

                        // Draw main title - centered and larger
                        using (var titleFont = new Font("Arial", 48, FontStyle.Bold))
                        {
                            var titleText = "Giấy Chứng Nhận Hoàn Thành Khóa Học";
                            var size = graphics.MeasureString(titleText, titleFont);
                            var titleX = (bitmap.Width - size.Width) / 2;
                            graphics.DrawString(titleText, titleFont, Brushes.White, titleX, 180);
                        }

                        // Draw course name - centered and bold
                        using (var courseNameFont = new Font("Arial", 42, FontStyle.Bold))
                        {
                            var size = graphics.MeasureString(courseName, courseNameFont);
                            var courseNameX = (bitmap.Width - size.Width) / 2;
                            graphics.DrawString(courseName, courseNameFont, Brushes.White, courseNameX, 250);
                        }

                        // Draw congratulatory message - centered
                        using (var messageFont = new Font("Arial", 20, FontStyle.Regular))
                        {
                            var messageText = "Chúc mừng bạn đã hoàn thành khóa học! Bạn đã hoàn thành khóa học xuất sắc và đã đạt được các kĩ năng cần thiết sau khóa học trên!";
                            var messageRect = new RectangleF(bitmap.Width * 0.15f, 330, bitmap.Width * 0.7f, 100);
                            var messageFormat = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            graphics.DrawString(messageText, messageFont, Brushes.White, messageRect, messageFormat);
                        }

                        // Position participant and creator sections
                        float participantX = bitmap.Width * 0.3f;
                        float creatorX = bitmap.Width * 0.7f;
                        float sectionY = 450; // Moved up to match the design

                        // Draw participant and creator sections with updated styling
                        DrawParticipantSection(graphics, studentName, participantX, sectionY);
                        DrawCreatorSection(graphics, creatorName, creatorX, sectionY);

                        // Add issue date at the bottom
                        using (var dateFont = new Font("Arial", 16, FontStyle.Regular))
                        {
                            var dateText = $"Ngày cấp: {issueDate.ToString("dd/MM/yyyy")}";
                            var dateSize = graphics.MeasureString(dateText, dateFont);
                            graphics.DrawString(dateText, dateFont, Brushes.White,
                                (bitmap.Width - dateSize.Width) / 2, bitmap.Height - 100);
                        }
                    }

                    // Save as PNG to memory stream
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Position = 0;

                    // Upload to Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription($"certificate_{studentName}_{courseName}_{Guid.NewGuid()}.png", memoryStream),
                        Folder = "certificates"
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    return uploadResult.SecureUrl.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating certificate: {ex.Message}");
                return _configuration["DefaultCertificateImageUrl"];
            }
        }

        private void DrawParticipantSection(Graphics graphics, string studentName, float xPosition, float yPosition)
        {
            using (var labelFont = new Font("Arial", 16, FontStyle.Regular))
            using (var nameFont = new Font("Arial", 36, FontStyle.Bold))
            using (var fullNameFont = new Font("Arial", 14, FontStyle.Regular))
            {
                // Draw "Người tham dự khóa học" label
                var labelText = "Người tham dự khóa học";
                var labelSize = graphics.MeasureString(labelText, labelFont);
                graphics.DrawString(labelText, labelFont, Brushes.White,
                    xPosition - (labelSize.Width / 2), yPosition);

                // Draw first name (larger)
                var firstName = GetFirstName(studentName);
                var nameSize = graphics.MeasureString(firstName, nameFont);
                graphics.DrawString(firstName, nameFont, Brushes.White,
                    xPosition - (nameSize.Width / 2), yPosition + 30);

                // Draw full name (smaller)
                var fullNameSize = graphics.MeasureString(studentName, fullNameFont);
                graphics.DrawString(studentName, fullNameFont, Brushes.White,
                    xPosition - (fullNameSize.Width / 2), yPosition + 80);

                // Draw decorative line
                using (var pen = new Pen(Color.White, 1))
                {
                    float lineWidth = 200;
                    graphics.DrawLine(pen,
                        xPosition - (lineWidth / 2), yPosition + 110,
                        xPosition + (lineWidth / 2), yPosition + 110);
                }
            }
        }

        private void DrawCreatorSection(Graphics graphics, string creatorName, float xPosition, float yPosition)
        {
            using (var labelFont = new Font("Arial", 16, FontStyle.Regular))
            using (var nameFont = new Font("Arial", 36, FontStyle.Bold))
            using (var fullNameFont = new Font("Arial", 14, FontStyle.Regular))
            {
                // Draw "Người tạo khóa học" label
                var labelText = "Người tạo khóa học";
                var labelSize = graphics.MeasureString(labelText, labelFont);
                graphics.DrawString(labelText, labelFont, Brushes.White,
                    xPosition - (labelSize.Width / 2), yPosition);

                // Draw first name (larger)
                var firstName = GetFirstName(creatorName);
                var nameSize = graphics.MeasureString(firstName, nameFont);
                graphics.DrawString(firstName, nameFont, Brushes.White,
                    xPosition - (nameSize.Width / 2), yPosition + 30);

                // Draw full name (smaller)
                var fullNameSize = graphics.MeasureString(creatorName, fullNameFont);
                graphics.DrawString(creatorName, fullNameFont, Brushes.White,
                    xPosition - (fullNameSize.Width / 2), yPosition + 80);

                // Draw decorative line
                using (var pen = new Pen(Color.White, 1))
                {
                    float lineWidth = 200;
                    graphics.DrawLine(pen,
                        xPosition - (lineWidth / 2), yPosition + 110,
                        xPosition + (lineWidth / 2), yPosition + 110);
                }
            }
        }

        // Helper method to extract the first name from a full Vietnamese name
        private string GetFirstName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            var parts = fullName.Trim().Split(' ');
            return parts.Length > 0 ? parts[parts.Length - 1] : fullName;
        }
    }
}