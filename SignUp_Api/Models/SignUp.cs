using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Bogus;

namespace SignUp_Api.Models
{
    public class SignUp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage ="UserName is Must!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "PassWord is Must!")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Use atleast 8 charecters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is Must!")]
        [RegularExpression(@"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$", ErrorMessage = "Invalid email format.")]
        [EmailAddress]
        public string Email { get; set; }
        public string UserStatus { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string SignInStatus { get; set; }
    }


    
}
