using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Answer
{
    public class AnswerRequest
    {
        [Required]
        public string OptionText { get; set; }

        [Required]
        public string OptionType { get; set; }

        [Required]
        public bool? IsCorrect { get; set; }
    }
}
