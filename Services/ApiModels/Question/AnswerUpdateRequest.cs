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

        [RegularExpression(@"^[\p{L}0-9,.\-_/ ]+$", ErrorMessage = "OptionText không được chứa ký tự đặc biệt")]
        public string? OptionText { get; set; }

        [RegularExpression(@"^[\p{L}0-9,.\-_/ ]+$", ErrorMessage = "OptionType không được chứa ký tự đặc biệt")]
        public string? OptionType { get; set; }

        public bool? IsCorrect { get; set; }
    }
}
