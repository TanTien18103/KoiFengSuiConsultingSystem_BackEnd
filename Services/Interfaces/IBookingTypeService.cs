using BusinessObjects.Enums;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBookingTypeService
    {
        Task<ResultModel> Calculate(BookingTypeEnums bookingType, string bookingId);
    }
}
