using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class AnswerUpdateRequest
    {
        [Required(ErrorMessage = "AnswerId không được để trống")]
        public string? AnswerId { get; set; }

        public string? OptionText { get; set; }

        public string? OptionType { get; set; }

        public bool? IsCorrect { get; set; }
    }
}
