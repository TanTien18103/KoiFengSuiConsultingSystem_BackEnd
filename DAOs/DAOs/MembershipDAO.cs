using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class MembershipDAO
    {
        private static volatile MembershipDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private MembershipDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static MembershipDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MembershipDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Membership> GetMembershipByIdDao(string membershipId)
        {
            return await _context.Memberships.FindAsync(membershipId);
        }

        public async Task<List<Membership>> GetMembershipsDao()
        {
            return await _context.Memberships.ToListAsync();
        }

        public async Task<Membership> CreateMembershipDao(Membership membership)
        {
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<Membership> UpdateMembershipDao(Membership membership)
        {
            _context.Memberships.Update(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task DeleteMembershipDao(string membershipId)
        {
            var membership = await GetMembershipByIdDao(membershipId);
            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
        }
    }
}
