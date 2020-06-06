using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class PlayerRatingForDto
    {
        [Required] public int GameId { get; set; }
        [Required] public List<string> UserId { get; set; }
        [Required] public List<decimal> Rating { get; set; }
    }
}
