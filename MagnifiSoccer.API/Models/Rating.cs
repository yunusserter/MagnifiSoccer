using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MagnifiSoccer.API.Models
{
    public class Rating
    {
        [Key] public int Id { get; set; }
        [Required] [StringLength(450)] public string UserId { get; set; }
        [Required] [StringLength(450)] public string VoterUserId { get; set; }
        [Required] [Range(4,10)] public decimal RatingValue { get; set; }
        [Required] public int GameId { get; set; }

        public User User { get; set; }
        public Game Game { get; set; }
    }
}
