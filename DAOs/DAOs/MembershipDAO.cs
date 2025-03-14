using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class MembershipDAO
    {
        public static MembershipDAO instance = null;
        private readonly KoiFishPondContext _context;

        public MembershipDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static MembershipDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MembershipDAO();
                }
                return instance;
            }
        }

        public async Task<Membership> GetMembershipByIdDao(string membershipId)
        {
            return await _context.Memberships.FindAsync(membershipId);
        }

        public async Task<List<Membership>> GetMembershipsDao()
        {
            return _context.Memberships.ToList();
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
