using System;
using System.Collections.Generic;
using System.Text;

namespace InclusService.Dto
{
   public struct Connection
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public string HostName { get; set; }
    }
}
