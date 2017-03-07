using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileAppServer.Model
{
    public class ResponseService
    {
        public int status { get; set; }
        public string message { get; set; }
        public string entity { get; set; }
    }
}
