using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InstancesWarning.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace InstancesWarning.Revit.ThirdButton
{
    [Transaction(TransactionMode.Manual)]
    public class ThirdButtonCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiApp = commandData.Application;
            var uidoc = uiApp.ActiveUIDocument;
            var doc = uidoc.Document;

            using (Transaction trans = new Transaction(doc, "Process Warnings and Update Elements"))
            {
                trans.Start();

                var elementIdsToUpdate = new List<ElementId>();

                foreach (FailureMessage failureMessage in doc.GetWarnings())
                {
                    var description = failureMessage.GetDescriptionText();
                    if (description.Contains("No Loss Defined"))
                    {
                        foreach (ElementId failingElementId in failureMessage.GetFailingElements())
                        {
                            elementIdsToUpdate.Add(failingElementId);
                        }

                        foreach (ElementId additionalElementId in failureMessage.GetAdditionalElements())
                        {
                            elementIdsToUpdate.Add(additionalElementId);
                        }
                    }
                }

                var notDefinedDuct = "76eff5da-2e71-45f7-b940-cc5716328ba0";
                var notDefinedPipe = "61e7b8e1-16d1-4fe4-82f0-327af736323f";

                foreach (ElementId elementId in elementIdsToUpdate)
                {
                    var element = doc.GetElement(elementId);
                    if (element.Category.Name == "Duct Fittings")
                    {
                        var lossMethodParam = element.get_Parameter(BuiltInParameter.RBS_DUCT_FITTING_LOSS_METHOD_SERVER_PARAM);
                        if (lossMethodParam != null)
                        {
                            lossMethodParam.Set(notDefinedDuct);
                        }
                    }
                    else if (element.Category.Name == "Pipe Fittings")
                    {
                        var lossMethodParam = element.get_Parameter(BuiltInParameter.RBS_PIPE_FITTING_LOSS_METHOD_SERVER_PARAM);
                        if (lossMethodParam != null)
                        {
                            lossMethodParam.Set(notDefinedPipe);
                        }
                    }
                }

                trans.Commit();

                var totalUpdatedElementIds = elementIdsToUpdate.Count;
                TaskDialog.Show("Result", $"Total updated element IDs: {totalUpdatedElementIds}");
            }

            return Result.Succeeded;
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();

            panel.AddItem(
                new PushButtonData(
                    MethodBase.GetCurrentMethod().DeclaringType?.Name,
                    "Loss" +  Environment.NewLine + "Method",
                    assembly.Location,
                    MethodBase.GetCurrentMethod().DeclaringType?.FullName)
                {
                    ToolTip = "Remove no loss method.",
                    LargeImage = ImageUtils.LoadImage(assembly, "32x32.NoLoss.png")
                });
        }
    }
}
