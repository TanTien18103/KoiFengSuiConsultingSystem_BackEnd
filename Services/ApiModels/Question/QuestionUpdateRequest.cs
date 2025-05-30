﻿using Services.ApiModels.Answer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Question
{
    public class QuestionUpdateRequest
    {
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Câu hỏi phải từ 5 đến 1000 ký tự")]
        public string? QuestionText { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Loại câu hỏi phải từ 3 đến 100 ký tự")]
        public string? QuestionType { get; set; }

        public List<AnswerUpdateRequest> answerUpdateRequests { get; set; } = new List<AnswerUpdateRequest>();
    }
}
