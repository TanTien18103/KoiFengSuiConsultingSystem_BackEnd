using System;

namespace Services.ApiModels.Account
{
    public class AccountResponse
    {
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public bool? Gender { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
} 