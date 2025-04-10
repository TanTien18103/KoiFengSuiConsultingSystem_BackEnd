using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Category
{
    public class CategoryResponse
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }

    }
}
