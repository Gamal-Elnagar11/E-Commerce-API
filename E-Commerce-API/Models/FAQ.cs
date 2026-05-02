using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Models
{
    public class FAQ
    {
        public int Id { get; set; }
         public string Question { get; set; }
        public string Answer { get; set; }


      
        public User? User { get; set; }

    }
}
