using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPDM.Interop.epdm;
using System.Windows.Forms;

namespace FullBomHoum
{
   public class FirstStep
    {
        private IEdmVault5 vault1 = null;
        string path;
        public FirstStep()
        {
           // OpenFileDialog ofd = new OpenFileDialog();
            try
            {
                /*

                if (vault1 == null)
                {
                    vault1 = new EdmVault5();
                }
                if (!vault1.IsLoggedIn)
                {
                    //Log into selected vault as the current user
                    vault1.LoginAuto("CUBY_PDM", 0);
                }

                //Set the initial directory in the Open dialog
                ofd.InitialDirectory = vault1.RootFolderPath;
                //Show the Open dialog
                System.Windows.Forms.DialogResult DialogResult;
                DialogResult = ofd.ShowDialog();
                //If the user didn't click Open, exit
                if (!(DialogResult == System.Windows.Forms.DialogResult.OK))
                {
                    return;
                }
                path = ofd.FileName;
                */

                path = @"C:\CUBY_PDM\Work\Other\Без проекта\CUBY-V1.1\CAD\Завод контейнер\Склад из контейнеров\Лестница\CUBY-00259479.sldasm";
                GetAssemblyID ass = new GetAssemblyID();
                ass.OnCmd(path);

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
    }
}
