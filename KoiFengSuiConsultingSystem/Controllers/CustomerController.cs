using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels;
using Services.Services.CustomerService;
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
            var res = await _customerService.GetCurrentCustomerElement();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("calculate-compatibility")]
        public async Task<IActionResult> CalculateCompatibility([FromBody] CompatibilityRequest request)
        {
            var res = await _customerService.CalculateCompatibility(request);
            return StatusCode(res.StatusCode, res);
        }
    }
}

