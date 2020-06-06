using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class InviteResponseForDto
    {
        [Required] public int GameId { get; set; }
        [Required] public bool Response { get; set; }
    }
}
