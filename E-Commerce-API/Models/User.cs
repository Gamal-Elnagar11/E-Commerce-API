using Microsoft.AspNetCore.Identity;

namespace E_Commerce_API.Models
{
    public class User : IdentityUser
    { 

         public bool IsDeleted { get; set; } = false;

        public string FullName { get; set; }
        public string Country {  get; set; }
        public DateOnly DateOfBirth { get; set; }

    }
}
