using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_API.DTO.ProductDTO
{
    public class UpdateProductDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        //public string ImageUrl { get; set; }


        [Required]
        public int Stock { get; set; }



        public int CategoryId { get; set; }

    }
}
