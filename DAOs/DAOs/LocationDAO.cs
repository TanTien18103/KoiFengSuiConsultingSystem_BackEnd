using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class LocationDAO
    {
        private static volatile LocationDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private LocationDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static LocationDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LocationDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Location> GetLocationByIdDao(string locationId)
        {
            return await _context.Locations.FindAsync(locationId);
        }

        public async Task<List<Location>> GetLocationsDao()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> CreateLocationDao(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateLocationDao(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task DeleteLocationDao(string locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
        }
    }
}
