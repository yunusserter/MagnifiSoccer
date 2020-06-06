using System.ComponentModel.DataAnnotations;

namespace MagnifiSoccer.Shared.Dtos.AuthDtos
{
    public class CreateRoleForDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}
