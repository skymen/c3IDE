﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using c3IDE.Models;

namespace c3IDE.Templates
{
    //this class contains a set of template strings used to create snippets of text for multiple files
    public class TemplateHelper
    {
        public static string LanguagePropertyCombo(string id)
        {
            return $@"    ""{id}"" : {{
        ""name"": ""property name"",
        ""desc"": ""property desc"",
        ""items"": {{
            ""item1"": ""item one"",
            ""item2"": ""item two"",
            ""item3"": ""item three"",
        }}
    }}";
        }

        public static string LanguagePropertyLink(string id)
        {
            return $@"    ""{id}"" : {{
        ""name"": ""property name"",
        ""desc"": ""property desc"",
        ""link-text"": ""link text"",
    }}";
        }

        public static string LanguagePropertyDefault(string id)
        {
           return $@"    ""{id}"" : {{
        ""name"": ""property name"",
        ""desc"": ""property desc""
    }}";
        }

        public static string LanguageProperty(string properties)
        {
            return $@"""properties"":{{
{properties}
}}";
        }

        public static string AceParam(string id, string type, string initValue)
        {
            var val = string.IsNullOrWhiteSpace(initValue) ? string.Empty : $",\n            \"initialValue\":{initValue}";
            var items = type == "combo" ? ",\n            \"items\":[\"item1\",\"item2\",\"item3\"]" : string.Empty;
            var objects = type == "object" ? ",\n            \"allowedPluginIds\":[\"Sprite\"]" : string.Empty;
            return $@"    ""params"": [
        {{
            ""id"": ""{id}"",
            ""type"": ""{type}""{val}{items}{objects}
        }},";
        }

        public static string AceLang(string id, string type, string name, string desc)
        {
            var items = type == "combo" ? ",\n            \"items\":{\"item1\":\n                \"item one\",\n                \"item2\": \"item two\",\n                \"item3\": \"item three\"\n            }" : string.Empty;
            return $@"    ""params"": {{
        ""{id}"": {{
            ""name"": ""{name}"",
            ""desc"": ""{desc}""{items}
        }},";
        }

        public static string AceCode(string id, string scriptName)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;
            var param = ti.ToTitleCase(id.Replace("-", " ").ToLower()).Replace(" ", string.Empty);
            param = char.ToLowerInvariant(param[0]) + param.Substring(1);

            return $"{scriptName}({param}, ";
        }

        public static string AceParamFirst(string id, string type, string initValue)
        {
            var val = string.IsNullOrWhiteSpace(initValue) ? string.Empty : $",\n            \"initialValue\":{initValue}";
            var items = type == "combo" ? ",\n            \"items\":[\"item1\",\"item2\",\"item3\"]" : string.Empty;
            var objects = type == "object" ? ",\n            \"allowedPluginIds\":[\"Sprite\"]" : string.Empty;
            return $@"    ""params"": [
        {{
            ""id"": ""{id}"",
            ""type"": ""{type}""{val}{items}{objects}
        }}
    ]
}}";
        }

        public static string AceLangFirst(string id, string type, string name, string desc)
        {
            var items = type == "combo" ? ",\n            \"items\":{\"item1\":\n                \"item one\",\n                \"item2\": \"item two\",\n                \"item3\": \"item three\"\n            }" : string.Empty;
            return $@""",
	""params"": {{
        ""{id}"": {{
            ""name"": ""{name}"",
            ""desc"": ""{desc}""{items}
        }}
    }}
}}";
        }

        public static string AceCodeFirst(string id, string scriptName)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;
            var param = ti.ToTitleCase(id.Replace("-", " ").ToLower()).Replace(" ", string.Empty);
            param = char.ToLowerInvariant(param[0]) + param.Substring(1);

            return $"{scriptName}({param})";
        }

        public static string CndAce(Condition cnd)
        {
            var trigger = cnd.Trigger == "true" ? ",\n	\"isTrigger\": true" : string.Empty;
            var faketrigger = cnd.FakeTrigger == "true" ? ",\n	\"isFakeTrigger\": true" : string.Empty;
            var isstatic = cnd.Static == "true" ? ",\n	\"isStatic\": true" : string.Empty;
            var looping = cnd.Looping == "true" ? ",\n	\"isLooping\": true" : string.Empty;
            var invertible = cnd.Invertible == "false" ? ",\n	\"isInvertible\": false" : string.Empty;
            var triggercompatible = cnd.TriggerCompatible == "false" ? ",\n	\"isCompatibleWithTriggers\": false" : string.Empty;

            return $@"{{
	""id"": ""{cnd.Id}"",
	""scriptName"": ""{cnd.ScriptName}"",
	""highlight"": {cnd.Highlight}{trigger}{faketrigger}{isstatic}{looping}{invertible}{triggercompatible}
}}";
        }

        public static string ExpAces(Expression exp)
        {
            var isvariadic = exp.IsVariadicParameters == "true" ? ",\n	\"isVariadicParameters\": true" : string.Empty;

            return $@"{{
	""id"": ""{exp.Id}"",
	""expressionName"": ""{exp.ScriptName}"",
	""returnType"": {exp.ReturnType}{isvariadic}
}}";
        }
    }
}