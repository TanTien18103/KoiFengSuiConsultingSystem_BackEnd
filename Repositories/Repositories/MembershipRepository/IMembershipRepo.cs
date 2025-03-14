using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.MembershipRepository
{
    public interface IMembershipRepo
    {
        Task<Membership> GetMembershipById(string membershipId);
        Task<List<Membership>> GetMemberships();
        Task<Membership> CreateMembership(Membership membership);
        Task<Membership> UpdateMembership(Membership membership);
        Task DeleteMembership(string membershipId);
    }
}
