using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Services;
using RealwareApiAssistantEditor.Service;
using System;

namespace RealwareApiAssistantEditor.Views
{
    public partial class ScriptTextEditView : DevExpress.XtraEditors.XtraUserControl
    {
        public event EventHandler ScriptChangedEvent;

        public ScriptTextEditView()
        {
            InitializeComponent();

            initializeHighlighting();

            richEditControl1.Text = Newtonsoft.Json.JsonConvert.SerializeObject(
                RealwareApiAssistant.Core.Models.IO.ApiScript.GetDefault(),
                Newtonsoft.Json.Formatting.Indented);

            intializeEvents();
        }

        private void initializeHighlighting()
        {
            // Register the created service and load the document
            richEditControl1.ReplaceService<ISyntaxHighlightService>(new ScriptSyntaxHighlightService(richEditControl1.Document));

            // Specify the richEdit's layout settings
            richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            richEditControl1.Document.Sections[0].Page.Width = Units.InchesToDocumentsF(80f);
            richEditControl1.Document.DefaultCharacterProperties.FontName = "Courier New";
        }

        private void intializeEvents()
        {
            richEditControl1.TextChanged += richEditControl1_TextChanged;
        }

        private void richEditControl1_TextChanged(object sender, EventArgs e)
            => ScriptChangedEvent?.Invoke(this, EventArgs.Empty);

    }
}
