using BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.WorkShopRepository;

namespace Services.ServicesHelpers.PriceService
{
    public class PriceService : IPriceService
    {
        private readonly IBookingOnlineRepo _bookingOnlineRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly ICourseRepo _courseRepo;
        private readonly IWorkShopRepo _workshopRepo;

        public PriceService(
            IBookingOnlineRepo bookingOnlineRepo,
            IBookingOfflineRepo bookingOfflineRepo,
            ICourseRepo courseRepo,
            IWorkShopRepo workshopRepo)
        {
            _bookingOnlineRepo = bookingOnlineRepo;
            _bookingOfflineRepo = bookingOfflineRepo;
            _courseRepo = courseRepo;
            _workshopRepo = workshopRepo;
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
                        return bookingOffline?.ConsultationPackage?.PackagePrice;

                    case PaymentTypeEnums.Course:
                        var course = await _courseRepo.GetCourseById(serviceId);
                        return course?.Price;

                    case PaymentTypeEnums.Workshop:
                        var workshop = await _workshopRepo.GetWorkShopById(serviceId);
                        return workshop?.Price;

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