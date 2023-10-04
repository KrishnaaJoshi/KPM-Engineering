using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InstancesWarning.Revit.Utilities;
using System;
using System.Reflection;

namespace InstancesWarning.Revit.SixthButton
{
    [Transaction(TransactionMode.Manual)]
    public class SixthButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var form = new AlignViewport(doc);

            form.ShowDialog();

            return Result.Succeeded;
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();

            panel.AddItem(new PushButtonData(
                MethodBase.GetCurrentMethod().DeclaringType?.Name,
                "Align Viewports" + Environment.NewLine + "on Sheets",
                assembly.Location,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            {
                ToolTip = "",
                LargeImage = ImageUtils.LoadImage(assembly, "32x32.Viewport.png")
            });
        }
    }
}