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
        public string OptionText { get; set; }

        [Required(ErrorMessage = "Loại phương án không được để trống.")]
        public string OptionType { get; set; }

        [Required(ErrorMessage = "Vui lòng chỉ định phương án đúng hay sai.")]
        public bool? IsCorrect { get; set; }
    }
}
