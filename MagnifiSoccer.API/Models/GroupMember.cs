using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MagnifiSoccer.API.Models
{
    [Table("GroupMember")]
    public class GroupMember
    {
        [Key] public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GroupId { get; set; }
        public string Role { get; set; }

        public Group Group { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
