using Hakon.Core.Extensions;
using Hakon.Core.Brain.Cortex;
using Microsoft.AspNetCore.Mvc;

namespace Hakon.Web.Features.Api
{
    public class ApiController : Controller
    {
        private readonly ICortex _cortex;

        public ApiController(ICortex cortex){
            this._cortex = cortex;
        }

        [HttpGet]
        [Route("/api/test")]
        public IActionResult Test(){
            return Ok(new ApiResult<string>()
            {
                Success = true,
                Data = "Hello World!"
            });
        }

        [HttpGet]
        [Route("/api/process")]
        public IActionResult Process(string text){
            if(!text.IsSet())
                return BadRequest("Text is required");

            this._cortex.AddEntry(text);
            var result = this._cortex.GenerateResponse();
            
            if(!result.Success)
                return this.Error(result.Error);

            return Success(result.Message);
        }

        [HttpGet]
        [Route("/api/network")]
        public IActionResult Network(){
            return Success(this._cortex.GetState());
        }

        private IActionResult Error(string message){
            return StatusCode(500, new ApiResult { Success = false, Error = message });
        }

        private IActionResult Success<T>(T data){
            return Ok(new ApiResult<T> { Success = true, Data = data });
        }

        private IActionResult BadRequest(string message){
            return BadRequest(new ApiResult { Success = false, Error = message });
        }
        
    }
}