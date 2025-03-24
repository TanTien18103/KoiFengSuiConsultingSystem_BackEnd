using Services.ApiModels.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.ApiModels.KoiVariety.KoiVarietyRequest;

namespace Services.ApiModels.RegisterCourse
{
    public class RegisterQuizRequest
    {
        public List<AnswerIdRequest> AnswerIds { get; set; } = new List<AnswerIdRequest>();

        public class AnswerIdRequest
        {
            public string AnswerId { get; set; }
        }
    }
}
