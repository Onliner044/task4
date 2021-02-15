using System.ComponentModel.DataAnnotations;

namespace task4.ViewModel {
    public class LoginViewModel {
        [Required(ErrorMessage = "The email is not specified")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is not specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
