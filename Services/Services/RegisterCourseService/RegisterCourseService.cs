using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.ChapterRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.EnrollChapterRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.RegisterCourseRepository;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.RegisterCourseService
{
    public class RegisterCourseService : IRegisterCourseService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;

        private readonly ICourseRepo _courseRepo;
        private readonly IChapterRepo _chapterRepo;

        private ICustomerRepo _customerRepo;

        private readonly IRegisterCourseRepo _registerCourseRepo;
        private readonly IEnrollChapterRepo _enrollChapterRepo;

        private readonly IHttpContextAccessor _contextAccessor;

        public RegisterCourseService(IOrderRepo orderRepo, IMapper mapper, ICourseRepo courseRepo, IChapterRepo chapterRepo, ICustomerRepo customerRepo, IRegisterCourseRepo registerCourseRepo, IEnrollChapterRepo enrollChapterRepo, IHttpContextAccessor contextAccessor)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _courseRepo = courseRepo;
            _chapterRepo = chapterRepo;
            _customerRepo = customerRepo;
            _registerCourseRepo = registerCourseRepo;
            _enrollChapterRepo = enrollChapterRepo;
            _contextAccessor = contextAccessor;
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

                var enrollChapter = await _enrollChapterRepo.GetEnrollChapterByChapterId(chapterId);
                if (enrollChapter == null) {
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

                await _registerCourseRepo.UpdateRegisterCourse(RegisterCourse);

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
    }
}
