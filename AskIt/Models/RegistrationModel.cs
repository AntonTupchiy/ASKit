using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskIt.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Please provide username", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please provide Password", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be 8 char long.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password dose not match.")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please provide full name", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please provide Surname", AllowEmptyStrings = false)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Please provide knowledge", AllowEmptyStrings = false)]
        public string Knowledge { get; set; }

        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",
            ErrorMessage = "Please provide valid email id")]
        public string EmailID { get; set; }

    }
}