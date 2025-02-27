using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class MembershipRepo : IMembershipRepo
    {
        private readonly MembershipDAO _membershipDAO;

        public MembershipRepo(MembershipDAO membershipDAO)
        {
            _membershipDAO = membershipDAO;
        }

        public async Task<Membership> GetMembershipById(string membershipId)
        {
            return await _membershipDAO.GetMembershipById(membershipId);
        }
        public async Task<Membership> CreateMembership(Membership membership)
        {
            return await _membershipDAO.CreateMembership(membership);
        }

        public async Task<Membership> UpdateMembership(Membership membership)
        {
            return await _membershipDAO.UpdateMembership(membership);
        }

        public async Task DeleteMembership(string membershipId)
        {
            await _membershipDAO.DeleteMembership(membershipId);
        }

        public async Task<List<Membership>> GetMemberships()
        {
            return await _membershipDAO.GetMemberships();
        }
    }
}
