using DevExpress.XtraBars.Docking2010.Views;
using RealwareApiAssistantEditor.Views;

namespace RealwareApiAssistantEditor.ViewModels
{
    public class ScriptViewModel
    {
        public string ScriptName { get; set; }
        public string ScriptFilePath { get; set; }
        public ScriptEditView EditorView { get; set; }
        public BaseDocument TabView { get; set; }

        public bool RequiresSave { get; set; }
    }
}
