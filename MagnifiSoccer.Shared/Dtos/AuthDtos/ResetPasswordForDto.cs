using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.AuthDtos
{
    public class ResetPasswordForDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
    }
}
