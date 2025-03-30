using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Attachment;
using Services.ApiModels.FengShuiDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class FengShuiDocumentMappingProfile : Profile
    {
        public FengShuiDocumentMappingProfile() 
        {
            CreateMap<FengShuiDocument, FengShuiDocumentDetailsResponse>();

            CreateMap<FengShuiDocument, FengShuiDocumentResponse>();
        }
    }
}
