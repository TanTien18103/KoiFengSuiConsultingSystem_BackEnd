using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class EditProfileRequest
    {
        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
            
        public string? FullName { get; set; }

        public DateOnly? Dob { get; set; }

        public bool? Gender { get; set; }

        public int? BankId { get; set; }

        public string? AccountNo { get; set; }

        public string? AccountName { get; set; }
    }
}
