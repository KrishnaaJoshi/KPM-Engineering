using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InstancesWarning.Revit.Utilities;
using System;
using System.Reflection;

namespace InstancesWarning.Revit.FifthCommand
{
    [Transaction(TransactionMode.Manual)]
    public class FifthButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

            try
            {
                using (Revisions revisionsForm = new Revisions(uiDoc))
                {
                    revisionsForm.ShowDialog();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }


        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            panel.AddItem(new PushButtonData(
                MethodBase.GetCurrentMethod().DeclaringType?.Name,
                "Add or Remove " + Environment.NewLine + "Revision",
                assembly.Location,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            {
                ToolTip = "",
                LargeImage = ImageUtils.LoadImage(assembly, "32x32.RevisionSheet.png")
            });
        }
    }
}























