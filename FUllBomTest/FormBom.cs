
using eDrawings.Interop.EModelViewControl;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Printing;
using EPDM.Interop.epdm;

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

        string OldFileName = "";
        bool isSpFromFile = false;
 
        private IEdmVault5 vault1 = null;
        IEdmBatchGet batchGetter;

        private void GetLastVersionFiles()
        {
           
            if (vault1 == null)
            {
                vault1 = new EdmVault5();
            }

            if (!vault1.IsLoggedIn)
            {
                vault1.LoginAuto("CUBY_PDM", 0);
         
            }

            IEdmVault7 vault2 = null;
            vault2 = (IEdmVault7)vault1;

            batchGetter = (IEdmBatchGet)vault2.CreateUtility(EdmUtility.EdmUtil_BatchGet);

            batchGetter.AddSelectionEx((EdmVault5)vault1, GetAssemblyID.ASMID, GetAssemblyID.ASMFolderID, null);
            if (batchGetter != null)
                {
                    batchGetter.CreateTree(0, (int)EdmGetCmdFlags.Egcf_Nothing);
                    //batchGetter.ShowDlg(0);
                    batchGetter.GetFiles(0, null);
                }

            }

            public FormBom(List<ComponentBom> list, string pathFolderSave)
        {
            listDrawing = list;
            outDir = pathFolderSave;
            InitializeComponent();
            GetLastVersionFiles();
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

            PrintNext();
        }

        private void OnDocumentLoadFailed(string fileName, int errorCode, string errorString)
        {
            PrintNext();
        }

        private void OnDocumentLoaded(string fileName)
        {

            string PRINTER_NAME = "Microsoft Print to PDF";
            int AUTO_SOURCE = 7;
            string pdfFileName = Path.GetFileNameWithoutExtension(fileName) + ".pdf";
            string pdfFilePath;
            pdfFilePath = Path.Combine(outDir, pdfFileName);
            int paperKind;
            int countSheets = m_Ctrl.SheetCount;
            int currentSheets;
            int indexFirstSP = 0;
            if (countSheets > 1)

            {
                for (currentSheets = 0; currentSheets < countSheets; currentSheets++)

                    if (m_Ctrl.SheetName[currentSheets] == "SP1")
                    {
                        indexFirstSP = currentSheets;
                        break;
                    }

            }
            int sheetHeigth = (int)m_Ctrl.SheetHeight;
            int sheetWidth = (int)m_Ctrl.SheetWidth;
            EMVPrintOrientation orient;
            if (sheetHeigth > sheetWidth)
            {
                orient = EMVPrintOrientation.ePortrait;
                if (sheetWidth == 210 || sheetHeigth == 297)
                {
                    paperKind = (int)PaperKind.A4;
                }
                else if (sheetWidth == 297 || sheetHeigth == 420)
                {
                    paperKind = (int)PaperKind.A3;
                }
                else if (sheetWidth == 420 || sheetHeigth == 594)
                {
                    paperKind = (int)PaperKind.A2;
                }
                else
                {
                    paperKind = 0;
                }
            }
            else
            {
                orient = EMVPrintOrientation.eLandscape;

                if (sheetWidth == 297 || sheetHeigth == 210)
                {
                    paperKind = (int)PaperKind.A4;
                }
                else if (sheetWidth == 420 || sheetHeigth == 297)
                {
                    paperKind = (int)PaperKind.A3;
                }
                else if (sheetWidth == 594 || sheetHeigth == 420)
                {
                    paperKind = (int)PaperKind.A2;
                }
                else
                {
                    paperKind = 0;
                }
            }
            bool printAllSheets = true;

            m_Ctrl.SetPageSetupOptions(orient, paperKind, sheetHeigth, sheetWidth, 1, AUTO_SOURCE, PRINTER_NAME, 0, 0, 0, 0);

            if (indexFirstSP == 0)
            {

                m_Ctrl.Print5(false, fileName, false, false, true, EMVPrintType.eScaleToFit, 1, 0, 0, printAllSheets, 1, 1, pdfFilePath);
            }
            else
            {
                printAllSheets = false;
                string nameSP = Path.GetFileNameWithoutExtension(fileName) + "-SP" + ".pdf";
                string pdfSPPath;
                pdfSPPath = Path.Combine(outDir, nameSP);
                if (!isSpFromFile)
                {
                    m_Ctrl.SetPageSetupOptions(orient, paperKind, sheetHeigth, sheetWidth, 1, AUTO_SOURCE, PRINTER_NAME, 0, 0, 0, 0);
                    OldFileName = fileName;
                    isSpFromFile = true;
                    m_Ctrl.Print5(false, fileName, false, false, false, EMVPrintType.eScaleToFit, 1, 0, 0, printAllSheets, 1, indexFirstSP, pdfFilePath);
                }
                else
                {
                    m_Ctrl.SetPageSetupOptions(EMVPrintOrientation.ePortrait, (int)PaperKind.A4, sheetHeigth, sheetWidth, 1, AUTO_SOURCE, PRINTER_NAME, 0, 0, 0, 0);
                    isSpFromFile = false;
                    OldFileName = "";
                    m_Ctrl.Print5(false, fileName, false, false, false, EMVPrintType.eScaleToFit, 1, 0, 0, printAllSheets, indexFirstSP + 1, countSheets, pdfSPPath);

                }

            }

        }

        private void PrintNext()
        {
            if (isSpFromFile)
            {
                m_Ctrl.CloseActiveDoc("");
                m_Ctrl.OpenDoc(OldFileName, false, false, false, "");
            }
            else
            {

                if (listDrawing.Count > 0)
                {
                    string filePath = listDrawing[0].pathFile;
                    listDrawing.RemoveAt(0);
                    m_Ctrl.CloseActiveDoc("");
                   // MessageBox.Show(filePath);
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
                            MessageBox.Show("К сожалению, попробуйте завтра.");
                        }


                    }
                }
            }
        }


    }
}


