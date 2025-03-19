using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Account;
using Services.ApiModels.KoiPond;

namespace Services.Mapper
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Account, EditProfileRequest>();
            CreateMap<EditProfileRequest, Account>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // Mapping cho Account Management
            CreateMap<Account, AccountResponse>();
        }
    }
} 