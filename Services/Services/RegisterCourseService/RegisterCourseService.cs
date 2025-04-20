using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
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
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using static Services.ApiModels.RegisterCourse.QuizResultResponse;

namespace Services.Services.RegisterCourseService
{
    public class RegisterCourseService : IRegisterCourseService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;

        private readonly ICourseRepo _courseRepo;
        private readonly IChapterRepo _chapterRepo;
        private readonly IQuizRepo _quizRepo;
        private readonly IQuestionRepo _questionRepo;
        private readonly IAnswerRepo _answerRepo;
        private readonly ICertificateRepo _certificateRepo;

        private ICustomerRepo _customerRepo;

        private readonly IRegisterCourseRepo _registerCourseRepo;
        private readonly IEnrollChapterRepo _enrollChapterRepo;
        private readonly IEnrollQuizRepo _enrollQuizRepo;
        private readonly IEnrollAnswerRepo _enrollAnswerRepo;
        private readonly IEnrollCertRepo _enrollCertRepo;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUploadService _uploadService;

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
            IEnrollCertRepo enrollCertRepo)
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
                if (registerCourse != null)
                {
                    var quiz = await _quizRepo.GetQuizById(enrollQuiz.QuizId);
                    if (totalScore >= quiz.Score.GetValueOrDefault() * 0.8m)
                    {
                        var existingEnrollCert = await _enrollCertRepo.GetByCustomerIdAndCertificateId(customerid, course.CertificateId);

                        if (existingEnrollCert == null)
                        {
                            var latestCertificate = (await _certificateRepo.GetAllCertificates())
                                               .OrderBy(c => c.CreateDate)
                                               .FirstOrDefault();

                            var enrollCert = new EnrollCert
                            {
                                EnrollCertId = GenerateShortGuid(),
                                CustomerId = customerid,
                                CertificateId = latestCertificate.CertificateId,
                                FinishDate = DateOnly.FromDateTime(DateTime.Now),
                                CreateDate = DateTime.Now,
                            };
                            await _enrollCertRepo.CreateEnrollCert(enrollCert);

                            registerCourse.EnrollCertId = enrollCert.EnrollCertId;
                            registerCourse.Status = RegisterCourseStatusEnums.Completed.ToString();
                            registerCourse.UpdateDate = DateTime.Now;
                            await _registerCourseRepo.UpdateRegisterCourse(registerCourse);
                        }

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
                certificate.CreateDate = DateTime.UtcNow;
                certificate.UpdateDate = DateTime.UtcNow;
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
                        course.UpdateAt = DateTime.Now;
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
    }
}
