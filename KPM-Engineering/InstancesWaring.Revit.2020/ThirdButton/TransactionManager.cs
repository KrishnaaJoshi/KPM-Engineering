using Autodesk.Revit.DB;

namespace InstancesWarning.Revit.ThirdButton
{
    internal class TransactionManager
    {
        private Document doc;

        public TransactionManager(Document doc)
        {
            this.doc = doc;
        }
    }
}