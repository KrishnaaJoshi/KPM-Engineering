using Autodesk.Revit.UI;
using InstancesWarning.Revit.FifthCommand;
using InstancesWarning.Revit.FirstButton;
using InstancesWarning.Revit.FourthButton;
using InstancesWarning.Revit.SecondButton;
using InstancesWarning.Revit.SixthButton;
using InstancesWarning.Revit.ThirdButton;
using System;
using System.Linq;

namespace InstancesWarning.Revit
{
    public class AppCommand : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            try
            {
                app.CreateRibbonTab("KPM-Engineering");
            }
            catch (Exception)
            {
                // Ignored
            }
            
            var ribbonPanel = app
                .GetRibbonPanels("KPM-Engineering")
                .FirstOrDefault(x => x.Name == "KPM-Engineering") ?? app
                .CreateRibbonPanel("KPM-Engineering", "KPM");

            FirstButtonCommand.CreateButton(ribbonPanel);

            var ribbonPanel1 = app.CreateRibbonPanel("KPM-Engineering", "Warnings");
                
            SecondButtonCommand.CreateButton(ribbonPanel1);
            ThirdButtonCommand.CreateButton(ribbonPanel1);
            FourthButtonCommand.CreateButton(ribbonPanel1);

            var ribbonPanel2 = app.CreateRibbonPanel("KPM-Engineering", "Generic");
            
            FifthButtonCommand.CreateButton(ribbonPanel2);
            SixthButtonCommand.CreateButton(ribbonPanel2);

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
