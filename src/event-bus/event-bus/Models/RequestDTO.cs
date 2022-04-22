﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_bus.Models
{
    class RequestDTO
    {
        public RequestDTO(string origin, string message, string target)
        {
            Origin = origin;
            Message = message;
            Date = DateTime.Now;
            Target = target;

        }

        //do not mention ID, it is used for database
        public int ID { get; set; }
        //the origin (microservice)
        public string Origin { get; set; }
        //channel of nats
        public string Target { get; set; }
        //do not mention, gotten automatically
        public DateTime Date { get; set; }
        //Json string
        public string Message { get; set; }

    }
}
