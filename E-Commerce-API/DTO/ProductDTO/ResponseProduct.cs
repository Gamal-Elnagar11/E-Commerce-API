using E_Commerce_API.DTO.CategoryDTO;

namespace E_Commerce_API.DTO.ProductDTO
{
    public class ResponseProduct
    { 
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public int Stock { get; set; }
            public CategoriesDTO Category { get; set; }
 
     }
}
