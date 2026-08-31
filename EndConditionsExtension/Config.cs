using System.Collections.Generic;
using System.ComponentModel;
using EndConditionsExtension.Elements;

namespace EndConditionsExtension;

internal class Config
{
    [Description("Do enable the debug (developer) mode?")]
    public bool Debug { get; set; } = false;

    [Description("A list of conditions for each CustomRole - the key is the CustomRole Id")]
    public Dictionary<int, EndCondition> EndConditions { get; set; } = new()
    {
        { 1, new EndCondition() }
    };
}