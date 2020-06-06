using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MagnifiSoccer.Shared.Dtos.GameDtos
{
    public class AutoSquadForDto
    {
        [Required] public List<string> GroupId { get; set; }
        [Required] public int GameId { get; set; }
        [Required] [Range(8,24)] public int Capacity { get; set; }
    }
}
