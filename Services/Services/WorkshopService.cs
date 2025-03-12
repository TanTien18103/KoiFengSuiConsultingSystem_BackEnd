using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.Workshop;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IWorkShopRepo _workShopRepo;
        private readonly IMapper _mapper;
        public WorkshopService(IWorkShopRepo workShopRepo, IMapper mapper)
        {
            _workShopRepo = workShopRepo;
            _mapper = mapper;
        }
        public async Task<ResultModel> SortingWorkshopByCreatedDate()
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.SortingWorkshopByCreatedDate();
                if(workshops == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo nào";
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK; 
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = "Lấy danh sách buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
        public async Task<ResultModel> ApprovedWorkshop(string id)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null || workshop.Status != WorkshopStatusEnums.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo đang chờ được cập nhật";
                    return res;
                }
                if(workshop != null && workshop.Status == WorkshopStatusEnums.Pending.ToString())
                {
                    workshop.Status = WorkshopStatusEnums.Approved.ToString();
                    await _workShopRepo.UpdateWorkShop(workshop);
                }
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = "Buổi hội thảo được phê duyệt";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
        public async Task<ResultModel> RejectedWorkshop(string id)
        {
            var res = new ResultModel();
            try
            {
                var workshop = await _workShopRepo.GetWorkShopById(id);
                if (workshop == null || workshop.Status != WorkshopStatusEnums.Pending.ToString())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo đang chờ được cập nhật";
                    return res;
                }
                if (workshop != null && workshop.Status == WorkshopStatusEnums.Pending.ToString())
                {
                    workshop.Status = WorkshopStatusEnums.Rejected.ToString();
                    await _workShopRepo.UpdateWorkShop(workshop);
                }
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<WorkshopResponse>(workshop);
                res.Message = "Buổi hội thảo bị từ chối";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
        public async Task<ResultModel> TrendingWorkshop(bool? trending = null)
        {
            var res = new ResultModel();
            try
            {
                var workshops = await _workShopRepo.GetWorkShops();
                if(workshops == null || !workshops.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy buổi hội thảo nào";
                    return res;
                }

                if(trending == true)
                {
                    workshops = workshops.Where(x => x.Trending == trending).ToList();
                }
                if(trending == false)
                {
                    workshops = workshops.Where(x => x.Trending == trending).ToList();
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<WorkshopResponse>>(workshops);
                res.Message = "Lấy danh sách buổi hội thảo thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }
    }
}
    