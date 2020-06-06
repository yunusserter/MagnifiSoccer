using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MagnifiSoccer.API.Models
{
    [Table("Group")]
    public class Group
    {
        [Key] public string Id { get; set; }
        [Required] public string GroupName { get; set; }
        public string PhotoUrl { get; set; }

        public ICollection<GroupMember> GroupMembers { get; set; }
        public ICollection<Game> Games { get; set; }
    }
}
