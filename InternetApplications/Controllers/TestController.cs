using AOP.Examples;
using Microsoft.AspNetCore.Mvc;

namespace InternetApplications.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController(ITestService testService) : ControllerBase
    {
        [HttpGet("fast")]
        public async Task<IActionResult> TestFast()
        {
            var result = await testService.FastMethodAsync();
            return Ok(new { message = result, timestamp = DateTime.UtcNow });
        }

        [HttpGet("slow")]
        public async Task<IActionResult> TestSlow([FromQuery] int delay = 1500)
        {
            var result = await testService.SlowMethodAsync(delay);
            return Ok(new { message = result, delay = delay, timestamp = DateTime.UtcNow });
        }

        [HttpGet("error")]
        public async Task<IActionResult> TestError()
        {
            try
            {
                await testService.MethodThatThrowsAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, type = ex.GetType().Name });
            }
        }
    }
}

