using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraToolbox;
using RealwareApiAssistant.Core;
using RealwareApiAssistantEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealwareApiAssistantEditor.ViewController
{
    internal class MainViewController
    {
        private List<ScriptViewModel> scripts;
        private int scriptCounter = 1;

        private readonly MainForm _form;

        public MainViewController(MainForm form)
        {
            this._form = form;

            scripts = new List<ScriptViewModel>();
        }

        public void OpenScriptDialog(string filePath = null)
        {
            XtraOpenFileDialog dialog = new XtraOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users\\seanr\\source\\repos\\RealWareApiAssistant\\Example Scripts";//TODO: Fix this path!!!
            dialog.Filter = "Realware API Assistant Script|*.json";
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //TODO: Check all tabs for if file is already open

                string scriptName = dialog.FileName.Substring(
                    dialog.FileName.LastIndexOf("\\") + 1);

                createScriptTab(scriptName, dialog.FileName);
            }
        }

        internal void NewScript()
        {
            var scriptName = string.Format(Constants.NewScriptName, scriptCounter);
            scriptCounter++;

            createScriptTab(scriptName);
        }

        private void createScriptTab(string scriptName, string scriptFilePath = null)
        {
            var editor = new Views.ScriptEditView();

            var tabView = _form.GetTabbedView().AddDocument(editor, scriptName);

            scripts.Add(new ScriptViewModel
            {
                EditorView = editor,
                ScriptName = scriptName,
                RequiresSave = scriptFilePath == null,
                ScriptFilePath = scriptFilePath
            });

            _form.GetTabbedView().ActivateDocument(editor);
        }

        internal void CloseApplication()
        {
            //TODO: If none need saved, continue

            _form.Close();
        }

        internal void ExecuteSelectedScript()
        {
            //TODO: If not saved, make them save it first
        }
    }
}
