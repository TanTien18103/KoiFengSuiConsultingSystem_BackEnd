using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.ChapterRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.EnrollAnswerRepository;
using Repositories.Repositories.EnrollChapterRepository;
using Repositories.Repositories.EnrollQuizRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.QuestionRepository;
using Repositories.Repositories.QuizRepository;
using Repositories.Repositories.RegisterCourseRepository;
using Services.ApiModels;
using Services.ApiModels.RegisterCourse;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private ICustomerRepo _customerRepo;

        private readonly IRegisterCourseRepo _registerCourseRepo;
        private readonly IEnrollChapterRepo _enrollChapterRepo;
        private readonly IEnrollQuizRepo _enrollQuizRepo;
        private readonly IEnrollAnswerRepo _enrollAnswerRepo;

        private readonly IHttpContextAccessor _contextAccessor;

        public RegisterCourseService(IOrderRepo orderRepo, IMapper mapper, ICourseRepo courseRepo, IChapterRepo chapterRepo, IQuizRepo quizRepo, IQuestionRepo questionRepo, IAnswerRepo answerRepo, ICustomerRepo customerRepo, IRegisterCourseRepo registerCourseRepo, IEnrollChapterRepo enrollChapterRepo, IEnrollQuizRepo enrollQuizRepo, IEnrollAnswerRepo enrollAnswerRepo, IHttpContextAccessor contextAccessor)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _courseRepo = courseRepo;
            _chapterRepo = chapterRepo;
            _quizRepo = quizRepo;
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _customerRepo = customerRepo;
            _registerCourseRepo = registerCourseRepo;
            _enrollChapterRepo = enrollChapterRepo;
            _enrollQuizRepo = enrollQuizRepo;
            _enrollAnswerRepo = enrollAnswerRepo;
            _contextAccessor = contextAccessor;
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

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = "Quiz submitted successfully!";
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
    }
}
