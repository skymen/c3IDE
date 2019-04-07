﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c3IDE.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace c3IDE.Tests
{
    [TestClass]
    public class C2ServiceRequestTest
    {
        [TestMethod]
        public void TestActionParseHappyPath()
        {
            var text = File.ReadAllText("TestFiles\\stack_edittime.js");
            
            C2ParsingService.Insatnce.Execute(text, "stack");
        }
    }
}