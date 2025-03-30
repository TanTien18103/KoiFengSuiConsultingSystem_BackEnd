using BusinessObjects.Constants;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.FengShuiDocumentRepository;
using Services.ApiModels.Attachment;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.FengShuiDocument;

namespace Services.Services.FengShuiDocumentService
{
    public class FengShuiDocumentService : IFengShuiDocumentService
    {
        private readonly IFengShuiDocumentRepo _fengShuiDocumentRepo;
        private readonly IMapper _mapper;
        public FengShuiDocumentService(IFengShuiDocumentRepo fengShuiDocumentRepo, IMapper mapper)
        {
            _fengShuiDocumentRepo = fengShuiDocumentRepo;
            _mapper = mapper;
        }

        public async Task<ResultModel> GetFengShuiDocumentById(string id)
        {
            var res = new ResultModel();
            try
            {
                var fengShuiDocument = await _fengShuiDocumentRepo.GetFengShuiDocumentById(id);
                if (fengShuiDocument != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<FengShuiDocumentDetailsResponse>(fengShuiDocument);
                    res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetAllFengShuiDocuments()
        {
            var res = new ResultModel();
            try
            {
                var fengShuiDocuments = await _fengShuiDocumentRepo.GetFengShuiDocuments();
                if (fengShuiDocuments != null)
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<FengShuiDocumentResponse>>(fengShuiDocuments);
                    res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_FOUND;
                    return res;
                }

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                res.Message = ResponseMessageConstrantsFengShuiDocument.FENGSHUIDOCUMENT_NOT_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
}
