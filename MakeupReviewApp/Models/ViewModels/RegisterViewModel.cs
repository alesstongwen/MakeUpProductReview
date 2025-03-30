using System.ComponentModel.DataAnnotations;

namespace MakeupReviewApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
