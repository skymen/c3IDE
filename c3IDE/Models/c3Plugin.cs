﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace c3IDE.Models
{
    public class C3Plugin
    {
        [BsonId]
        public Guid Id { get; set; }

        public Plugin Plugin { get; set; }

        public Aces Aces { get; set; }

        public EditTimeType EditTimeType { get; set; }

        public EditTimeInstance EditTimeInstance { get; set; }
    }
}
