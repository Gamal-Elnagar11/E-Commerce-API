using AutoMapper;
using E_Commerce_API.DTO.FAQDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FAQController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFAQService _faqService;
        private readonly IMapper _mapper;

        public FAQController(IUnitOfWork unitOfWork, IFAQService fAQService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _faqService = fAQService;
            _mapper = mapper;
        }


        [HttpGet("Quastions")]
        public async Task<IActionResult> GetAllQuastion()
        {
            var result = await _faqService.GetAllQuastion();
            var map = _mapper.Map<List<GetAllQuastionDTO>>(result);

            return Ok(map);
        }

         

        [HttpGet("Answer{id}", Name = "Answer-ID")]
        public async Task<IActionResult> GetAnswerById(int id)
        {
            try
            {
              var result = await _faqService.GetAnswerById(id);
                var map = _mapper.Map<AnswerDTO>(result);
                return Ok(map);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpPost(Name = "AddQ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddFAQ(FAQsDTO faqDTO)
        {
            var result = _mapper.Map<FAQ>(faqDTO);
            var addresult = await _faqService.AddFAQ(result);
            var map = _mapper.Map<FAQsDTO>(addresult);
            return Ok(map);
        }


        [HttpPut(Name ="UpdateQ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateFAQ(FAQtest faq)
        {
            try
            {
                 var updated = await _faqService.UpdateFAQ(faq);
                var map = _mapper.Map<FAQtest>(updated);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("{id}", Name ="DeleteQ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            try
            {
                var result = await _faqService.DeleteFAQ(id);
                var map = _mapper.Map<FAQsDTO>(result);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
