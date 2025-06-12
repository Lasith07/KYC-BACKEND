using vue_ts.Services.DetailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using vue_ts.DTOs.Responses;
using vue_ts.DTOs.Requests;

namespace vue_ts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailController : ControllerBase
    {
        private readonly IDetailService detail;

        public DetailController(IDetailService detailservice)
        {
            detail = detailservice;
        }

        [HttpPost("save")]
        public IActionResult CreateDetail([FromBody] CreateDetailRequest request)
        {
            var response = detail.CreateDetail(request);
            return StatusCode(response.status_code, response);
        }
    }
}