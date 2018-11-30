﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c3IDE.PluginModels;

namespace c3IDE.EventCore
{
    //this event is called when a new plugin is created from the home screen 
    public class NewPluginEvents : EventMessageBase
    {
        public PluginTypeEnum Type { get; set; }
        public NewPluginEvents(object sender, PluginTypeEnum type) : base(sender, type)
        {
            Type = type;
        }
    }

    //this event is called when a new c3 plugin object is created from template, that c3 plugin object is boardcast to all user controls 
    public class UpdatePluginEvents : EventMessageBase
    {
        public C3Plugin PluginData { get; set; }
        public UpdatePluginEvents(object sender, C3Plugin data) : base(sender, data)
        {
            PluginData = data;
        }
    }

    //this event is triggered when the user clicks the save button
    public class SavePluginEvents : EventMessageBase
    {
        public SavePluginEvents(object sender) : base(sender, null)
        {
        }
    }

    //this event is triggered when a new property created and saved
    public class NewPropertyPluginEvents : EventMessageBase
    {
        public Property Property { get; set; }
        public NewPropertyPluginEvents(object sender, Property newProperty) : base(sender, newProperty)
        {
            Property = newProperty;
        }
    }

    //this event is triggered when a property is updated and saved
    public class UpdatePropertyPluginEvents : EventMessageBase
    {
        public Property NewProperty { get; set; }
        public Property OldProperty { get; set; }

        public UpdatePropertyPluginEvents(object sender, Property newProperty, Property oldProperty ) : base(sender, new { NewProperty = newProperty, OldProperty = oldProperty})
        {
            NewProperty = newProperty;
            OldProperty = oldProperty;
        }
    }
}
