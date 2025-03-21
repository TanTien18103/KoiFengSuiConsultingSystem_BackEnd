using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.QuestionRepository;
using Repositories.Repositories.QuizRepository;
using Services.ApiModels;
using Services.ApiModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.QuestionService
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepo _questionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;
        private readonly IMasterRepo _masterRepo;
        private readonly IQuizRepo _quizRepo;
        private readonly IAnswerRepo _answerRepo;

        public QuestionService(IQuestionRepo questionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountRepo accountRepo, IMasterRepo masterRepo, IQuizRepo quizRepo, IAnswerRepo answerRepo)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
            _masterRepo = masterRepo;
            _quizRepo = quizRepo;
            _answerRepo = answerRepo;
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
        public async Task<ResultModel> GetQuestionById(string questionId)
        {
            var res = new ResultModel();
            try
            {
                var question = await _questionRepository.GetQuestionById(questionId);
                if (question == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_NOT_FOUND;
                    return res;
                }
                res.Data = _mapper.Map<QuestionResponse>(question);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsQuestion.QUESTION_FOUND;
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

        public async Task<ResultModel> GetQuestions()
        {
            var res = new ResultModel();
            try
            {
                var questions = await _questionRepository.GetQuestions();
                if (questions == null || questions.Count == 0)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTIONS_NOT_FOUND;
                    return res;
                }
                res.Data = _mapper.Map<List<QuestionResponse>>(questions);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsQuestion.QUESTIONS_FOUND;
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

        public async Task<ResultModel> GetQuestionsByQuizId(string quizId)
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
                foreach (var quiz in quizes)
                {
                    var questions = await _questionRepository.GetQuestionsByQuizId(quiz.QuizId);
                    if (questions == null || questions.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = ResponseMessageConstrantsQuestion.QUESTIONS_NOT_FOUND;
                        return res;
                    }
                    res.Data = _mapper.Map<List<QuestionResponse>>(questions);
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTIONS_FOUND;
                }
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

        public async Task<ResultModel> CreateQuestion(string quizId, QuestionRequest questionRequest)
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

                var quiz = await _quizRepo.GetQuizById(quizId);

                if (quiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                var question = _mapper.Map<Question>(questionRequest);
                question.QuestionId = GenerateShortGuid();
                question.QuizId = quizId;


                var createdQuestion = await _questionRepository.CreateQuestion(question);

                var answers = new List<Answer>();
                foreach (var answer in questionRequest.Answers)
                {
                    var ans = new Answer
                    {
                        AnswerId = GenerateShortGuid(),
                        QuestionId = createdQuestion.QuestionId,
                        OptionText = answer.OptionText,
                        IsCorrect = answer.IsCorrect,
                        OptionType = answer.OptionType,
                        CreateAt = DateTime.Now
                    };
                    answers.Add(ans);
                }

                foreach (var ans in answers)
                {
                    var createdAnswer = await _answerRepo.CreateAnswer(ans);
                    if (createdAnswer == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.StatusCode = StatusCodes.Status500InternalServerError;
                        res.Message = ResponseMessageConstrantsAnswer.ANSWER_CREATE_FAILED;
                        return res;
                    }
                }

                res.Data = _mapper.Map<QuestionResponse>(createdQuestion);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsQuestion.QUESTION_CREATED_SUCCESS;
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
        public async Task<ResultModel> DeleteQuestion(string questionId)
        {
            var res = new ResultModel();
            try
            {
                var question = await _questionRepository.GetQuestionById(questionId);
                if (question == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_NOT_FOUND;
                    return res;
                }
                await _questionRepository.DeleteQuestion(questionId);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsQuestion.QUESTION_DELETED_SUCCESS;
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

        public async Task<ResultModel> UpdateQuestion(string questionid, QuestionRequest questionRequest)
        {
            var res = new ResultModel();
            try
            {
                var question = await _questionRepository.GetQuestionById(questionid);
                if (question == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_NOT_FOUND;
                    return res;
                }
                var updatedQuestion = _mapper.Map<Question>(questionRequest);
                updatedQuestion.QuestionId = questionid;
                updatedQuestion.QuizId = question.QuizId;
                updatedQuestion.CreateAt = question.CreateAt;

                var updated = await _questionRepository.UpdateQuestion(updatedQuestion);
                if (updated == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_UPDATED_FAILED;
                    return res;
                }
                res.Data = _mapper.Map<QuestionResponse>(updated);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsQuestion.QUESTION_UPDATED_SUCCESS;
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
