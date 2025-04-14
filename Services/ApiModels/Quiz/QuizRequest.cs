using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Quiz
{
    public class QuizRequest
    {
        [Required(ErrorMessage = "Title không được để trống")]
        [RegularExpression(@"^[\p{L}0-9,.\-_/ ]+$", ErrorMessage = "Title không được chứa ký tự đặc biệt")]
        public string Title { get; set; }


    }
}
