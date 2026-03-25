using Microsoft.AspNetCore.Identity;

namespace E_Commerce_API.Models
{
    public class User : IdentityUser
    { 

        //public string? UserName { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
