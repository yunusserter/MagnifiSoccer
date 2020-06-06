using System.ComponentModel.DataAnnotations;

namespace MagnifiSoccer.Shared.Dtos.GroupDtos
{
    public class RemoveGroupForDto
    {
        [Required] public string GroupId { get; set; }
    }
}
