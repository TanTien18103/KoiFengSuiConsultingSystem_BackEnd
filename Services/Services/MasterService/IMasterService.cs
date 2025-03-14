using BusinessObjects.Models;
using Services.ApiModels.KoiPond;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.Master;

namespace Services.Services.MasterService
{
    public interface IMasterService
    {
        Task<ResultModel> GetAllMasters();
        Task<ResultModel> GetMasterById(string masterId);
    }
}
