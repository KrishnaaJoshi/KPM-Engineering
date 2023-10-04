using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InstancesWarning.Revit.Utilities;

namespace InstancesWarning.Revit.FourthButton
{
    [Transaction(TransactionMode.Manual)]
    public class FourthButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                var warnings = doc.GetWarnings();

                var descriptions = new List<string>();
                var elementIds = new List<ElementId>();

                foreach (FailureMessage warning in warnings)
                {
                    var description = warning.GetDescriptionText();
                    if (description.Contains("Elements have duplicate \"Mark\" values."))
                    {
                        descriptions.Add(description);

                        var failingElementIds = warning.GetFailingElements().ToList();
                        elementIds.AddRange(failingElementIds.Where(elementId => !elementIds.Contains(elementId)));
                    }
                }

                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Update Elements Numbers");

                    foreach (ElementId elementId in elementIds)
                    {
                        var element = doc.GetElement(elementId);
                        var doorNumberParameter = element.get_Parameter(BuiltInParameter.DOOR_NUMBER);

                        if (doorNumberParameter != null)
                        {
                            var preValue = doorNumberParameter.AsString();
                            var updatedValue = $"{preValue}-{elementIds.IndexOf(elementId)}";
                            doorNumberParameter.Set(updatedValue);
                        }
                    }

                    transaction.Commit();

                    TaskDialog.Show("Update Element Numbers", $"{elementIds.Count} Element numbers updated successfully.");
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
                "Mark" + Environment.NewLine + "Value",
                assembly.Location,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            {
                ToolTip = "Remove mark value warnings.",
                LargeImage = ImageUtils.LoadImage(assembly, "32x32.MarkValue.png")
            });
        }
    }
}
