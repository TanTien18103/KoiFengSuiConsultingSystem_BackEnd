﻿using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.QuestionRepository;
using Repositories.Repositories.QuizRepository;
using Services.ApiModels;
using Services.ApiModels.Answer;
using Services.ApiModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using static Services.ApiModels.Question.QuestionRequest;

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

                // Create a fresh response with updated data
                var Question = await _questionRepository.GetQuestionById(questionId);

                if (Question == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_NOT_FOUND;
                    return res;
                }

                var Answers = await _answerRepo.GetAnswersByQuestionId(questionId);

                if (Answers == null || !Answers.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND;
                    return res;
                }
                // Map the question to response
                var questionResponse = _mapper.Map<QuestionResponse>(Question);
                // Manually set the updated answers to ensure they reflect the latest changes
                questionResponse.Answers = _mapper.Map<List<AnswerResponse>>(Answers);

                res.Data = questionResponse;
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

        public async Task<ResultModel> GetQuestionsByQuizIdMaster(string quizId)
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
                    var questions = await _questionRepository.GetQuestionsByQuizId(quizId);
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
                var quiz = await _quizRepo.GetQuizById(quizId);
                if (quiz == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantQuiz.QUIZ_NOT_FOUND;
                    return res;
                }

                var existingQuestions = await _questionRepository.GetQuestionsByQuizId(quizId);
                int totalQuestionCount = existingQuestions.Count() + 1;

                decimal pointPerQuestion = totalQuestionCount > 0 ? quiz.Score.Value / totalQuestionCount : 0m;

                foreach (var q in existingQuestions)
                {
                    q.Point = pointPerQuestion;
                    await _questionRepository.UpdateQuestion(q);
                }

                var question = _mapper.Map<Question>(questionRequest);
                question.QuestionId = GenerateShortGuid();
                question.QuizId = quizId;
                question.CreateAt = TimeHepler.SystemTimeNow;
                question.UpdateAt = TimeHepler.SystemTimeNow;
                question.Point = pointPerQuestion;

                if (question.Answers != null && question.Answers.Any())
                {
                    foreach (var answer in question.Answers)
                    {
                        answer.AnswerId = GenerateShortGuid();
                        answer.QuestionId = question.QuestionId;
                        answer.CreateAt = TimeHepler.SystemTimeNow;
                        answer.UpdateDate = TimeHepler.SystemTimeNow;
                    }
                }

                var created = await _questionRepository.CreateQuestion(question);
                if (created == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTION_CREATE_FAILED;
                    return res;
                }

                res.Data = _mapper.Map<QuestionResponse>(created);
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
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> UpdateQuestion(string questionId, QuestionUpdateRequest questionRequest)
        {
            try
            {
                var question = await _questionRepository.GetQuestionById(questionId);
                if (question == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = ResponseMessageConstrantsQuestion.QUESTIONS_NOT_FOUND
                    };
                }

                var answers = await _answerRepo.GetAnswersByQuestionId(questionId);

                if (answers == null || !answers.Any())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND
                    };
                }

                // Update answers
                foreach (var answer in answers)
                {
                    var answerRequest = questionRequest.answerUpdateRequests.FirstOrDefault(x => x.AnswerId == answer.AnswerId);
                    if (answerRequest != null)
                    {
                        answer.OptionText = answerRequest.OptionText;
                        answer.OptionType = answerRequest.OptionType;
                        answer.IsCorrect = answerRequest.IsCorrect;
                        await _answerRepo.UpdateAnswer(answer);
                    }
                }

                // Update question
                question.QuestionText = questionRequest.QuestionText;
                question.QuestionType = questionRequest.QuestionType;
                await _questionRepository.UpdateQuestion(question);

                // Create a fresh response with updated data
                var updatedQuestion = await _questionRepository.GetQuestionById(questionId);
                var updatedAnswers = await _answerRepo.GetAnswersByQuestionId(questionId);

                // Map the question to response
                var questionResponse = _mapper.Map<QuestionResponse>(updatedQuestion);

                // Manually set the updated answers to ensure they reflect the latest changes
                questionResponse.Answers = _mapper.Map<List<AnswerResponse>>(updatedAnswers);

                return new ResultModel
                {
                    Data = questionResponse,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ResponseMessageConstrantsQuestion.QUESTION_UPDATED_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
        public async Task<ResultModel> GetQuestionsByQuizId(string quizId)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrWhiteSpace(quizId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.INVALID_INPUT;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var questions = await _questionRepository.GetQuestionsByQuizId(quizId);
                if (questions == null || !questions.Any())
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
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.InnerException?.Message ?? ex.Message
                };
            }
        }
    }
}
