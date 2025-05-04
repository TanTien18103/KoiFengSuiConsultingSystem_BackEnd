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
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Loại câu hỏi phải từ 3 đến 500 ký tự")]
        public string Title { get; set; }
    }
}
