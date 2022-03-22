using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Models
{
    internal class SuccessfulRequests
    {
        public int ID { get; set; }
        public RequestDTO RequestDTO { get; set; }
        public string Channel { get; set; }
        public DateTime DateTime { get; set; }
    }
}
