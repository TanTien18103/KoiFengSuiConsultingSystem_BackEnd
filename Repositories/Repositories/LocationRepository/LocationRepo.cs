using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.LocationRepository
{
    public class LocationRepo : ILocationRepo
    {
        public Task<Location> CreateLocationRepo(Location location)
        {
            return LocationDAO.Instance.CreateLocationDao(location);
        }

        public Task DeleteLocationRepo(string locationId)
        {
            return LocationDAO.Instance.DeleteLocationDao(locationId);
        }

        public Task<Location> GetLocationByIdRepo(string locationId)
        {
            return LocationDAO.Instance.GetLocationByIdDao(locationId);
        }

        public Task<List<Location>> GetLocationsRepo()
        {
            return LocationDAO.Instance.GetLocationsDao();
        }

        public Task<Location> UpdateLocationRepo(Location location)
        {
            return LocationDAO.Instance.UpdateLocationDao(location);
        }
    }
}
