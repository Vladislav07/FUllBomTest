using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Windows.Forms;

namespace FullBomHoum
{
    public  class SldApp
    {
          SldWorks swApp;
          Action<string> ActionOpenFile;
          List<string> drawings;
        public SldApp()
        {
            swApp = new SldWorks();
            drawings = GetAssemblyID.listdrawings;
            swApp.Visible = true;
        }

        public void Metod ()
        {
            ModelDoc2 swModelDoc = default(ModelDoc2);
            Frame swFrame = default(Frame);
            ModelWindow swModelWindow = default(ModelWindow);
            object[] modelWindows = null;
            int errors = 0;
            int warnings = 0;
            int HWnd = 0;
            string fileName = null;
            string strFolder = null;
            try
            {
              foreach (string item in drawings)
                {
                    fileName = item;
                   swModelDoc = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocDRAWING, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
              
                    swApp.CreateNewWindow();
                }
                swFrame = (Frame)swApp.Frame();
                modelWindows = (object[])swFrame.ModelWindows;
            
                foreach (object obj in modelWindows)
                {
                    swModelWindow = (ModelWindow)obj;
                    //Get the model document in this model window
                    swModelDoc = (ModelDoc2)swModelWindow.ModelDoc;
                    //Rebuild the document
                    swModelDoc.EditRebuild3();
                    swModelDoc = null;
                    //Show the model window
                 
                    swFrame.ShowModelWindow(swModelWindow);
                   
               
               
                }

            }
            catch (Exception)
            {
                MessageBox.Show(errors.ToString());
                
            }
           
           

        }
    }
}
