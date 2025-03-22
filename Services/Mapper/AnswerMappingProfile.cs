using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Answer;
using Services.ApiModels.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class AnswerMappingProfile : Profile
    {
        public AnswerMappingProfile()
        {
            CreateMap<Answer, AnswerResponse>();

            CreateMap<AnswerRequest, Answer>();
        }
    }
}
