using EndConditionsExtension.Elements;
using System.Collections.Generic;
using System.ComponentModel;

namespace EndConditionsExtension;

internal class Config
{
    [Description("Do enable the debug (developer) mode?")]
    public bool Debug { get; set; } = false;
    [Description("A list of conditions for each CustomRole")]
    public Dictionary<int, EndCondition> EndConditions { get; set; } = new()
    {
        { 1, new EndCondition() }
    };
}