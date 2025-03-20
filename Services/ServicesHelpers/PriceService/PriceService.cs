using BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.RegisterAttendRepository;
using Repositories.Repositories.WorkShopRepository;
using Microsoft.AspNetCore.Http;
using BusinessObjects.Exceptions;
using BusinessObjects.Constants;

namespace Services.ServicesHelpers.PriceService
{
    public class PriceService : IPriceService
    {
        private readonly IBookingOnlineRepo _bookingOnlineRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly ICourseRepo _courseRepo;
        private readonly IRegisterAttendRepo _registerAttendRepo;

        public PriceService(
            IBookingOnlineRepo bookingOnlineRepo,
            IBookingOfflineRepo bookingOfflineRepo,
            ICourseRepo courseRepo,
            IRegisterAttendRepo registerAttendRepo)
        {
            _bookingOnlineRepo = bookingOnlineRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _courseRepo = courseRepo;
            _registerAttendRepo = registerAttendRepo;
        }

        public async Task<decimal?> GetServicePrice(PaymentTypeEnums serviceType, string serviceId, bool isFirstPayment = true, decimal? selectedPrice = null)
        {
            try
            {
                switch (serviceType)
                {
                    case PaymentTypeEnums.BookingOnline:
                        var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(serviceId);
                        return bookingOnline?.Price;

                    case PaymentTypeEnums.BookingOffline:
                        var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(serviceId);
                        if (bookingOffline?.ConsultationPackage == null)
                            throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsBooking.BOOKING_NO_PACKAGE, StatusCodes.Status400BadRequest);

                        if (selectedPrice == null)
                            throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsBooking.PRICE_NOT_CHOSEN, StatusCodes.Status400BadRequest);

                        if (selectedPrice != bookingOffline.ConsultationPackage.MinPrice &&
                            selectedPrice != bookingOffline.ConsultationPackage.MaxPrice)
                        {
                            throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsBooking.PRICE_SELECTED_INVALID, StatusCodes.Status400BadRequest);
                        }
                        return isFirstPayment ? selectedPrice * 0.3m : selectedPrice * 0.7m;

                    case PaymentTypeEnums.Course:
                        var course = await _courseRepo.GetCourseById(serviceId);
                        return course?.Price;

                    case PaymentTypeEnums.RegisterAttend:
                        var registerAttends = await _registerAttendRepo.GetRegisterAttendsByGroupId(serviceId);
                        if (registerAttends != null && registerAttends.Any() && registerAttends.First().Workshop != null)
                        {
                            return registerAttends.First().Workshop.Price;
                        }
                        return null;
                    default:
                        throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.SERVICETYPE_INVALID, StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting price for service: {ex.Message}");
            }
        }
    }
}