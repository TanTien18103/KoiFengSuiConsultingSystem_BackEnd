using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.LocationRepository
{
    public interface ILocationRepo
    {
        Task<Location> GetLocationByIdRepo(string locationId);
        Task<List<Location>> GetLocationsRepo();
        Task<Location> CreateLocationRepo(Location location);
        Task<Location> UpdateLocationRepo(Location location);
        Task DeleteLocationRepo(string locationId);
    }
}
