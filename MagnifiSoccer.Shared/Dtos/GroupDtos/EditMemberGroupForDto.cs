using System.ComponentModel.DataAnnotations;

namespace MagnifiSoccer.Shared.Dtos.GroupDtos
{
    public class EditMemberGroupForDto
    {
        [Required] public string GroupId { get; set; }
        [Required] public string UserId { get; set; }
        [Required] public string Role { get; set; }
    }
}
