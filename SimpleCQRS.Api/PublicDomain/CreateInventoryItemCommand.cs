using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleCQRS.Api.PublicDomain
{
   

    public class CreateInventoryItem : Command
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }
}