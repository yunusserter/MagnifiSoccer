using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class EditSquadForDto
    {
        [Required] public int GameId { get; set; }
        [Required] public List<string> Team1Users { get; set; }
        [Required] public List<int> Team1Positions { get; set; }
        [Required] public List<string> Team2Users { get; set; }
        [Required] public List<int> Team2Positions { get; set; }
    }
}
