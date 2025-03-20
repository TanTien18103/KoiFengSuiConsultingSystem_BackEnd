using BusinessObjects.Enums;

namespace Services.ServicesHelpers.PriceService
{
    public interface IPriceService
    {
        Task<decimal?> GetServicePrice(PaymentTypeEnums serviceType, string serviceId, bool isFirstPayment = true);
    }
}