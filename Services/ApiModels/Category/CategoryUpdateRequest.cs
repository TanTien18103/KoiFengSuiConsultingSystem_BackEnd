using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Category
{
    public class CategoryUpdateRequest
    {
        public string? CategoryName { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
