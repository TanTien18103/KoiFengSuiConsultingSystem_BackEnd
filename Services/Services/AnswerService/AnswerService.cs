using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.QuestionRepository;
using Services.ApiModels;
using Services.ApiModels.Answer;
using Services.ApiModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using AnswerResponse = Services.ApiModels.Answer.AnswerResponse;

namespace Services.Services.AnswerService
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepo _answerRepo;
        private readonly IMapper _mapper;
        private readonly IQuestionRepo _questionRepo;

        public AnswerService(IAnswerRepo answerRepo, IMapper mapper, IQuestionRepo questionRepo)
        {
            _answerRepo = answerRepo;
            _mapper = mapper;
            _questionRepo = questionRepo;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }


        public async Task<ResultModel> CreateAnswer(string questionid, AnswerRequest answerRequest)
        {
            var res = new ResultModel();
            try
            {
                var question = await _questionRepo.GetQuestionById(questionid);
                if (question == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsQuestion.QUESTIONS_NOT_FOUND;
                    return res;
                }

                var answer = _mapper.Map<Answer>(answerRequest);
                answer.AnswerId = GenerateShortGuid();
                answer.QuestionId = questionid;
                answer.CreateAt = TimeHepler.SystemTimeNow;

                var result = await _answerRepo.CreateAnswer(answer);
                if (result == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_CREATE_FAILED;
                    return res;
                }

                res.Data = _mapper.Map<AnswerResponse>(result);
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
                res.Message = ex.InnerException?.Message;
                return res;
            }
        }

        public async Task<ResultModel> DeleteAnswer(string answerId)
        {
            var res = new ResultModel();
            try
            {
                var answer = await _answerRepo.GetAnswerById(answerId);
                if (answer == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND;
                    return res;
                }

                await _answerRepo.DeleteAnswer(answerId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsAnswer.ANSWER_DELETED_SUCCESS;
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

        public async Task<ResultModel> GetAnswerById(string answerId)
        {
            var res = new ResultModel();
            try
            {
                var answer = await _answerRepo.GetAnswerById(answerId);
                if (answer == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND;
                    return res;
                }

                res.Data = _mapper.Map<AnswerResponse>(answer);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsAnswer.ANSWER_FOUND;
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

        public async Task<ResultModel> UpdateAnswer(string answerid, AnswerUpdateRequest answer)
        {
            var res = new ResultModel();

            try
            {
                var answerData = await _answerRepo.GetAnswerById(answerid);
                if (answerData == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND;
                    return res;
                }

                // Chỉ cập nhật các trường có giá trị
                if (!string.IsNullOrWhiteSpace(answer.OptionText))
                    answerData.OptionText = answer.OptionText;

                if (!string.IsNullOrWhiteSpace(answer.OptionType))
                    answerData.OptionType = answer.OptionType;

                if (answer.IsCorrect.HasValue)
                    answerData.IsCorrect = answer.IsCorrect.Value;

                var updatedAnswer = await _answerRepo.UpdateAnswer(answerData);

                if (updatedAnswer == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_UPDATED_FAILED;
                    return res;
                }

                res.Data = _mapper.Map<AnswerResponse>(updatedAnswer);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsAnswer.ANSWER_UPDATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi cập nhật câu trả lời: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> GetAnswersByQuestionId(string questionId)
        {
            var res = new ResultModel();
            try
            {
                var answers = await _answerRepo.GetAnswersByQuestionId(questionId);
                if (answers == null || answers.Count == 0)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsAnswer.ANSWER_NOT_FOUND;
                    return res;
                }

                res.Data = _mapper.Map<List<AnswerResponse>>(answers);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsAnswer.ANSWER_FOUND;
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
    }
}
