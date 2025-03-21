using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.QuizRepository;
using Services.ApiModels;
using Services.ApiModels.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.QuizService
{
    public class QuizService : IQuizService
    {
        private readonly ICourseRepo _courseRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;
        private readonly IMasterRepo _masterRepo;
        private readonly IQuizRepo _quizRepo;

        public QuizService(ICourseRepo courseRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountRepo accountRepo, IMasterRepo masterRepo, IQuizRepo quizRepo)
        {
            _courseRepo = courseRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
            _masterRepo = masterRepo;
            _quizRepo = quizRepo;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<ResultModel> GetQuizzes()
        {
            var res = new ResultModel();
            try
            {
                var quizes = await _quizRepo.GetQuizzes();
                if (quizes == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_FOUND;
                res.Data = _mapper.Map<List<QuizResponse>>(quizes);
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

        public async Task<ResultModel> GetQuizzesByMaster()
        {
            var res = new ResultModel();
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
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

                var master = await _masterRepo.GetMasterByAccountId(accountId);

                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    return res;
                }

                var quizes = await _quizRepo.GetQuizzesByMasterId(master.MasterId);

                if (quizes == null || !quizes.Any())  
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_FOUND;
                res.Data = _mapper.Map<List<QuizResponse>>(quizes);
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

        public async Task<ResultModel> GetQuizById(string quizId)
        {
            var res = new ResultModel();
            try
            {
                var quiz = await _quizRepo.GetQuizById(quizId);
                if (quiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_FOUND;
                res.Data = _mapper.Map<QuizResponse>(quiz);
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

        public async Task<ResultModel> CreateQuiz(string courseId, QuizRequest quiz)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(courseId) || quiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.REQUIRED_INPUT;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
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
                var master = await _masterRepo.GetMasterByAccountId(accountId);

                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    return res;
                }

                var course = await _courseRepo.GetCourseById(courseId);

                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                var newQuiz = _mapper.Map<Quiz>(quiz);
                newQuiz.QuizId = GenerateShortGuid();
                newQuiz.CourseId = courseId;
                newQuiz.CreateBy = master.MasterId;
                newQuiz.CreateAt = DateTime.Now;

                var createdQuiz = await _quizRepo.CreateQuiz(newQuiz);

                if (createdQuiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_CREATE_FAILED;
                    return res;
                }

                var existingQuiz = await _quizRepo.GetQuizById(createdQuiz.QuizId);
                if (existingQuiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                course.UpdateAt = DateTime.Now;
                course.QuizId = existingQuiz.QuizId;

                var updatedCourse = await _courseRepo.UpdateCourse(course);

                if (updatedCourse == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_UPDATED_FAILED;
                    return res;

                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_CREATE_SUCCESS;
                res.Data = _mapper.Map<QuizResponse>(existingQuiz);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> DeleteQuiz(string quizId)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(quizId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.REQUIRED_INPUT;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var quiz = await _quizRepo.GetQuizById(quizId);
                if (quiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                await _quizRepo.DeleteQuiz(quizId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_DELETE_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> UpdateQuiz(string courseId, QuizRequest quiz)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(courseId) || quiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.REQUIRED_INPUT;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_UPDATE_FAILED;
                    return res;
                }

                var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
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

                var master = await _masterRepo.GetMasterByAccountId(accountId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    return res;
                }

                var course = await _courseRepo.GetCourseById(courseId);
                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                var existingQuiz = await _quizRepo.GetQuizByCourseId(courseId);
                if (existingQuiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                existingQuiz.Title = quiz.Title;
                existingQuiz.Score = quiz.Score;
                var updatedQuiz = await _quizRepo.UpdateQuiz(existingQuiz);
                if (updatedQuiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_UPDATE_FAILED;
                    return res;
                }

                course.UpdateAt = DateTime.UtcNow;
                await _courseRepo.UpdateCourse(course);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantQuiz.QUIZ_UPDATE_SUCCESS;
                res.Data = _mapper.Map<QuizResponse>(updatedQuiz);
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

    }
}
