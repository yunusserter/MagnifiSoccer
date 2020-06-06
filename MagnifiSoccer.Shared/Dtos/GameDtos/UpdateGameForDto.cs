﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class UpdateGameForDto
    {
        [Required] public int GameId { get; set; }
        [Required] public string GroupId { get; set; }
        [Required] public DateTime GameDate { get; set; }
        [Required] public string Location { get; set; }
        [Required] public decimal Price { get; set; }
        public string WinnerTeam { get; set; }
    }
}
