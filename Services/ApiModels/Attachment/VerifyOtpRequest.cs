using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Attachment
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Mã OTP không được để trống")]
        public string OtpCode { get; set; }
    }
}
