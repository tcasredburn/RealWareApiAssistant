using DevExpress.XtraEditors;
using RealwareApiAssistantEditor.ViewController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealwareApiAssistantEditor.Views
{
    public partial class ScriptTreeEditView : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly TreeEditViewController _controller;

        public ScriptTreeEditView()
        {
            InitializeComponent();

            _controller = new TreeEditViewController(treeList1);
        }

    }
}
