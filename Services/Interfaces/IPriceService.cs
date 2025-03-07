using BusinessObjects.Enums;

namespace Services.Interfaces
{
    public interface IPriceService
    {
        Task<decimal?> GetServicePrice(PaymentTypeEnums serviceType, string serviceId);
    }
} 