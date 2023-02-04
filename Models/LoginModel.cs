using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TheWebApplication.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter Name")]
        [Display(Name = "FirstName")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase      Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }

        public List<LoginModel>? loginModels { get; set; }
    }
}
