using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Contract
{
    public class ContractRequest
    {
        public string BookingOfflineId { get; set; }
        public IFormFile PdfFile { get; set; }
    }
    public class VerifyOtpRequest
    {
        [Required]
        public string OtpCode { get; set; }
    }
}
