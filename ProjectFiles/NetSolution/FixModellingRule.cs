#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.OPCUAServer;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
#endregion

public class FixModellingRule : BaseNetLogic
{
    [ExportMethod]
    public void SetPropertiesAsMandatory()
    {
        // Insert code to be executed by the method
        var motorType = Owner.Get("MotorType");
        Log.Info("Setting ModellingRule to MandatoryPlaceholder for node: " + motorType.BrowseName);

        foreach (var item in motorType.Children)
        {
            SetModellingRuleMandatory(item);
        }
    }

    private void SetModellingRuleMandatory(IUANode node)
    {
        Log.Info("Setting ModellingRule to MandatoryPlaceholder for node: " + node.BrowseName);
        node.ModellingRule = NamingRuleType.MandatoryPlaceholder;

        foreach (var item in node.Children)
        {
            SetModellingRuleMandatory(item);
        }
    }
}
