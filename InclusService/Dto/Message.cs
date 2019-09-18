using System;
using System.Collections.Generic;
using System.Text;

namespace InclusService.Dto
{
    public struct Message
    {
        public Type Type { get; set; }

        public object Data { get; set; }
    }
}
