using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InstancesWarning.Revit.SixthButton
{
    public partial class AlignViewport : System.Windows.Forms.Form
    {
        private Document doc;

        public AlignViewport(Document doc)
        {
            InitializeComponent();

            var sheetsCollector = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet));
            foreach (ViewSheet sheet in sheetsCollector)
            {
                listBoxSheets.Items.Add(new ListBoxItem(sheet.Name, sheet.Id));
            }

            var viewportsCollector = new FilteredElementCollector(doc).OfClass(typeof(Viewport));
            foreach (Viewport viewport in viewportsCollector)
            {
                var associatedView = doc.GetElement(viewport.ViewId) as Autodesk.Revit.DB.View;
                if (associatedView != null)
                {
                    listBoxViewPorts.Items.Add(new ListBoxItem(associatedView.Name, viewport.Id));
                }
            }

            this.doc = doc;
        }

        private void btnAlign_Click(object sender, EventArgs e)
        {
            var selectedSheetItem = listBoxSheets.SelectedItem as ListBoxItem;
            if (selectedSheetItem == null) return;

            var selectedViewportItems = listBoxViewPorts.SelectedItems.Cast<ListBoxItem>().ToList();
            if (!selectedViewportItems.Any()) return;

            using (Transaction trans = new Transaction(doc, "Align Viewports"))
            {
                trans.Start();

                double avgCenterY = 0;

                foreach (ListBoxItem item in selectedViewportItems)
                {
                    Viewport vp = doc.GetElement(item.Id) as Viewport;
                    if (vp != null)
                    {
                        var bbox = vp.GetBoxCenter();
                        avgCenterY += bbox.Y;
                    }
                }
                avgCenterY /= selectedViewportItems.Count;

                foreach (ListBoxItem item in selectedViewportItems)
                {
                    Viewport vp = doc.GetElement(item.Id) as Viewport;
                    if (vp != null)
                    {
                        var bbox = vp.GetBoxCenter();
                        var offset = avgCenterY - bbox.Y;

                        XYZ moveVector = new XYZ(0, offset, 0);
                        ElementTransformUtils.MoveElement(doc, item.Id, moveVector);
                    }
                }

                trans.Commit();
            }

            MessageBox.Show($"Aligned {selectedViewportItems.Count} viewports to sheet: {selectedSheetItem.Name}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public class ListBoxItem
        {
            public string Name { get; set; }
            public ElementId Id { get; set; }

            public ListBoxItem(string name, ElementId id)
            {
                Name = name;
                Id = id;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://kpm-engineering.com/");
        }
    }
}
