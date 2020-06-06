using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class RemoveGameForDto
    {
        [Required] public int GameId { get; set; }
        [Required] public string GroupId { get; set; }
    }
}