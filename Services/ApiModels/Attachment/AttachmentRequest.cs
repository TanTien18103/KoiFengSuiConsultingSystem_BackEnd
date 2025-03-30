using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Attachment
{
    public class AttachmentRequest
    {
        [Required(ErrorMessage = "ID buổi tư vấn offline không được để trống")]
        public string BookingOfflineId { get; set; }

        [Required(ErrorMessage = "File PDF không được để trống")]
        public IFormFile PdfFile { get; set; }
    }
}
