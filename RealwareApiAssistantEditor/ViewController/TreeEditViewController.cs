using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealwareApiAssistantEditor.ViewController
{
    internal class TreeEditViewController
    {
        private TreeList treeList1;

        public TreeEditViewController(TreeList treeList1)
        {
            this.treeList1 = treeList1;

            initializeTreeColumns();
            initializeTreeNodes();

            treeList1.ExpandAll();
        }

        private void initializeTreeColumns()
        {
            treeList1.Columns.Clear();
            
            TreeListColumn columnName = new TreeListColumn();
            columnName.Caption = "Name";
            columnName.VisibleIndex = 0;
            treeList1.Columns.Add(columnName);

            TreeListColumn columnValue = new TreeListColumn();
            columnValue.Caption = "Value";
            columnValue.VisibleIndex = 1;
            treeList1.Columns.Add(columnValue);
        }

        private void initializeTreeNodes()
        {
            treeList1.Nodes.Clear();

            treeList1.BeginUnboundLoad();

            initializeNodesApiSettings();
            initializeNodesData();
            initializeNodesIdentifiers();
            initializeNodesChanges();
            initializeScriptOptions();

            treeList1.EndUnboundLoad();
        }


        private void initializeNodesApiSettings()
        {
            TreeListNode nodeApiSettings = addNode("Api Settings");
            TreeListNode nodeApiSettingsUrl = addNode("Url", "", nodeApiSettings);
            TreeListNode nodeApiSettingsToken = addNode("Token", "", nodeApiSettings);
        }

        private void initializeNodesData()
        {
            TreeListNode nodeData = addNode("Data");
            TreeListNode nodeDataType = addNode("Type", "Excel", nodeData);
            TreeListNode nodeDataSource = addNode("Source", null, nodeData);
        }

        private void initializeNodesIdentifiers()
        {
            TreeListNode nodeIdentifiers = addNode("Identifiers");

            TreeListNode nodeIdentifiers0 = addNode("0", null, nodeIdentifiers);
            TreeListNode nodeIdentifiers0_Id = addNode("Id Name", "accountNo", nodeIdentifiers0);
            TreeListNode nodeIdentifiers0_ValueType = addNode("Value Type", "Static", nodeIdentifiers0);
            TreeListNode nodeIdentifiers0_Value = addNode("Value", "R81804841567600", nodeIdentifiers0);

            TreeListNode nodeIdentifiers1 = addNode("1", null, nodeIdentifiers);
            TreeListNode nodeIdentifiers1_Id = addNode("Id Name", "taxYear", nodeIdentifiers1);
            TreeListNode nodeIdentifiers1_ValueType = addNode("Value Type", "Static", nodeIdentifiers1);
            TreeListNode nodeIdentifiers1_Value = addNode("Value", "2023", nodeIdentifiers1);
        }

        private void initializeNodesChanges()
        {
            TreeListNode nodeChanges = addNode("Changes");
            TreeListNode nodeChangesUpdate = addNode("Update", null, nodeChanges);

            TreeListNode nodeChangesInsert = addNode("Insert", null, nodeChanges);
        }

        private void initializeScriptOptions()
        {
            TreeListNode nodeScriptOptions = addNode("Script Options");
            
            TreeListNode nodeScriptOptionsSkipConfirmations = addNode("Skip Confirmations", false, nodeScriptOptions);
            TreeListNode nodeScriptOptionsSkipWarningPrompts = addNode("Skip Warning Prompts", false, nodeScriptOptions);
            TreeListNode nodeScriptOptionsRetryImmediately = addNode("Retry Immediately after Bad Request", true, nodeScriptOptions);
            TreeListNode nodeScriptOptionsForceExcelNULLValues = addNode("Force Excel NULL Values", true, nodeScriptOptions);
            TreeListNode nodeScriptOptionsThreads = addNode("Threads", 1, nodeScriptOptions);
        }

        private TreeListNode addNode(string title, object value = null, TreeListNode parent = null)
        {
            return treeList1.AppendNode(
                new object[] { title, value },
                parent
                );
        }
    }
}
