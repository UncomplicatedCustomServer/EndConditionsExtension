using EndConditionsExtension.Elements;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndConditionsExtension
{
    internal class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; }
        [Description("Do enable the debug (developer) mode?")]
        public bool Debug { get; set; }
        [Description("A list of conditions for each CustomRole")]
        public Dictionary<int, EndCondition> EndConditions { get; set; } = new()
        {
            { 1, new() }
        };
    }
}
