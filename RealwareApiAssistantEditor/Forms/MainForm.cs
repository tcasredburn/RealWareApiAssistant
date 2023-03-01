using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using RealwareApiAssistantEditor.ViewController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealwareApiAssistantEditor
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private MainViewController _controller;

        public MainForm()
        {
            InitializeComponent();

            // Manager initialization
            _controller = new MainViewController(this);

            // Button/menu initialization
            // File
            iNewScript.ItemClick += (s, e) => _controller.NewScript();
            iOpenScript.ItemClick += (s, e) => _controller.OpenScriptDialog();
            iExit.ItemClick += (s, e) => _controller.CloseApplication();
            // Script
            iExecuteScript.ItemClick += (s, e) => _controller.ExecuteSelectedScript();
            //
        }

        public TabbedView GetTabbedView() => tabbedView1;

    }
}
