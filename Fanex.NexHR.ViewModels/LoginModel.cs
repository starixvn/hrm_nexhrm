namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name ="User name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings =false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
