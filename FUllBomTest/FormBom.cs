
using eDrawings.Interop.EModelViewControl;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Printing;


namespace FullBomHoum
{
    public struct ComponentBom
    {
        public ComponentBom(string _path, int _count)
        {
            pathFile = _path;
            count = _count;

        }
       public string pathFile { get; set; }
       public int count { get; set; }
    }
    
    public partial class FormBom : Form
    {
      
        EModelViewControl m_Ctrl;

        string outDir = null;
        List<ComponentBom> listDrawing;
 

       

        public FormBom(List<ComponentBom> list, string pathFolderSave)
        {
            listDrawing = list;
            outDir = pathFolderSave;
            InitializeComponent();
           
        }

      

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ctrlEDrw.LoadEDrawings();
        }

        private void OnControlLoaded(EModelViewControl ctrl)
        {
            m_Ctrl = ctrl;
            m_Ctrl.OnFinishedLoadingDocument += OnDocumentLoaded;
            m_Ctrl.OnFailedLoadingDocument += OnDocumentLoadFailed;
            m_Ctrl.OnFinishedPrintingDocument += OnDocumentPrinted;
            m_Ctrl.OnFailedPrintingDocument += OnPrintFailed;

          
            PrintNext();
        }
        
        private void OnPrintFailed(string PrintJobName)
        {
            Trace.WriteLine($"Failed to export - {PrintJobName}");
            PrintNext();
        }

        private void OnDocumentPrinted(string PrintJobName)
        {

            Console.WriteLine($"export completed {PrintJobName}");
            PrintNext();
        }

        private void OnDocumentLoadFailed(string fileName, int errorCode, string errorString)
        {
            Console.WriteLine($"{fileName} failed to loaded: {errorString}");
            PrintNext();
        }

        private void OnDocumentLoaded(string fileName)
        {
            Console.WriteLine($"{fileName} loaded");
             string PRINTER_NAME = "Microsoft Print to PDF";
             int AUTO_SOURCE = 7;
             m_Ctrl.SetPageSetupOptions(EMVPrintOrientation.eLandscape, 7, 100, 100, 1, AUTO_SOURCE, PRINTER_NAME, 0, 0, 0, 0);
            string pdfFileName = Path.GetFileNameWithoutExtension(fileName) + ".pdf";
            string pdfFilePath;
            pdfFilePath = Path.Combine(outDir, pdfFileName);
            int sheetHeigth = (int)m_Ctrl.SheetHeight;
            int sheetWidth =(int) m_Ctrl.SheetWidth;
            EMVPrintOrientation orient;
            if (sheetHeigth > sheetWidth)
            {
                orient = EMVPrintOrientation.ePortrait;
            }
            else
            {
               orient = EMVPrintOrientation.eLandscape;
            }
          
            
           
            m_Ctrl.SetPageSetupOptions(orient, (int)PaperKind.A4, sheetHeigth, sheetWidth, 1,
                (int)PaperSourceKind.Custom, PRINTER_NAME, 0, 0, 0,0);
            m_Ctrl.Print5(false, fileName, false, false, true, EMVPrintType.eScaleToFit, 1, 0, 0, true, 1, 1, pdfFilePath);
          
        }

        private void PrintNext()
        {
            
            if (listDrawing.Count > 0)
            {
              string filePath = listDrawing[0].pathFile;
              //countMarkup = listDrawing[0].count.ToString();
              listDrawing.RemoveAt(0);
              m_Ctrl.CloseActiveDoc("");
               // string pdfFileName = Path.GetFileNameWithoutExtension(filePath) + ".pdf";
              //  string  pdfFilePath = Path.Combine(outDir, pdfFileName);
              //  AddTextStampPage.Run(pdfFilePath, 1);

                m_Ctrl.OpenDoc(filePath, false, false, false, "");

            }
            else
            {
                var rs = MessageBox.Show("processing completed", "", MessageBoxButtons.OK);
                if (rs == DialogResult.OK)
                {
                    try
                    {
                        Invoke(new Action(() => this.Close()));
                    }
                    catch
                    {

                    }
                    MessageBox.Show("К сожалению, попробуйте завтра.");
                    
                }                
            }
        }       
    }
}
