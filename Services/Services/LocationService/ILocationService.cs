using Services.ApiModels;
using Services.ApiModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.LocationService
{
    public interface ILocationService
    {
        Task<ResultModel> CreateLocation(LocationRequest locationRequest);
        Task<ResultModel> UpdateLocation(string Id, LocationUpdateRequest locationRequest);
        Task<ResultModel> DeleteLocation(string Id);
        Task<ResultModel> GetAllLocations();
        Task<ResultModel> GetLocationById(string Id);
    }
}
