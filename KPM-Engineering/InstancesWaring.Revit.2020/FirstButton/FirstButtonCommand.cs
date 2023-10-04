using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InstancesWarning.Revit.Utilities;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace InstancesWarning.Revit.FirstButton
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class FirstButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.Form())
                {
                    dialog.Text = "Vendor Information";
                    dialog.Size = new System.Drawing.Size(400, 150);

                    var vendorLabel = new Label
                    {
                        Text = "Vendor: KPM Engineering Pvt Ltd",
                        Location = new System.Drawing.Point(10, 10),
                        AutoSize = true
                    };

                    var urlLabel = new LinkLabel
                    {
                        Text = "kpm-engineering",
                        Location = new System.Drawing.Point(10, 40),
                        AutoSize = true
                    };

                    urlLabel.LinkClicked += (sender, e) =>
                    {
                        Process.Start("https://kpm-engineering.com/");
                    };

                    var infoLabel = new Label
                    {
                        Text = "BUILDING THE FUTURE, ONE LINE OF CODE AT A TIME",
                        Location = new System.Drawing.Point(10, 70),
                        AutoSize = true
                    };

                    dialog.Controls.Add(vendorLabel);
                    dialog.Controls.Add(urlLabel);
                    dialog.Controls.Add(infoLabel);

                    dialog.ShowDialog();
                }

                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            panel.AddItem(
                new PushButtonData(
                    MethodBase.GetCurrentMethod().DeclaringType?.Name,
                    "Information",
                    assembly.Location,
                    MethodBase.GetCurrentMethod().DeclaringType?.FullName)
                {
                    ToolTip = "Click for vendor info",
                    LargeImage = ImageUtils.LoadImage(assembly, "32x32.KPM_Logo.png")
                });
        }
    }
}
