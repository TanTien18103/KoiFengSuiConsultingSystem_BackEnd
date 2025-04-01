using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.FengShuiDocument;
﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.FengShuiDocumentRepository;
using Services.Services.FengShuiDocumentService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FengShuiDocumentController : ControllerBase
    {
        private readonly IFengShuiDocumentService _fengShuiDocumentService;

        public FengShuiDocumentController(IFengShuiDocumentService fengShuiDocumentService)
        {
            _fengShuiDocumentService = fengShuiDocumentService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> CreateFengShuiDocument([FromForm] CreateFengShuiDocumentRequest request)
        {
            var result = await _fengShuiDocumentService.CreateFengShuiDocument(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("document/{bookingOfflineId}")]
        [Authorize]
        public async Task<IActionResult> GetFengShuiDocumentByBookingOfflineId(string bookingOfflineId)
        {
            var result = await _fengShuiDocumentService.GetFengShuiDocumentByBookingOfflineId(bookingOfflineId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{documentId}/cancel-by-manager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CancelDocumentByManager(string documentId)
        {
            var result = await _fengShuiDocumentService.CancelDocumentByManager(documentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{documentId}/cancel-by-customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelDocumentByCustomer(string documentId)
        {
            var result = await _fengShuiDocumentService.CancelDocumentByCustomer(documentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{documentId}/confirm-by-customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ConfirmDocumentByCustomer(string documentId)
        {
            var result = await _fengShuiDocumentService.ConfirmDocumentByCustomer(documentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{documentId}/confirm-by-manager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ConfirmDocumentByManager(string documentId)
        {
            var result = await _fengShuiDocumentService.ConfirmDocumentByManager(documentId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFengShuiDocumentById([FromRoute] string id)
        {
            var result = await _fengShuiDocumentService.GetFengShuiDocumentById(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFengShuiDocuments()
        {
            var result = await _fengShuiDocumentService.GetAllFengShuiDocuments();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all-by-master")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> GetAllFengShuiDocumentsByMaster()
        {
            var result = await _fengShuiDocumentService.GetAllFengShuiDocumentsByMaster();
            return StatusCode(result.StatusCode, result);
        }
    }
}
