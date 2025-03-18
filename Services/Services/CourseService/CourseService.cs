using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.CourseRepository;
using Services.ApiModels;
using Services.ApiModels.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Constants;
using Services.ApiModels.Workshop;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using System.Security.Claims;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Repositories.AccountRepository;

namespace Services.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepo _courseRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;


        public CourseService(ICourseRepo courseRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountRepo accountRepo)
        {
            _courseRepo = courseRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
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

        private string GetAuthenticatedName()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        }

        public async Task<ResultModel> GetCourseById(string courseId)
        {
            var res = new ResultModel();

            try
            {
                var course = await _courseRepo.GetCourseById(courseId);
                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<CourseRespone>(course);
                res.Message = ResponseMessageConstrantsCourse.COURSE_INFO_FOUND;
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

        public async Task<ResultModel> GetCourses()
        {
            var res = new ResultModel();
            try
            {
                var courses = await _courseRepo.GetCourses();
                if (courses == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<CourseRespone>>(courses);
                res.Message = ResponseMessageConstrantsCourse.COURSE_INFO_FOUND;
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
        public async Task<ResultModel> CreateCourse(CourseRequest request)
        {
            var res = new ResultModel();

            try
            {
                if (request == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_INFO_INVALID;
                    return res;
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                var course = _mapper.Map<Course>(request);
                course.CourseId = GenerateShortGuid();
                course.CreateAt = DateTime.UtcNow;
                course.Status = CourseStatusEnum.Active.ToString();
                course.CreateBy = accountId;

                if (course.CreateBy == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.NOTFOUND_ACCOUNTID_CORRESPONDING_TO_ACCOUNT;
                    return res;
                }

                await _courseRepo.CreateCourse(course);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = _mapper.Map<CourseRespone>(course);
                res.Message = ResponseMessageConstrantsCourse.COURSE_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi tạo khóa học: {ex.Message}";
                return res;
            }
        }
        public async Task<ResultModel> UpdateCourse(string id, CourseRequest request)
        {
            var res = new ResultModel();
            try
            {
                var course = await _courseRepo.GetCourseById(id);
                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    res.Message = ResponseMessageIdentity.UNAUTHENTICATED_OR_UNAUTHORIZED;
                    return res;
                }

                _mapper.Map(request, course);
                course.UpdateAt = DateTime.UtcNow;
                await _courseRepo.UpdateCourse(course);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<CourseRespone>(course);
                res.Message = ResponseMessageConstrantsCourse.COURSE_UPDATED_SUCCESS;
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

        public async Task<ResultModel> DeleteCourse(string courseId)
        {
            var res = new ResultModel();
            try
            {
                var course = await _courseRepo.GetCourseById(courseId);
                if (course == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.COURSE_NOT_FOUND;
                    return res;
                }

                await _courseRepo.DeleteCourse(courseId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsCourse.COURSE_DELETED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi xóa khóa học: {ex.Message}";
                return res;
            }
        }
    }
}
