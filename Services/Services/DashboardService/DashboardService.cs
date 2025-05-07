using BusinessObjects.Constants;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.OrderRepository;
using Services.ApiModels;
using Services.ApiModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IOrderRepo _orderRepo;

        public DashboardService(IAccountRepo accountRepo, IOrderRepo orderRepo)
        {
            _accountRepo = accountRepo;
            _orderRepo = orderRepo;
        }

        public async Task<ResultModel> ShowDashboard()
        {
            var res = new ResultModel();
            try
            {
                var revenue = await _orderRepo.GetTotalRevenue(); // decimal
                var totalCustomer = await _accountRepo.GetTotalCustomer(); // int

                var maleCount = await _accountRepo.GetGenderCount(true);
                var femaleCount = await _accountRepo.GetGenderCount(false);

                var monthlyStats = await _orderRepo.GetMonthlyServiceStatistics(); // list of MonthlyServiceCount
                var admittedTimeline = await _orderRepo.GetTodayTimeAdmitted(); // list of TimeAdmittedRecord

                var todayCourses = await _orderRepo.GetTodayCourseCount();
                var todayWorkshops = await _orderRepo.GetTodayWorkshopCheckInCount();
                var todayBookingOnline = await _orderRepo.GetTodayBookingOnlineCount();
                var todayBookingOffline = await _orderRepo.GetTodayBookingOfflineCount();

                var monthlyComparison = monthlyStats.Select(dto => new MonthlyComparison
                {
                    MonthYear = dto.MonthYear,
                    Courses = dto.Courses,
                    Workshops = dto.Workshops,
                    BookingOnline = dto.BookingOnline,
                    BookingOffline = dto.BookingOffline
                }).ToList();

                var dashboard = new DashboardResponse
                {
                    Revenue = revenue,
                    TotalCustomers = totalCustomer,
                    GenderStats = new GenderStats
                    {
                        Male = maleCount,
                        Female = femaleCount
                    },
                    MonthlyComparison = monthlyStats.Select(dto => new MonthlyComparison
                    {
                        MonthYear = dto.MonthYear,
                        Courses = dto.Courses,
                        Workshops = dto.Workshops,
                        BookingOnline = dto.BookingOnline,
                        BookingOffline = dto.BookingOffline
                    }).ToList(),
                    TimeAdmittedToday = admittedTimeline.Select(dto => new HourlyAdmit
                    {
                        Count = dto.Count,
                        Time = dto.Time
                    }).ToList(),
                    TodayRecord = new TodayRecord
                    {
                        Courses = todayCourses,
                        CheckInWorkshops = todayWorkshops,
                        BookingOnline = todayBookingOnline,
                        BookingOffline = todayBookingOffline
                    }
                };
                res.IsSuccess = true;
                res.Data = dashboard;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return res;
        }
    }
}
