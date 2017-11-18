using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Db.Models
{
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string VAT { get; set; }
        public ApplicationUser UserId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string FirstName { get; set; }
        public ApplicationUser DisplayName { get; set; }
        public string FontAwesomeIcon { get; set; }
        public string AvatarURL { get; set; }
        public string URLPath { get; set; }
        public string ShortDesc { get; set; }
        public string TimeSpan { get; set; }
        public int Percentage { get; set; }
        public string Type { get; set; }
    }
}
