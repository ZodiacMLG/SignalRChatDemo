using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Models
{
    public class UserBase
    {
        [Required]
        public string Name {  get; set; }
        [Required]
        public string Password { get; set; }
    }
}
