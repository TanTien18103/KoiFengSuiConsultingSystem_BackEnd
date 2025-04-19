using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Certificate
{
    public class CertificateRequest
    {
        [Required(ErrorMessage = "Ngày cấp không được để trống")]
        public DateOnly? IssueDate { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Description { get; set; }

        public IFormFile CertificateImage { get; set; }
    }
}
