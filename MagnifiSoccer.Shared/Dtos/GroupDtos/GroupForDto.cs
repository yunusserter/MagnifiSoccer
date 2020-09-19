using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MagnifiSoccer.Shared.Dtos.GroupDtos
{
    public class GroupForDto
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public IFormFile File { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
    }
}
