using BusinessObjects.Enums;
using Services.ApiModels;
using Services.ApiModels.RegisterAttend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.RegisterAttendService
{
    public interface IRegisterAttendService
    {
        Task<ResultModel> GetRegisterAttends(RegisterAttendStatusEnums? status = null);
        Task<ResultModel> GetRegisterAttendsByCurrentUser(RegisterAttendStatusEnums? status = null);
        Task<ResultModel> GetRegisterAttendById(string registerAttendId);
        Task<ResultModel> GetRegisterAttends();
        Task<ResultModel> GetRegisterAttendByCustomerId();
        Task<ResultModel> GetRegisterAttendByGroupId(string groupId);
        Task<ResultModel> GetRegisterAttendByWorkshopId(string id);
        Task<ResultModel> CreateRegisterAttend(RegisterAttendRequest request);
        Task<ResultModel> UpdatePendingTickets(string workshopId, int newNumberOfTickets);
    }
}
