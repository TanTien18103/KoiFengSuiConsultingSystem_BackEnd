using BusinessObjects.Enums;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IRegisterAttendService
    {
        Task<ResultModel> GetRegisterAttends(RegisterAttendStatusEnums? status = null);

        Task<ResultModel> GetRegisterAttendById(string registerAttendId);
        Task<ResultModel> GetRegisterAttends();
        Task<ResultModel> GetRegisterAttendByCustomerId();
        Task<ResultModel> GetRegisterAttendByWorkshopId(string id);

    }
}
