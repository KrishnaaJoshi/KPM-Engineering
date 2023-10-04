using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InstancesWarning.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InstancesWarning.Revit.SecondButton
{
    [Transaction(TransactionMode.Manual)]
    public class SecondButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiApp = commandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;
                var doc = uiDoc.Document;

                var warnings = doc.GetWarnings().Cast<FailureMessage>().ToList();
                var groupedWarnings = new Dictionary<string, List<ElementId>>();

                foreach (var warning in warnings)
                {
                    if (warning.GetDescriptionText().Contains("There are identical instances in the same place."))
                    {
                        var failingElementIds = warning.GetFailingElements();
                        var locationKey = GetLocationKey(doc, failingElementIds.First());

                        if (!groupedWarnings.ContainsKey(locationKey))
                        {
                            groupedWarnings[locationKey] = new List<ElementId>();
                        }

                        groupedWarnings[locationKey].AddRange(failingElementIds);
                    }
                }

                using (Transaction transaction = new Transaction(doc, "Delete Identical Instances"))
                {
                    transaction.Start();

                    foreach (var group in groupedWarnings.Values)
                    {
                        var instancesToDelete = group.Skip(1);  // Keep the first instance, delete the rest.
                        foreach (var elementIdToDelete in instancesToDelete)
                        {
                            doc.Delete(elementIdToDelete);
                        }
                    }

                    transaction.Commit();
                }

                TaskDialog.Show("Output", groupedWarnings.Values.Sum(g => g.Count - 1) + " Identical Instances Deleted.");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private string GetLocationKey(Document doc, ElementId elementId)
        {
            var element = doc.GetElement(elementId);
            var location = element.Location as LocationPoint;
            if (location != null)
            {
                var point = location.Point;
                return $"{point.X},{point.Y},{point.Z}";
            }

            return string.Empty;
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();

            panel.AddItem(
                new PushButtonData(
                    MethodBase.GetCurrentMethod().DeclaringType?.Name,
                    "Identical" + Environment.NewLine+ "Instance",
                    assembly.Location,
                    MethodBase.GetCurrentMethod().DeclaringType?.FullName)
                {
                    ToolTip = "Remove identical instance warnings.",
                    LargeImage = ImageUtils.LoadImage(assembly, "32x32.IdenticalInstance.png")
                });
        }
    }
}
