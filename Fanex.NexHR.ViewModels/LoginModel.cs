namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}