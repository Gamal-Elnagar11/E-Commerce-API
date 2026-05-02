using System.Security.Claims;
using E_Commerce_API.DTO.FeedBackDTO;
using E_Commerce_API.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
         public FeedbackController(IFeedbackService feedbackService, IHttpContextAccessor contextAccessor)
        {
            _feedbackService = feedbackService;
         }
 


        [HttpPost(Name ="AddFB")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]
        public async Task<IActionResult> AddFeedback(FeedDTO feeddto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;

                if (userIdClaim == null || usernameClaim == null)
                    return Unauthorized("User not found");


                 await _feedbackService.AddFeedback(feeddto.Rating, userIdClaim, feeddto.Comment, usernameClaim);
                return Ok("Feedback Added Successfuly");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [HttpGet(Name ="GetFB")]
      //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllFeedback()
        {
            var result = await _feedbackService.GetAllFeedback();
            return Ok(result);
        }


        [HttpDelete("{id}",Name ="DeleteFB")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                var result = await _feedbackService.DeleteFeedback(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
