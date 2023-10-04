using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace InstancesWarning.Revit.FifthCommand
{
    public partial class Revisions : System.Windows.Forms.Form
    {
        private readonly UIDocument _uiDoc;
        private int _sheetsUpdated = 0;

        public Revisions(UIDocument uiDoc)
        {
            InitializeComponent();
            _uiDoc = uiDoc;
            PopulateRevisions();
            PopulateSheets();
            button1.Click += button1_Click;
            SelectSheet.SelectionMode = SelectionMode.MultiExtended;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var doc = _uiDoc.Document;
            var selectedSheets = GetSelectedSheets();
            var selectedRevisions = GetSelectedRevisions();

            UpdateSheetRevisions(selectedSheets, selectedRevisions, CheckBox.Checked);

            MessageBox.Show($"{_sheetsUpdated} sheets updated.", "Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
        private void PopulateRevisions()
        {
            var collector = new FilteredElementCollector(_uiDoc.Document);
            var revisions = collector.OfClass(typeof(Revision)).ToElements();

            foreach (var revision in revisions.Cast<Revision>())
            {
                GetRevision.Items.Add(revision.Name);
            }
        }

        private void PopulateSheets()
        {
            var collector = new FilteredElementCollector(_uiDoc.Document);
            var sheets = collector.OfClass(typeof(ViewSheet)).ToElements();

            foreach (var sheet in sheets.Cast<ViewSheet>())
            {
                SelectSheet.Items.Add(sheet.Name);
            }
        }

        

        private IList<ElementId> GetSelectedSheets()
        {
            var selectedSheets = new List<ElementId>();

            foreach (string sheetName in SelectSheet.SelectedItems)
            {
                var sheet = GetSheetByName(sheetName);
                if (sheet != null) selectedSheets.Add(sheet.Id);
            }

            return selectedSheets;
        }

        private IList<ElementId> GetSelectedRevisions()
        {
            var selectedRevisions = new List<ElementId>();

            foreach (string revisionName in GetRevision.SelectedItems)
            {
                var revision = GetRevisionByName(revisionName);
                if (revision != null) selectedRevisions.Add(revision.Id);
            }

            return selectedRevisions;
        }

        private ViewSheet GetSheetByName(string sheetName)
        {
            return new FilteredElementCollector(_uiDoc.Document)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .FirstOrDefault(s => s.Name == sheetName);
        }

        private Revision GetRevisionByName(string revisionName)
        {
            return new FilteredElementCollector(_uiDoc.Document)
                .OfClass(typeof(Revision))
                .Cast<Revision>()
                .FirstOrDefault(r => r.Name == revisionName);
        }

        private void UpdateSheetRevisions(IList<ElementId> sheetIds, IList<ElementId> revisionIds, bool addRevisions)
        {
            var doc = _uiDoc.Document;

            using (var tx = new Transaction(doc, "Update Revisions"))
            {
                tx.Start();

                foreach (var sheetId in sheetIds)
                {
                    var sheet = doc.GetElement(sheetId) as ViewSheet;
                    if (sheet != null)
                    {
                        var sheetRevisions = new HashSet<ElementId>(sheet.GetAllRevisionIds());

                        foreach (var revisionId in revisionIds)
                        {
                            if (addRevisions)
                            {
                                sheetRevisions.Add(revisionId);
                            }
                            else
                            {
                                sheetRevisions.Remove(revisionId);
                            }
                        }

                        sheet.SetAdditionalRevisionIds(sheetRevisions);
                        _sheetsUpdated++;
                    }
                }

                tx.Commit();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://kpm-engineering.com/");
        }
    }
}
