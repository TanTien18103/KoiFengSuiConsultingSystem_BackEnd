using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Customer
{
    public class CustomerResponse
    {
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; }

        public string CustomerId { get; set; }
        public string LifePalace { get; set; }
        public string Element { get; set; }
        public string MembershipId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
