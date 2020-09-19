using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class ResultForDto
    {
        [Required] public string WinnerTeam { get; set; }
        public List<string> Players { get; set; }
        public List<bool> Attendees { get; set; }
        [Required] public int GameId { get; set; }

    }
}
