using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GroupDtos
{
    public class JoinGroupForDto
    {
        [Required] public string GroupId { get; set; }
        public string Email { get; set; }
    }
}
