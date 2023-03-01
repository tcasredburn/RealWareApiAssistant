using DevExpress.XtraEditors;
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
    public partial class ScriptEditView : DevExpress.XtraEditors.XtraUserControl
    {
        public ScriptEditView()
        {
            InitializeComponent();

            // Set default page to simple or "first" page
            xtraTabControl1.SelectedTabPageIndex = 0;
        }
    }
}
