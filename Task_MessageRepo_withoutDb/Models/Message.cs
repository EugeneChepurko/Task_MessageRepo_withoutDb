using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task_MessageRepo_withoutDb.Models
{
    [JsonObject(IsReference = false)]
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Mess { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateTime { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}