using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels;
using Services.Interfaces;
using System.Security.Claims;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }


        [HttpGet("current-customer-element-palace")]
        public async Task<IActionResult> GetCurrentCustomerElementPalace()
        {
            var result = await _customerService.GetElementLifePalaceById();
            
            if (result == null)
                return Unauthorized(new { message = "Customer is not logged in or does not exist." });

            return Ok(result);
        }

        [HttpPost("calculate-compatibility")]
        public async Task<IActionResult> CalculateCompatibility([FromBody] CompatibilityRequest request)
        {
            var result = await _customerService.CalculateCompatibility(request);

            return Ok(new FengShuiResult
            {
                CompatibilityScore = result.CompatibilityScore,
                Message = result.Message
            });
        }


    }

}

