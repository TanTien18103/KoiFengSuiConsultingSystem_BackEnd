using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            CreateMap<QuestionRequest, Question>();
            CreateMap<Question, QuestionResponse>();
            CreateMap<QuestionUpdateRequest, Question>();
            CreateMap<AnswerUpdateRequest, Answer>();
        }
    }
}
