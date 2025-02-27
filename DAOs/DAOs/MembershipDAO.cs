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
        private readonly KoiFishPondContext _context;

        public MembershipDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Membership> GetMembershipById(string membershipId)
        {
            return await _context.Memberships.FindAsync(membershipId);
        }

        public async Task<List<Membership>> GetMemberships()
        {
            return _context.Memberships.ToList();
        }

        public async Task<Membership> CreateMembership(Membership membership)
        {
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<Membership> UpdateMembership(Membership membership)
        {
            _context.Memberships.Update(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task DeleteMembership(string membershipId)
        {
            var membership = await GetMembershipById(membershipId);
            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
        }


    }
}
