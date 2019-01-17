﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteDB;

namespace c3IDE.Models
{
    public class Options
    {
        [BsonId]
        public Guid Key { get; set; } = Guid.Parse("e0cddcac-e99d-4338-ac91-b56b0db58ed0");
        public string DataPath { get; set; }
        public string CompilePath { get; set; }
        public string ExportPath { get; set; }
        public string FontSize { get; set; }
        public string FontFamily { get; set; }

        //todo: add default author/coompany for the dashboard
        //todo: description from dashboad not going to addon.js
    }
}
