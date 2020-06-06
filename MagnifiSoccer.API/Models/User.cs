using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MagnifiSoccer.API.Models
{
    public class User : IdentityUser
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public decimal Remainder { get; set; }
        public decimal OverAllRating { get; set; }

        public ICollection<Rating> Ratings { get; set; }
    }
}
