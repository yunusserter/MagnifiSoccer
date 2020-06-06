using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MagnifiSoccer.API.Models
{
    [Table("GamePlayer")]
    public class GamePlayer
    {
        [Key] public int Id { get; set; }
        public int GameId { get; set; }
        [StringLength(450)] public string UserId { get; set; }
        public int Position { get; set; }
        public string Team { get; set; }
        public bool? Attended { get; set; }
        public decimal GameRating { get; set; }
        public bool? InviteResponse { get; set; }

        public Game Game { get; set; }
        public User User { get; set; }
    }
}
