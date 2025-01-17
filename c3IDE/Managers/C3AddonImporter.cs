﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using c3IDE.Models;
using c3IDE.Templates;
using c3IDE.Templates.c3IDE.Templates;
using c3IDE.Utilities;
using c3IDE.Utilities.Helpers;
using Esprima.Ast;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Action = c3IDE.Models.Action;
using Expression = c3IDE.Models.Expression;

namespace c3IDE.Managers
{
    public class C3AddonImporter : Singleton<C3AddonImporter>
    {
        public async Task<C3Addon> Import(string path)
        {
            WindowManager.ShowLoadingOverlay(true);
            try
            {
                return await Task.Run(() =>
                {
                    var fi = new FileInfo(path);
                    var tmpPath = OptionsManager.CurrentOptions.DataPath + "\\tmp_c3";
                    if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

                    //unzip c3addon to temp location
                    ZipFile.ExtractToDirectory(path, tmpPath);

                    var addon = JObject.Parse(File.ReadAllText(Path.Combine(tmpPath, "addon.json")));
                    string type = addon["type"].ToString();
                    string id = addon["id"].ToString();

                    //todo: handle mixed c3addon with only c2runtime
                    bool c2Only = !File.Exists(Path.Combine(tmpPath, "c3runtime", $"{type}.js"));


                    if (type != "effect" && !c2Only)
                    {
                        string pluginEdit, pluginRun;

                        pluginEdit = File.ReadAllText(Path.Combine(tmpPath, $"{type}.js"));
                        pluginRun = File.ReadAllText(Path.Combine(tmpPath, "c3runtime", $"{type}.js"));
                        string typeEdit = File.ReadAllText(Path.Combine(tmpPath, $"type.js"));
                        string typeRun = File.ReadAllText(Path.Combine(tmpPath, "c3runtime", $"type.js"));
                        string instanceEdit = File.ReadAllText(Path.Combine(tmpPath, $"instance.js"));
                        string instanceRun = File.ReadAllText(Path.Combine(tmpPath, "c3runtime", $"instance.js"));
                        string c2runtime = null;

                        if (Directory.Exists(Path.Combine(tmpPath, "c2runtime")))
                        {
                            c2runtime = File.ReadAllText(Path.Combine(tmpPath, "c2runtime", "runtime.js"));
                        }

                        PluginType pluginType = PluginType.SingleGlobalPlugin;
                        string pluginCat = "other";
                        switch (type)
                        {
                            case "plugin":
                                pluginType = pluginEdit.Contains("SetPluginType(\"world\")")
                                    ? PluginType.DrawingPlugin
                                    : PluginType.SingleGlobalPlugin;
                                pluginCat = Regex.Match(pluginEdit, @"PLUGIN_CATEGORY = ""(?<cat>).*""").Groups["cat"]
                                    .Value;
                                break;
                            case "behavior":
                                pluginType = PluginType.Behavior;
                                pluginCat = Regex.Match(pluginEdit, @"BEHAVIOR_CATEGORY = ""(?<cat>.*)""").Groups["cat"]
                                    .Value;
                                break;
                        }

                        if (string.IsNullOrWhiteSpace(pluginCat)) pluginCat = "other";

                        var ace = JObject.Parse(File.ReadAllText(Path.Combine(tmpPath, "aces.json")));
                        var lang = JObject.Parse(File.ReadAllText(Path.Combine(tmpPath, "lang", "en-US.json")))["text"][type + "s"][id.ToLower()];

                        var prop = "\"properties\": " + (string.IsNullOrWhiteSpace(lang["properties"]?.ToString()) ? "{ }" : lang["properties"]);
                        var cats = "\"aceCategories\": " + (string.IsNullOrWhiteSpace(lang["aceCategories"]?.ToString()) ? "{ }" : lang["aceCategories"]);

                        //pasre ace implementations
                        LogManager.AddImportLogMessage("EXTRACTING C3RUNTIME / ACTIONS");
                        var actFuncs = JavascriptManager.GetAllFunction(File.ReadAllText(Path.Combine(tmpPath, "c3runtime", "actions.js")));
                        LogManager.AddImportLogMessage("EXTRACTING C3RUNTIME / CONDITION");
                        var cndFuncs = JavascriptManager.GetAllFunction(File.ReadAllText(Path.Combine(tmpPath, "c3runtime", "conditions.js")));
                        LogManager.AddImportLogMessage("EXTRACTING C3RUNTIME / EXPRESSIONS");
                        var expFuncs = JavascriptManager.GetAllFunction(File.ReadAllText(Path.Combine(tmpPath, "c3runtime", "expressions.js")));

                        var actionList = new List<Models.Action>();
                        var conditionList = new List<Models.Condition>();
                        var expressionList = new List<Models.Expression>();

                        foreach (JProperty category in ace.Properties())
                        {
                            //parse actions
                            var ationJson = ace[category.Name]["actions"]?.ToString();
                            var actions = ationJson != null ? JArray.Parse(ationJson) : null;
                            if (actions != null)
                            {
                                foreach (var action in actions.Children<JObject>())
                                {
                                    var actionId = action["id"].ToString();
                                    var actionAce = action.ToString();
                                    var actionLang = $"\"{actionId}\":" + lang["actions"][actionId];
                                    var actionScript = action["scriptName"].ToString();
                                    var actionParams = string.Empty;

                                    //only needed for stub methods
                                    //if (action["params"] != null && action["params"].Children<JObject>().Any())
                                    //{
                                    //    var ep = action["params"].Children<JObject>().Select(x => x["id"].ToString());
                                    //    actionParams = string.Join(",", ep);
                                    //}

                                    actFuncs.TryGetValue(actionScript.Trim(), out var code);
                                    if (code == null)
                                    {
                                        LogManager.AddImportLogMessage($"ACTION FUNCTION DEFINITION DOES NOT EXISTS => {actionScript.Trim()}");
                                        continue;
                                    }

                                    var act = new Models.Action
                                    {
                                        Id = actionId,
                                        Category = category.Name,
                                        Ace = actionAce,
                                        Language = actionLang,
                                        //Code = $"{actionScript}({string.Join(",", actionParams)}) {{ \n}}"
                                        Code = FormatHelper.Insatnce.Javascript(code) ?? string.Empty
                                    };

                                    actionList.Add(act);
                                }
                            }

                            //parse conditions
                            var conditionJson = ace[category.Name]["conditions"]?.ToString();
                            var conditions = conditionJson != null ? JArray.Parse(conditionJson) : null;
                            if (conditions != null)
                            {
                                foreach (var condition in conditions.Children<JObject>())
                                {
                                    var conditionId = condition["id"].ToString();
                                    var conditionAce = condition.ToString();
                                    var conditionLang = $"\"{conditionId}\":" + lang["conditions"][conditionId];
                                    var conditionScript = condition["scriptName"].ToString();
                                    var conditionParams = string.Empty;

                                    //only needed for stub methods
                                    //if (condition["params"] != null && condition["params"].Children<JObject>().Any())
                                    //{
                                    //    var ep = condition["params"].Children<JObject>().Select(x => x["id"].ToString());
                                    //    conditionParams = string.Join(",", ep);
                                    //}

                                    cndFuncs.TryGetValue(conditionScript.Trim(), out var code);
                                    if (code == null)
                                    {
                                        LogManager.AddImportLogMessage($"CONDITION FUNCTION DEFINITION DOES NOT EXISTS => {conditionScript.Trim()}");
                                        continue;
                                    }
                                    var cnd = new Models.Condition()
                                    {
                                        Id = conditionId,
                                        Category = category.Name,
                                        Ace = conditionAce,
                                        Language = conditionLang,
                                        //Code = $"{conditionScript}({string.Join(",", conditionParams)}) {{ \n}}"
                                        Code = FormatHelper.Insatnce.Javascript(code) ?? string.Empty
                                    };

                                    conditionList.Add(cnd);
                                }
                            }

                            //parse expression
                            var expressionJson = ace[category.Name]["expressions"]?.ToString();
                            var expressions = expressionJson != null ? JArray.Parse(expressionJson) : null;
                            if (expressions != null)
                            {
                                foreach (var expression in expressions.Children<JObject>())
                                {
                                    var expressionId = expression["id"].ToString();
                                    var expressionAce = expression.ToString();
                                    var expressionLang = $"\"{expressionId}\":" + lang["expressions"][expressionId];
                                    var expressionScript = expression["expressionName"].ToString();
                                    var expressionParams = string.Empty;

                                    //only needed for stub methods
                                    //if (expression["params"] != null && expression["params"].Children<JObject>().Any())
                                    //{
                                    //    var ep = expression["params"].Children<JObject>().Select(x => x["id"].ToString());
                                    //    expressionParams = string.Join(",", ep);
                                    //}

                                    expFuncs.TryGetValue(expressionScript.Trim(), out var code);
                                    if (code == null)
                                    {
                                        LogManager.AddImportLogMessage($"EXPRESSION FUNCTION DEFINITION DOES NOT EXISTS => {expressionScript.Trim()}");
                                        continue;
                                    }
                                    var exp = new Models.Expression()
                                    {
                                        Id = expressionId,
                                        Category = category.Name,
                                        Ace = expressionAce,
                                        Language = expressionLang,
                                        //Code = $"{expressionScript}({expressionParams}) {{ \n}}"
                                        Code = FormatHelper.Insatnce.Javascript(expFuncs[expressionScript.Trim()]) ??
                                               string.Empty
                                    };

                                    expressionList.Add(exp);
                                }
                            }
                        }

                        var thirdPartyFiles = new List<ThirdPartyFile>();
                        var files = Regex.Matches(pluginEdit, @"filename\s?:\s?(""|')(?<file>.*)(""|')");
                        var domFilesMatches = Regex.Matches(pluginEdit, @"SetDOMSideScripts\(\[(?<file>.*)\]\)");
                        var domFileList = new List<string>();
                        var completeFileList = new HashSet<string>();

                        foreach(Match match in domFilesMatches)
                        {
                            var domScripts = match.Groups["file"].ToString().Split(',');
                            foreach(var domScript in domScripts)
                            {
                                var fn = domScript.Trim('"').Trim('\'');
                                domFileList.Add(fn);
                                completeFileList.Add(fn);
                            }
                        }

                        foreach(Match match in files)
                        {
                            var fn = match.Groups["file"].ToString();
                            completeFileList.Add(fn);
                        }

                        foreach (var fn in completeFileList)
                        {
                            var info = new FileInfo(Path.Combine(Path.Combine(tmpPath, fn)));

                            var f = new ThirdPartyFile
                            {
                                Bytes = File.ReadAllBytes(info.FullName),
                                Content = File.ReadAllText(info.FullName),
                                Extention = info.Extension,
                            };

                            switch (info.Extension)
                            {
                                case ".js":
                                    f.Content = FormatHelper.Insatnce.FixMinifiedFiles(f.Content);
                                    f.Compress = true;
                                    f.PlainText = true;
                                    break;
                                case ".html":
                                case ".css":
                                case ".txt":
                                case ".json":
                                case ".xml":
                                    f.PlainText = true;
                                    f.Compress= false;
                                    break;
                                default:
                                    f.Content = $"BINARY FILE => {f.FileName}\nBYTE LENGTH : ({f.Bytes.Length})";
                                    f.PlainText = false;
                                    f.Compress = false;
                                    break;
                            }


                            if (fn.Contains("c3runtime"))
                            {
                                f.C3Folder = true;
                                f.FileName = fn.Replace("c3runtime/", string.Empty).Trim();
                            }
                            else if (fn.Contains("c2runtime"))
                            {
                                f.C2Folder = true;
                                f.FileName = fn.Replace("c2runtime/", string.Empty).Trim();
                            }
                            else
                            {
                                f.Rootfolder = true;
                                f.FileName = fn.Replace("/", "\\").Trim();
                            }

                            foreach(var df in domFileList)
                            {
                                if(df.Contains(f.FileName))
                                {
                                    f.Domfolder = true;
                                }
                            }

                            f.PluginTemplate = TemplateHelper.ThirdPartyFile(f);

                            thirdPartyFiles.Add(f);
                        }

                        //todo: create c3addon, and map parsed data to c3addon 
                        var c3addon = new C3Addon
                        {
                            AddonId = id,
                            AddonCategory = pluginCat,
                            Author = addon["author"]?.ToString(),
                            Class = addon["name"]?.ToString()?.Replace(" ", string.Empty),
                            Company = addon["author"]?.ToString(),
                            Name = addon["name"]?.ToString(),
                            Description = addon["description"]?.ToString(),
                            AddonJson = addon.ToString(),
                            PluginRunTime = pluginRun,
                            PluginEditTime = pluginEdit,
                            TypeEditTime = typeEdit,
                            TypeRunTime = typeRun,
                            InstanceEditTime = instanceEdit,
                            InstanceRunTime = instanceRun,
                            LanguageProperties = prop,
                            LanguageCategories = cats,
                            Id = Guid.NewGuid(),
                            CreateDate = DateTime.Now,
                            LastModified = DateTime.Now,
                            Type = pluginType
                        };

                        c3addon.Actions = new Dictionary<string, Action>();
                        c3addon.Conditions = new Dictionary<string, Condition>();
                        c3addon.Expressions = new Dictionary<string, Expression>();

                        foreach (var action in actionList)
                        {
                            c3addon.Actions.Add(action.Id, action);
                        }

                        foreach (var condition in conditionList)
                        {
                            c3addon.Conditions.Add(condition.Id, condition);
                        }

                        foreach (var expression in expressionList)
                        {
                            c3addon.Expressions.Add(expression.Id, expression);
                        }

                        c3addon.IconXml = File.Exists(Path.Combine(tmpPath, "icon.svg")) ?
                            File.ReadAllText(Path.Combine(tmpPath, "icon.svg")) :
                            ResourceReader.Insatnce.GetResourceText("c3IDE.Templates.Files.icon.svg");

                        c3addon.Template = TemplateFactory.Insatnce.CreateTemplate(c3addon.Type);

                        c3addon.ThirdPartyFiles = new Dictionary<string, ThirdPartyFile>();
                        foreach (var thirdPartyFile in thirdPartyFiles)
                        {
                            c3addon.ThirdPartyFiles.Add($"{thirdPartyFile.ID}", thirdPartyFile);
                        }

                        if (!string.IsNullOrWhiteSpace(c2runtime))
                        {
                            c3addon.C2RunTime = c2runtime;
                        }

                        //regenerate addon file
                        c3addon.AddonJson = TemplateCompiler.Insatnce.CompileTemplates(c3addon.Template.AddonJson, c3addon);

                        //fixup pluginedit time (replace png iconw ith svg)
                        c3addon.PluginEditTime = Regex.Replace(c3addon.PluginEditTime, @"this._info.SetIcon\(.*\);", "this._info.SetIcon(\"icon.svg\", \"image/svg+xml\");");

                        return c3addon;
                    }
                    else if (type != "effect" && c2Only)
                    {
                        string pluginEdit;

                        pluginEdit = File.ReadAllText(Path.Combine(tmpPath, $"{type}.js"));
                        string typeEdit = File.ReadAllText(Path.Combine(tmpPath, $"type.js"));
                        string instanceEdit = File.ReadAllText(Path.Combine(tmpPath, $"instance.js"));

                        string c2runtime = null;

                        if (Directory.Exists(Path.Combine(tmpPath, "c2runtime")))
                        {
                            c2runtime = File.ReadAllText(Path.Combine(tmpPath, "c2runtime", "runtime.js"));
                        }

                        PluginType pluginType = PluginType.SingleGlobalPlugin;
                        string pluginCat = "other";
                        switch (type)
                        {
                            case "plugin":
                                pluginType = pluginEdit.Contains("SetPluginType(\"world\")")
                                    ? PluginType.DrawingPlugin
                                    : PluginType.SingleGlobalPlugin;
                                pluginCat = Regex.Match(pluginEdit, @"PLUGIN_CATEGORY = ""(?<cat>).*""").Groups["cat"]
                                    .Value;
                                break;
                            case "behavior":
                                pluginType = PluginType.Behavior;
                                pluginCat = Regex.Match(pluginEdit, @"BEHAVIOR_CATEGORY = ""(?<cat>.*)""").Groups["cat"]
                                    .Value;
                                break;
                        }

                        if (string.IsNullOrWhiteSpace(pluginCat)) pluginCat = "other";

                        var ace = JObject.Parse(File.ReadAllText(Path.Combine(tmpPath, "aces.json")));
                        var lang = JObject.Parse(File.ReadAllText(Path.Combine(tmpPath, "lang", "en-US.json")))["text"][type + "s"][id.ToLower()];

                        var prop = "\"properties\": " + (string.IsNullOrWhiteSpace(lang["properties"]?.ToString()) ? "{ }" : lang["properties"]);
                        var cats = "\"aceCategories\": " + (string.IsNullOrWhiteSpace(lang["aceCategories"]?.ToString()) ? "{ }" : lang["aceCategories"]);

                        var actionList = new List<Models.Action>();
                        var conditionList = new List<Models.Condition>();
                        var expressionList = new List<Models.Expression>();

                        foreach (JProperty category in ace.Properties())
                        {
                            //parse actions
                            var ationJson = ace[category.Name]["actions"]?.ToString();
                            var actions = ationJson != null ? JArray.Parse(ationJson) : null;
                            if (actions != null)
                            {
                                foreach (var action in actions.Children<JObject>())
                                {
                                    var actionId = action["id"].ToString();
                                    var actionAce = action.ToString();
                                    var actionLang = $"\"{actionId}\":" + lang["actions"][actionId];
                                    var actionScript = action["scriptName"].ToString();
                                    var actionParams = string.Empty;

                                    //only needed for stub methods
                                    if (action["params"] != null && action["params"].Children<JObject>().Any())
                                    {
                                        var ep = action["params"].Children<JObject>().Select(x =>
                                        {
                                            var p = x["id"].ToString();
                                            var ti = new CultureInfo("en-US", false).TextInfo;
                                            var param = ti.ToTitleCase(p.Replace("-", " ").ToLower()).Replace(" ", string.Empty);
                                            param = char.ToLowerInvariant(param[0]) + param.Substring(1);
                                            return param;
                                        });
                                        actionParams = string.Join(",", ep);
                                    }

                                    var act = new Models.Action
                                    {
                                        Id = actionId,
                                        Category = category.Name,
                                        Ace = actionAce,
                                        Language = actionLang,
                                        Code = $"{actionScript}({string.Join(",", actionParams)}) {{ \n}}"
                                    };

                                    actionList.Add(act);
                                }
                            }

                            //parse conditions
                            var conditionJson = ace[category.Name]["conditions"]?.ToString();
                            var conditions = conditionJson != null ? JArray.Parse(conditionJson) : null;
                            if (conditions != null)
                            {
                                foreach (var condition in conditions.Children<JObject>())
                                {
                                    var conditionId = condition["id"].ToString();
                                    var conditionAce = condition.ToString();
                                    var conditionLang = $"\"{conditionId}\":" + lang["conditions"][conditionId];
                                    var conditionScript = condition["scriptName"].ToString();
                                    var conditionParams = string.Empty;

                                    //only needed for stub methods
                                    if (condition["params"] != null && condition["params"].Children<JObject>().Any())
                                    {
                                        var ep = condition["params"].Children<JObject>().Select(x =>
                                        {
                                            var p = x["id"].ToString();
                                            var ti = new CultureInfo("en-US", false).TextInfo;
                                            var param = ti.ToTitleCase(p.Replace("-", " ").ToLower()).Replace(" ", string.Empty);
                                            param = char.ToLowerInvariant(param[0]) + param.Substring(1);
                                            return param;
                                        });
                                        conditionParams = string.Join(",", ep);
                                    }


                                    var cnd = new Models.Condition()
                                    {
                                        Id = conditionId,
                                        Category = category.Name,
                                        Ace = conditionAce,
                                        Language = conditionLang,
                                        Code = $"{conditionScript}({string.Join(",", conditionParams)}) {{ \n}}"
                                    };

                                    conditionList.Add(cnd);
                                }
                            }

                            //parse expression
                            var expressionJson = ace[category.Name]["expressions"]?.ToString();
                            var expressions = expressionJson != null ? JArray.Parse(expressionJson) : null;
                            if (expressions != null)
                            {
                                foreach (var expression in expressions.Children<JObject>())
                                {
                                    var expressionId = expression["id"].ToString();
                                    var expressionAce = expression.ToString();
                                    var expressionLang = $"\"{expressionId}\":" + lang["expressions"][expressionId];
                                    var expressionScript = expression["expressionName"].ToString();
                                    var expressionParams = string.Empty;

                                    //only needed for stub methods
                                    if (expression["params"] != null && expression["params"].Children<JObject>().Any())
                                    {
                                        var ep = expression["params"].Children<JObject>().Select(x =>
                                        {
                                            var p = x["id"].ToString();
                                            var ti = new CultureInfo("en-US", false).TextInfo;
                                            var param = ti.ToTitleCase(p.Replace("-", " ").ToLower()).Replace(" ", string.Empty);
                                            param = char.ToLowerInvariant(param[0]) + param.Substring(1);
                                            return param;
                                        });
                                        expressionParams = string.Join(",", ep);
                                    }

                                    var exp = new Models.Expression()
                                    {
                                        Id = expressionId,
                                        Category = category.Name,
                                        Ace = expressionAce,
                                        Language = expressionLang,
                                        Code = $"{expressionScript}({expressionParams}) {{ \n}}"
                                    };

                                    expressionList.Add(exp);
                                }
                            }
                        }

                        var files = Regex.Matches(pluginEdit, @"filename\s?:\s?(""|')(?<file>.*)(""|')");
                        var thirdPartyFiles = new List<ThirdPartyFile>();
                        foreach (Match match in files)
                        {
                            var fn = match.Groups["file"].ToString();
                            var info = new FileInfo(Path.Combine(Path.Combine(tmpPath, fn)));

                            var f = new ThirdPartyFile
                            {
                                Bytes = null,
                                Content = File.ReadAllText(info.FullName),
                                Extention = info.Extension,
                            };

                            f.PluginTemplate = TemplateHelper.ThirdPartyFile(f);

                            switch (info.Extension)
                            {
                                case ".js":
                                    f.Content = FormatHelper.Insatnce.FixMinifiedFiles(f.Content);
                                    f.Compress = true;
                                    f.PlainText = true;
                                    break;
                                case ".html":
                                case ".css":
                                case ".txt":
                                case ".json":
                                case ".xml":
                                    f.PlainText = true;
                                    f.Compress = false;
                                    break;
                                default:
                                    f.Content = $"BINARY FILE => {f.FileName}\nBYTE LENGTH : ({f.Bytes.Length})";
                                    f.PlainText = false;
                                    f.Compress = false;
                                    break;
                            }

                            if (fn.Contains("c3runtime"))
                            {
                                f.C3Folder = true;
                                f.FileName = fn.Replace("c3runtime/", string.Empty).Trim();
                            }
                            else if (fn.Contains("c2runtime"))
                            {
                                f.C2Folder = true;
                                f.FileName = fn.Replace("c2runtime/", string.Empty).Trim();
                            }
                            else
                            {
                                f.Rootfolder = true;
                                f.FileName = fn.Replace("/", "\\").Trim();
                            }

                            thirdPartyFiles.Add(f);
                        }

                        //todo: create c3addon, and map parsed data to c3addon 
                        var c3addon = new C3Addon
                        {
                            AddonId = id,
                            AddonCategory = pluginCat,
                            Author = addon["author"]?.ToString(),
                            Class = addon["name"]?.ToString()?.Replace(" ", string.Empty),
                            Company = addon["author"]?.ToString(),
                            Name = addon["name"]?.ToString(),
                            Description = addon["description"]?.ToString(),
                            AddonJson = addon.ToString(),
                            //PluginRunTime = pluginRun,
                            PluginEditTime = pluginEdit,
                            TypeEditTime = typeEdit,
                            //TypeRunTime = typeRun,
                            InstanceEditTime = instanceEdit,
                            //InstanceRunTime = instanceRun,
                            LanguageProperties = prop,
                            LanguageCategories = cats,
                            Id = Guid.NewGuid(),
                            CreateDate = DateTime.Now,
                            LastModified = DateTime.Now,
                            Type = pluginType
                        };

                        c3addon.Actions = new Dictionary<string, Action>();
                        c3addon.Conditions = new Dictionary<string, Condition>();
                        c3addon.Expressions = new Dictionary<string, Expression>();

                        foreach (var action in actionList)
                        {
                            c3addon.Actions.Add(action.Id, action);
                        }

                        foreach (var condition in conditionList)
                        {
                            c3addon.Conditions.Add(condition.Id, condition);
                        }

                        foreach (var expression in expressionList)
                        {
                            c3addon.Expressions.Add(expression.Id, expression);
                        }

                        c3addon.IconXml = File.Exists(Path.Combine(tmpPath, "icon.svg")) ?
                            File.ReadAllText(Path.Combine(tmpPath, "icon.svg")) :
                            ResourceReader.Insatnce.GetResourceText("c3IDE.Templates.Files.icon.svg");

                        c3addon.Template = TemplateFactory.Insatnce.CreateTemplate(c3addon.Type);

                        c3addon.ThirdPartyFiles = new Dictionary<string, ThirdPartyFile>();
                        foreach (var thirdPartyFile in thirdPartyFiles)
                        {
                            c3addon.ThirdPartyFiles.Add($"{thirdPartyFile.ID}", thirdPartyFile);
                        }

                        if (!string.IsNullOrWhiteSpace(c2runtime))
                        {
                            c3addon.C2RunTime = c2runtime;
                        }

                        //regenerate template files
                        c3addon.AddonJson = TemplateCompiler.Insatnce.CompileTemplates(c3addon.Template.AddonJson, c3addon);
                        c3addon.PluginRunTime = TemplateCompiler.Insatnce.CompileTemplates(c3addon.Template.PluginRunTime, c3addon); 
                        c3addon.TypeRunTime = TemplateCompiler.Insatnce.CompileTemplates(c3addon.Template.TypeRunTime, c3addon);
                        c3addon.InstanceRunTime = TemplateCompiler.Insatnce.CompileTemplates(c3addon.Template.InstanceRunTime, c3addon);

                        //fixup pluginedit time (replace png iconw ith svg)
                        c3addon.PluginEditTime = Regex.Replace(c3addon.PluginEditTime, @"this._info.SetIcon\(.*\);", "this._info.SetIcon(\"icon.svg\", \"image/svg+xml\");");

                        return c3addon;
                    }
                    else
                    {
                        //read file text
                        var effectCode = File.ReadAllText(Path.Combine(tmpPath, "effect.fx"));

                        //parse json
                        var lang = JObject.Parse(File.ReadAllText(Path.Combine(tmpPath, "lang", "en-US.json")))["text"][type + "s"][id.ToLower()];

                        var effect = new Effect();
                        effect.BlendsBackground = addon["blends-background"] != null && addon["blends-background"].ToString().ToLower().Contains("true");
                        effect.CrossSampling = addon["cross-sampling"] != null && addon["cross-sampling"].ToString().ToLower().Contains("true");
                        effect.PreservesOpaqueness = addon["preserves-opaqueness"] != null && addon["preserves-opaqueness"].ToString().ToLower().Contains("true");
                        effect.Animated = addon["animated"] != null && addon["animated"].ToString().ToLower().Contains("true");
                        effect.MustPredraw = addon["must-predraw"] != null && addon["must-predraw"].ToString().ToLower().Contains("true");

                        if (addon["extend-box"] != null)
                        {
                            effect.ExtendBoxVertical = int.Parse(addon["extend-box"]["vertical"].ToString());
                            effect.ExtendBoxHorizontal = int.Parse(addon["extend-box"]["horizontal"].ToString());
                        }
                        
                        //add code fx
                        effect.Code = effectCode;

                        //setup params
                        effect.Parameters = new Dictionary<string, EffectParameter>();
                        foreach (var param in addon["parameters"])
                        {
                            var p = new EffectParameter
                            {
                                Json = param.ToString(formatting: Formatting.Indented),
                                Key = param["id"].ToString()
                            };

                            var l = lang["parameters"][p.Key];
                            p.Lang = $"\"{p.Key}\":" + l;

                            switch (param["type"].ToString())
                            {
                                case "float":
                                case "percent":
                                    p.VariableDeclaration = $"uniform lowp float {p.Key}";
                                    break;
                                case "color":
                                    p.VariableDeclaration = $"uniform lowp vec3 {p.Key}";
                                    break;
                            }   
                            effect.Parameters.Add(p.Key, p);
                        }

                        var c3addon = new C3Addon
                        {
                            AddonId = id,
                            AddonCategory = addon["category"].ToString(),
                            Author = addon["author"]?.ToString(),
                            Class = addon["name"]?.ToString()?.Replace(" ", string.Empty),
                            Company = addon["author"]?.ToString(),
                            Name = addon["name"]?.ToString(),
                            Description = addon["description"]?.ToString(),
                            AddonJson = addon.ToString(),
                            Id = Guid.NewGuid(),
                            CreateDate = DateTime.Now,
                            LastModified = DateTime.Now,
                            Type = PluginType.Effect,
                            Effect = effect
                        };

                        //c3 icon
                        c3addon.IconXml = File.Exists(Path.Combine(tmpPath, "icon.svg")) ?
                            File.ReadAllText(Path.Combine(tmpPath, "icon.svg")) :
                            ResourceReader.Insatnce.GetResourceText("c3IDE.Templates.Files.icon.svg");
                        

                        c3addon.Template = TemplateFactory.Insatnce.CreateTemplate(c3addon.Type);

                        return c3addon;
                    }
                });

            }
             catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                LogManager.AddImportLogMessage($"ERROR -> \n{ex.Message}");
                LogManager.AddImportLogMessage($"TRACE -> \n{ex.StackTrace}");
                throw;
            }
            finally
            {
                WindowManager.ShowLoadingOverlay(false);
                var logData = string.Join(Environment.NewLine, LogManager.ImportLog);
                File.WriteAllText(Path.Combine(OptionsManager.CurrentOptions.DataPath, "import.log"), logData);
                LogManager.ImportLog.Clear();
            }

        }
    }
}
