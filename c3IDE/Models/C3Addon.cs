﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;
using c3IDE.Templates;
using c3IDE.Utilities.Helpers;
using LiteDB;
using Newtonsoft.Json;

namespace c3IDE.Models
{
    [Serializable]
    public class C3Addon : IEquatable<C3Addon>, ICloneable
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Company { get; set; }
        public string Author { get; set; }
        public string AddonId { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public string Version => $"{MajorVersion}.{MinorVersion}.{RevisionVersion}.{BuildVersion}";

        public string Description { get; set; }
        public string AddonFolder { get; set; }
        public string AddonCategory { get; set; } = "other";

        public int MajorVersion { get; set; } = 1;
        public int MinorVersion { get; set; } = 0;
        public int RevisionVersion { get; set; } = 0;
        public int BuildVersion { get; set; } = 0;

        public PluginType Type { get; set; }

        [BsonIgnore]
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case PluginType.SingleGlobalPlugin:
                    case PluginType.DrawingPlugin:
                        return "(Plugin)";
                    case PluginType.Behavior:
                        return "(Behavior)";
                    case PluginType.Effect:
                        return "(Effect)";
                    case PluginType.Theme:
                        return "(Theme)";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        //public string IconBase64 { get; set; }
        public string IconXml { get; set; }

        [BsonIgnore]
        public BitmapImage IconImage => ImageHelper.Insatnce.XmlToBitmapImage(IconXml);
        //public BitmapImage IconImage => ImageHelper.Insatnce.Base64ToBitmap(IconBase64);

        [JsonIgnore]
        public ITemplate Template { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModified { get; set; }

        [BsonIgnore]
        public string ChangeDate => LastModified.ToString("yyyy-MM-dd, hh:mm");
        public string AddonJson { get; set; }
        public string PluginEditTime { get; set; }
        public string PluginRunTime { get; set; }
        public string TypeEditTime { get; set; }
        public string TypeRunTime { get; set; }
        public string InstanceEditTime { get; set; }
        public string InstanceRunTime { get; set; }
        public string LanguageProperties { get; set; }
        public string LanguageCategories { get; set; }

        //effect files
        //public string EffectAddon { get; set; }
        //public string EffectLanguage { get; set; }
        //public string EffectCode { get; set; }

        public Effect Effect { get; set; }

        public string ThemeCss { get; set; }
        public string ThemeLangauge { get; set; }

        [BsonIgnore]
        public List<string> Categories
        {
            get
            {
                var set = new HashSet<string>();
                set.UnionWith(Actions?.Select(x => x.Value.Category) ?? new List<string>());
                set.UnionWith(Conditions?.Select(x => x.Value.Category) ?? new List<string>());
                set.UnionWith(Expressions?.Select(x => x.Value.Category) ?? new List<string>());
                return set.ToList();
            }
        }
        public Dictionary<string, Action> Actions { get; set; }
        public Dictionary<string, Condition> Conditions { get; set; }
        public Dictionary<string, Expression> Expressions { get; set; }
        public Dictionary<string, ThirdPartyFile> ThirdPartyFiles { get; set; }

        //c2 specfic files
        public string C2RunTime { get; set; }

        /// <summary>
        /// deep clone object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            object clone = null;
            using(var stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    var oldGuid = this.Id;
                    this.Id = Guid.NewGuid();
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    clone = formatter.Deserialize(stream);
                    this.Id = oldGuid;
                }
            }
            return clone;
        }

        public bool Equals(C3Addon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((C3Addon)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// the toString override is used when filtering on teh dashboard
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Author}{Class}";
        }

    }
}

