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
        [Required(ErrorMessage = "Nội dung phương án không được để trống.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Nội dung phương án phải có ít nhất 1 ký tự và không vượt quá 500 ký tự.")]
        public string OptionText { get; set; }

        [Required(ErrorMessage = "Loại phương án không được để trống.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Loại phương án phải có ít nhất 1 ký tự và không vượt quá 100 ký tự.")]
        public string OptionType { get; set; }

        [Required(ErrorMessage = "Vui lòng chỉ định phương án đúng hay sai.")]
        public bool? IsCorrect { get; set; }
    }
}
