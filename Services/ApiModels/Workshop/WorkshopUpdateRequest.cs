using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Workshop
{
    public class WorkshopUpdateRequest
    {
        public string? WorkshopName { get; set; }
        public DateTime? StartDate { get; set; }
        public string? LocationId { get; set; }
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        public string? Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải là số nguyên dương")]
        public int? Capacity { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
