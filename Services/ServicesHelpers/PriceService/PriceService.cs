using BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.RegisterAttendRepository;
using Repositories.Repositories.WorkShopRepository;

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

        public async Task<decimal?> GetServicePrice(PaymentTypeEnums serviceType, string serviceId)
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
                        return bookingOffline?.ConsultationPackage?.MaxPrice;

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
                        throw new ArgumentException("Invalid service type");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting price for service: {ex.Message}");
            }
        }
    }
}