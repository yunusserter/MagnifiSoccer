using System.ComponentModel.DataAnnotations;

namespace MagnifiSoccer.Shared.Dtos.GroupDtos
{
    public class KickMemberGroupForDto
    {
        [Required] public string GroupId { get; set; }
        [Required] public string UserId { get; set; }
    }
}
