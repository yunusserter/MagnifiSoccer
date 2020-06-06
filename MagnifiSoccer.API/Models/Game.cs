using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagnifiSoccer.API.Models
{
    [Table("Game")]
    public class Game
    {
        [Key]
        public int Id { get; set; }

        public string GroupId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string WinnerTeam { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? GameDate { get; set; }

        public string Location { get; set; }

        [Column(TypeName = "Decimal(16,2)")]
        public decimal? Price { get; set; }

        public Group Group { get; set; }
        public ICollection<GamePlayer> GamePlayers { get; set; }
    }
}
