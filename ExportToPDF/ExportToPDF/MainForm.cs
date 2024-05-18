
using eDrawings.Interop.EModelMarkupControl;
using eDrawings.Interop.EModelViewControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace ExportToPDF
{
    public partial class MainForm : Form
    {
        
    
        private string FILE_PATH = "";
        DataTable dt;
        FormBom fb = null;


        public MainForm()
        {
            InitializeComponent();
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            FILE_PATH = openFileDialog1.FileName;
            BooksRepo repo = new BooksRepo();
            dt = new DataTable();
            repo.GetTableDt(FILE_PATH, ref dt);
            this.advancedDataGridView1.AutoGenerateColumns = true;

        }

        void Data_output(DataTable dt)
        {
            this.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;
            this.Cursor = Cursors.Arrow;
            this.Activate();
        }



        public void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                Data_output(dt);
            }

            catch (System.Runtime.InteropServices.COMException ex)
            { MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            this.Show();

        }

        private void btnToPDF_Click(object sender, EventArgs e)
        {
            Convert_to_PDF(advancedDataGridView1);
        }


        private void Convert_to_PDF(ADGV.AdvancedDataGridView DG)
        {
            List<ComponentBom> listDrawingPath = new List<ComponentBom>();
            try
            {
                foreach (DataGridViewRow i in DG.Rows)
                {
                    if (i.IsNewRow) continue;
                    DataGridViewCellCollection j = i.Cells;
                    if (j[GetAssemblyID.strDraw].Value.ToString() == "1" && j[GetAssemblyID.strWhereUsed].Value.ToString() == GetAssemblyID.strSUMQTY)
                    {
                        string Found_In = j[GetAssemblyID.strFoundIn].Value.ToString();
                        string File_Name = Path.GetFileNameWithoutExtension(j[GetAssemblyID.strFileName].Value.ToString()) + ".slddrw";

                        string fileDRW = Path.Combine(Found_In, File_Name);
                        ComponentBom comBom = new ComponentBom(fileDRW, int.Parse(j[GetAssemblyID.strSUMQTY].Value.ToString()));
                        listDrawingPath.Add(comBom);
                    }
                }

                if (listDrawingPath.Count > 0)
                {
                    string str = selectFolder();
                    fb = new FormBom(listDrawingPath, str);

                    fb.ShowDialog();
                }
            }
            catch
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(" No access to file " + "\n" + saveFileDialog1.FileName.ToString());
            }
        }



        private string selectFolder()
        {
            string directory = null;
            FolderBrowserDialog DirDialog = new FolderBrowserDialog();
            DirDialog.Description = "Выбор директории";

            DirDialog.SelectedPath = @"D:\";

            if (DirDialog.ShowDialog() == DialogResult.OK)
            {
                directory = DirDialog.SelectedPath;
            }
            return directory;
        }

   
    }

}
