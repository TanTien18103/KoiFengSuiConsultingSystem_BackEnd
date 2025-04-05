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
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.CategoryRepository;
using Services.ApiModels.Order;
using BusinessObjects.Exceptions;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.OrderRepository;
using Services.ServicesHelpers.UploadService;

namespace Services.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepo _courseRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;
        private readonly IMasterRepo _masterRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IUploadService _uploadService;

        public CourseService(ICourseRepo courseRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountRepo accountRepo, IMasterRepo masterRepo, ICategoryRepo categoryRepo, ICustomerRepo customerRepo, IOrderRepo orderRepo, IUploadService uploadService)
        {
            _courseRepo = courseRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
            _masterRepo = masterRepo;
            _categoryRepo = categoryRepo;
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
            _uploadService = uploadService;
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
                res.Data = _mapper.Map<CourseResponse>(course);
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
                res.Data = _mapper.Map<List<CourseResponse>>(courses);
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
                var masterid = await _masterRepo.GetMasterIdByAccountId(accountId);
                if (string.IsNullOrEmpty(masterid))
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
                course.Status = CourseStatusEnum.Inactive.ToString();
                course.CreateBy = masterid;
                course.CategoryId = request.CourseCategory;
                course.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);

                if (course.CreateBy == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCourse.NOTFOUND_ACCOUNTID_CORRESPONDING_TO_ACCOUNT;
                    return res;
                }

                var result = await _courseRepo.CreateCourse(course);
                var response = await _courseRepo.GetCourseById(result.CourseId);
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Data = _mapper.Map<CourseResponse>(response);
                res.Message = ResponseMessageConstrantsCourse.COURSE_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message =ex.InnerException?.Message;
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
                course.CategoryId = request.CourseCategory;
                course.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);

                await _courseRepo.UpdateCourse(course);
                var response = await _courseRepo.GetCourseById(id);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<CourseResponse>(response);
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

        public async Task<ResultModel> GetCoursesByMaster()
        {
            var res = new ResultModel();
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_NOT_SEND;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var master = await _masterRepo.GetMasterByAccountId(accountId);
                if (master == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsMaster.MASTER_INFO_NOT_FOUND;
                    return res;
                }

                var courses = await _courseRepo.GetCoursesByMasterId(master.MasterId);
                if (courses == null || !courses.Any())
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
                res.Data = _mapper.Map<List<CourseByMasterResponse>>(courses);
                res.Message = ResponseMessageConstrantsCourse.COURSE_INFO_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi lấy danh sách khóa học của master: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> GetIsBestSellerCourses()
        {
            var res = new ResultModel();
            try
            {
                var courses = await _courseRepo.GetCourses();
                var bestSeller = courses.Where(x => x.IsBestSeller == true).ToList();
                if (bestSeller == null)
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
                res.Data = _mapper.Map<List<CourseResponse>>(bestSeller);
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

        public async Task<ResultModel> GetCoursesByCategoryId(string id)
        {
            var res = new ResultModel();
            try
            {
                var category = await _categoryRepo.GetCategoryById(id);
                if(category == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_NOT_FOUND;
                    return res;
                }

                var courses = await _courseRepo.GetCourses();
                var coursesByCategoryId = courses.Where(x => x.CategoryId == id).ToList();
                if (coursesByCategoryId == null)
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
                res.Data = _mapper.Map<List<CourseResponse>>(coursesByCategoryId);
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

        public async Task<ResultModel> SortByRating()
        {
            var res = new ResultModel();
            try
            {
                var courses = await _courseRepo.GetCourses();
                var bestSeller = courses.OrderByDescending(x => x.Rating).ToList();
                if (bestSeller == null)
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
                res.Data = _mapper.Map<List<CourseResponse>>(bestSeller);
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

        public async Task<ResultModel> GetCourseByIdForMobile(string courseId)
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

                var courseDetail = _mapper.Map<CourseDetailResponse>(course);

                courseDetail.EnrolledStudents = course.RegisterCourses?.Count ?? 0;

                courseDetail.TotalChapters = course.Chapters?.Count ?? 0;

                if (course.Chapters != null && course.Chapters.Any())
                {
                    var durations = course.Chapters
                        .Where(c => c.Duration.HasValue)
                        .Select(c => c.Duration.Value);

                    if (durations.Any())
                    {
                        TimeSpan totalTimeSpan = TimeSpan.Zero;
                        foreach (var duration in durations)
                        {
                            totalTimeSpan = totalTimeSpan.Add(new TimeSpan(duration.Hour, duration.Minute, duration.Second));
                        }

                        courseDetail.TotalDuration = new TimeOnly(totalTimeSpan.Hours, totalTimeSpan.Minutes, totalTimeSpan.Seconds);
                    }
                }
                int totalQuestions = 0; 
                if (course.Quizzes != null && course.Quizzes.Any())
                {
                    totalQuestions = course.Quizzes
                        .SelectMany(q => q.Questions)
                        .Count();
                }
                courseDetail.TotalQuestions = totalQuestions;

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = courseDetail;
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

        public async Task<ResultModel> GetPurchasedCourses()
        {
            var res = new ResultModel();
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_NOT_SEND;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var curCustomer = await _customerRepo.GetCustomerByAccountId(accountId);

                var paidOrders = await _orderRepo.GetOrdersByCustomerAndType(
                    curCustomer.CustomerId,
                    PaymentTypeEnums.Course.ToString(),
                    PaymentStatusEnums.Paid.ToString());

                if (paidOrders == null || !paidOrders.Any())
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = new List<CourseResponse>();
                    res.Message = ResponseMessageConstrantsCourse.PAID_COURSES_NOT_FOUND;
                    return res;
                }

                var courseIds = paidOrders.Select(o => o.ServiceId).ToList();
                var courses = await _courseRepo.GetCoursesByIds(courseIds);
                var courseViewModels = _mapper.Map<List<CourseResponse>>(courses);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = courseViewModels;
                res.Message = ResponseMessageConstrantsCourse.PAID_COURSES_FOUND;
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
