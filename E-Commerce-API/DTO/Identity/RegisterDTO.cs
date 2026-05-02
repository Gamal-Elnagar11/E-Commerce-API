using System.ComponentModel.DataAnnotations;

namespace E_Commerce_API.DTO.Identity
{
    public class RegisterDTO
    {

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }


        [Required(ErrorMessage = "DateOfBirth is required")]
         public DateOnly DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }
         
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [Compare(nameof(Password),ErrorMessage = "this Password and Confirmation Password Dont Match!!")]
        public string ConfirmPassword { get; set; }


         

    }
}
