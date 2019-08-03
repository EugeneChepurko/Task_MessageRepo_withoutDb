using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace Task_MessageRepo_withoutDb.Models
{
    [JsonObject(IsReference = false)]
    public class ApplicationUser : IdentityUser
    {
        public string LastMessage { get; set; }
    }
}