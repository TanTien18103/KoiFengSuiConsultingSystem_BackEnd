using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Category
{
    public class CategoryRequest
    {
        [Required]
        public string CategoryName { get; set; }

    }
}
