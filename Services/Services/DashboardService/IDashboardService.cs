using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.DashboardService
{
    public interface IDashboardService
    {
        Task <ResultModel> ShowDashboard();
    }
}
