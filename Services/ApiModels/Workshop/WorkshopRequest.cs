using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Workshop
{
    public class WorkshopRequest
    {
        [Required]
        public string WorkshopName { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public string LocationId { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9 ,.\\-_]+$", ErrorMessage = "Mô tả không được chứa ký tự đặc biệt")]
        public string Description { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải là số nguyên dương")]
        public int? Capacity { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public IFormFile ImageUrl { get; set; }

    }
}
