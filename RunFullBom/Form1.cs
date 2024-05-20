using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EPDM.Interop.epdm;
using Microsoft.VisualBasic;
using FullBomHoum;

namespace RunFullBom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private IEdmVault5 vault1 = null;

        public void Form1_Load(System.Object sender, System.EventArgs e)
        {
            try
            {
                IEdmVault5 vault1 = new EdmVault5();
                IEdmVault8 vault = (IEdmVault8)vault1;
                EdmViewInfo[] Views = null;

                vault.GetVaultViews(out Views, false);
                VaultsComboBox.Items.Clear();
                foreach (EdmViewInfo View in Views)
                {
                    VaultsComboBox.Items.Add(View.mbsVaultName);
                }
                if (VaultsComboBox.Items.Count > 0)
                {
                    VaultsComboBox.Text = (string)VaultsComboBox.Items[0];
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BrowseButton_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                ListBox.Items.Clear();

                if (vault1 == null)
                {
                    vault1 = new EdmVault5();
                }
                if (!vault1.IsLoggedIn)
                {
                    //Log into selected vault as the current user
                    vault1.LoginAuto(VaultsComboBox.Text, this.Handle.ToInt32());
                }

                //Set the initial directory in the Open dialog
                OpenFileDialog.InitialDirectory = vault1.RootFolderPath;
                //Show the Open dialog
                System.Windows.Forms.DialogResult DialogResult;
                DialogResult = OpenFileDialog.ShowDialog();
                //If the user didn't click Open, exit
                if (!(DialogResult == System.Windows.Forms.DialogResult.OK))
                {
                    return;
                }

                foreach (string FileName in OpenFileDialog.FileNames)
                {
                    ListBox.Items.Add(FileName);
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetInfo_Click(object sender, EventArgs e)
        {
            string path = ListBox.Items[0].ToString();
            FullBomHoum.GetAssemblyID ass = new GetAssemblyID();
            ass.OnCmd(path);
        }
    }
}
