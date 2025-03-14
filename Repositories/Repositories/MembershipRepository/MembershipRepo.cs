using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.MembershipRepository
{
    public class MembershipRepo : IMembershipRepo
    {
        public Task<Membership> GetMembershipById(string membershipId)
        {
            return MembershipDAO.Instance.GetMembershipByIdDao(membershipId);
        }
        public Task<List<Membership>> GetMemberships()
        {
            return MembershipDAO.Instance.GetMembershipsDao();
        }
        public Task<Membership> CreateMembership(Membership membership)
        {
            return MembershipDAO.Instance.CreateMembershipDao(membership);
        }
        public Task<Membership> UpdateMembership(Membership membership)
        {
            return MembershipDAO.Instance.UpdateMembershipDao(membership);
        }
        public Task DeleteMembership(string membershipId)
        {
            return MembershipDAO.Instance.DeleteMembershipDao(membershipId);
        }
    }
}
