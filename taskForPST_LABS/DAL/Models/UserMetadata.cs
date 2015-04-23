using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.ModelBinding;

namespace taskForPST_LABS.DAL.Models
{
    public class UserMetadata
    {
        public int? UserId { get; set; }
        [DisplayName("Логин")]
        [Required]
        [Index(IsUnique = true)] 
        [StringLength(20,MinimumLength=3,ErrorMessage="Имя должно быть от 3 до 20 символов")]
        public string Username { get; set; }

        [DisplayName("Пароль")]
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Пароль должнен быть от 5 до 20 символов")]
        public string Password { get; set; }

        [DisplayName("Имя")]
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 20 символов")]
        public string Name { get; set; }

        [DisplayName("Фамилия")]
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Фамилия должна быть от 2 до 20 символов")]
        public string Surname { get; set; }

        [DisplayName("Отчество")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Отчество должно быть от 2 до 25 символов")]
        public string Lastname { get; set; }

        [DisplayName("Email")]
        [Required]
        [Index("IX_Users_Column",IsUnique = true)] 
        [EmailAddress(ErrorMessage = "Введите правильный e-mail")]
        [StringLength(150, MinimumLength = 6, ErrorMessage = "Email должен быть от 6 символов")]
        public string Email { get; set; }
    }
}