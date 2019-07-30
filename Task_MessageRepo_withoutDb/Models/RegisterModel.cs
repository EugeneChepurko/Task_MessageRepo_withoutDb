using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task_MessageRepo_withoutDb.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please, enter password")]
        [Display(Name = "Password must contains three character categories: digits, uppercase, lowercase characters and special characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please, enter password")]
        [Compare("Password", ErrorMessage = "Password is not matched")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}