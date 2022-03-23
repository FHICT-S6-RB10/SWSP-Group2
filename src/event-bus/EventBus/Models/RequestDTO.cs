using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class RequestDTO
    {
        public RequestDTO(string origin, string message)
        {
            this.Origin = origin;
            this.Message = message;
            this.Date = DateTime.Now;
             
        }
        public int ID { get; set; }
        public string Origin { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        
    }
}
