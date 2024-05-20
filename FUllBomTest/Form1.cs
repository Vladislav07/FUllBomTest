using System;
using System.Data;
using EPDM.Interop.epdm;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using ExportToPDF;
using System.IO;

namespace FullBomHoum
{
    public partial class Form1 : Form
    {
        public string pathname0;

        public System.Data.DataTable dt2;
        string config = "";

        IEdmFile7 aFile;
        IEdmFolder5 aFolder;
        IEdmBomMgr bomMgr;
        int load = 0;
        int version = 0;
        int lw;
        int sumEr = 0;

        public IEdmVault5 vault1 = new EdmVault5();
        public IEdmVault7 vault2 = null;

        int rowerfilt = 0;

        public Form1()
        {
            InitializeComponent();
            //Всплывающие подсказки
            createToolTip(this.VP, "Full purchase list");
            createToolTip(this.VD, "Only parts list");
            createToolTip(this.PM, "Purchase list without Kanban");
            createToolTip(this.BOM, "BOM");
            createToolTip(this.Error_Filter, "List of rows containing errors");
            createToolTip(this.button3, "Resets all filters");
            createToolTip(this.button2, "Re-generates the list");
            createToolTip(this.button1, "Export to Excel");

            lw = label7.Width * 2;
            this.advancedDataGridView1.AutoGenerateColumns = true;

            vault2 = (IEdmVault7)vault1;

            if (!vault1.IsLoggedIn) { vault1.LoginAuto(GetAssemblyID.pdmName, this.Handle.ToInt32()); }

            aFile = (IEdmFile7)vault1.GetObject(EdmObjectType.EdmObject_File, GetAssemblyID.ASMID);
            aFolder = (IEdmFolder5)vault1.GetObject(EdmObjectType.EdmObject_Folder, GetAssemblyID.ASMFolderID);

            // Заполняем при запуске в первый раз и рефреше
            if (load == 0)
            {
                pathname0 = aFolder.LocalPath;
                this.Text = (pathname0 + "\\" + aFile.Name);

                FillV(aFile.CurrentVersion);

                EdmStrLst5 cfgList = default(EdmStrLst5);
                cfgList = aFile.GetConfigurations();
                IEdmPos5 pos = default(IEdmPos5);
                pos = cfgList.GetHeadPosition();
                while (!pos.IsNull)
                { comboBox2.Items.Add(cfgList.GetNext(pos)); }
                if (comboBox2.FindStringExact(GetAssemblyID.strConfigPoint) != -1)
                { comboBox2.SelectedItem = GetAssemblyID.strConfigPoint; }
                else
                { comboBox2.SelectedIndex = 1; }

                bomMgr = (IEdmBomMgr)vault2.CreateUtility(EdmUtility.EdmUtil_BomMgr);
                bomMgr.GetBomLayouts(out EdmBomLayout[] ppoRetLayouts);

                for (int i = 0; i < ppoRetLayouts.Length; i++)
                { comboBox3.Items.Add(ppoRetLayouts[i].mbsLayoutName); }

                if (comboBox3.FindStringExact(GetAssemblyID.strFullBOM) != -1)
                { comboBox3.SelectedItem = GetAssemblyID.strFullBOM; }
                else
                { MessageBox.Show("Add the " + GetAssemblyID.strFullBOM + " templete to Bills of materials"); Environment.Exit(0); }

                comboBox4.Items.Add("");

                VR2("", aFile.CurrentVersion);//Заполняет ревизию

            }

        }
        void FillV(int FillVersion)
        {
            for (int k = 1; k <= FillVersion; k++)
            { if (comboBox1.FindStringExact(FillVersion.ToString()) == -1) { comboBox1.Items.Add(k.ToString()); } }
        }
        void VR2(string revisia, int versia)
        {
            string REV = "";
            int VER = versia;

            IEdmEnumeratorVersion5 verEnum = default(IEdmEnumeratorVersion5);
            verEnum = (IEdmEnumeratorVersion5)aFile;
            IEdmRevision5 ReV = default(IEdmRevision5);
            IEdmPos5 pos = default(IEdmPos5);
            pos = verEnum.GetFirstRevisionPosition();



            while (!pos.IsNull)
            {
                ReV = verEnum.GetNextRevision(pos);

                if (comboBox4.FindStringExact(ReV.Name) == -1) //если combobox не содержит имя ревизии, добавляет ее
                { comboBox4.Items.Add(ReV.Name); }

                if (ReV.Name == revisia || ReV.VersionNo == versia)
                {
                    REV = ReV.Name;
                    VER = ReV.VersionNo;
                    break;
                }

                if (revisia == "" && versia == 0)

                {
                    REV = "";
                    VER = aFile.CurrentVersion;
                }

                else
                {
                    REV = "";
                    VER = versia;
                }

            }
            comboBox1.SelectedItem = VER.ToString();
            comboBox4.SelectedItem = REV;
        }


        void Data_output()
        {
            this.Cursor = Cursors.WaitCursor;
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();////Инициализируем таймер
            //sw.Start();////Запускаем таймер
            config = comboBox2.SelectedItem.ToString();
            version = Convert.ToInt16(comboBox1.SelectedItem);
            System.Data.DataTable dt = new System.Data.DataTable();


            BOM_dt.BOM(aFile, config, version, 2, 1, ref dt);///1//(int)EdmBomFlag.EdmBf_AsBuilt + //2// (int)EdmBomFlag.EdmBf_ShowSelected);
            PostProcessing.PostProcess(ref dt);
            this.bindingSource1.DataSource = dt;

            //Запрет изменения значения в ячейках
            for (int i = 0; i < dt.Columns.Count; i++)
            { advancedDataGridView1.Columns[i].ReadOnly = true; }
            //Разрешаем изменение Er
            advancedDataGridView1.Columns[GetAssemblyID.strErC].ReadOnly = false;

            //Выравнивание колонок  в datagrid
            this.advancedDataGridView1.Columns[GetAssemblyID.strQTY].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.advancedDataGridView1.Columns[GetAssemblyID.strErC].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.advancedDataGridView1.Columns[GetAssemblyID.strTotalQTY].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.bindingSource1.Sort = GetAssemblyID.strSection + "," + GetAssemblyID.strPartNumber + "," + GetAssemblyID.strDescription_RUS;
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            this.label6.Text = " of " + dt.Rows.Count.ToString() + " rows";
            this.label8.Left = this.label7.Left; SumError(out sumEr);
            this.label8.Text = sumEr.ToString() + " errors";
            ChangeColor(advancedDataGridView1);
            //sw.Stop();////Останавливаем таймер
            //MessageBox.Show(Convert.ToString(sw.ElapsedMilliseconds) + " мс секунд"); ////Отображаем время в милисекундах

            load = 1;
            this.Cursor = Cursors.Arrow;
        }
        private void Label7_SizeChanged(object sender, EventArgs e) { label7.Left -= label7.Width - lw; lw = label7.Width; label8.Left = label7.Left; }
        private void Label8_SizeChanged(object sender, EventArgs e) { label8.Left = label7.Left; }


        private void ChangeColor(ADGV.AdvancedDataGridView DG)//Раскрашиваем DataGrid
        {

            for (int i = 0; i < DG.Rows.Count; i++)
            {

                string regCuby = @"^CUBY-\d{8}$";
                string fileName = DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strFileName].Index].Value.ToString();
                string[] parts = fileName.Split('.');
                string cuteFileName = parts[0].ToString();
                bool IsCUBY = Regex.IsMatch(cuteFileName, regCuby);

                //0. Изменение цвета ячейки в колонке с ошибками
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strErC].Index].Value.ToString() != "0") //Количество ошибок в строке
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strErC].Index].Style = GetAssemblyID.cellStyleErr; }

                //1. Проверка на наличие чертежа
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDraw].Index].Value.ToString() == ""
                 && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() != GetAssemblyID.strPrelim
                 && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                 || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы")
                 && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strNoSHEETS].Index].Value.ToString() != "1")
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDraw].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strNoSHEETS].Index].Style = GetAssemblyID.cellStyleErr;
                }
                //  else if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDraw].Index].Value.ToString() == "True" & (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали" || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы"))
                // { DG.Rows[i].DefaultCellStyle.BackColor = Color.Honeydew;} //Раскрашивать зеленым если есть чертеж


                //2. Проверка на наличие DXF
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDXF].Index].Value.ToString() == ""
                    && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали")
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLaserCut].Index].Value.ToString() == "1")
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDXF].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLaserCut].Index].Style = GetAssemblyID.cellStyleErr;
                }


                //2.1. Проверка на наличие IGS
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strIgs].Index].Value.ToString() == ""
                    && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали")
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DCuting].Index].Value.ToString() == "1")
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strIgs].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DCuting].Index].Style = GetAssemblyID.cellStyleErr;
                }


                //3. Проверка на виртуальные детали
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPartNumber].Index].Value.ToString().Contains("^") == true)
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPartNumber].Index].Style = GetAssemblyID.cellStyleErr; }

                string strFileName = DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strFileName].Index].Value.ToString();
                string splitStrFileName = strFileName.Substring(0, strFileName.IndexOf('.'));


                //5. Проверка заапрувленных покупных
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() != "Kanban")
                {
                    if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() != "Approved to use")
                        && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Стандартные изделия"
                        || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Прочие изделия"
                        || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Материалы"))
                    {
                        if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() != "Check library item")
                                && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Стандартные изделия"
                                || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Прочие изделия"
                                || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Материалы"))
                        {
                            DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Style = GetAssemblyID.cellStyleErr;
                        }
                    }
                }

                if (IsCUBY && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Стандартные изделия"
                         || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Прочие изделия"
                         || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Материалы"))
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strFileName].Index].Style = GetAssemblyID.cellStyleErr;
                }

                /*
                //Проверка массы >0
               float W;
               float.TryParse(DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strWeight].Index].Value.ToString(), System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out W);
               if (W <=0) {DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strWeight].Index].Style = GetAssemblyID.cellStyleErr;}
               */

                //6. Проверка есть галочка нанесения покрытия, но покрытие не указано
                if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Value.ToString() == "1")
                && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoating].Index].Value.ToString() == ""
                || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoating].Index].Value.ToString().ToUpper() == "NONE"))

                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoating].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Style = GetAssemblyID.cellStyleErr;
                }

                //7. Проверка нет галочки нанесения покрытия, но покрытие указано
                if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Value.ToString() == "0")
                && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoating].Index].Value.ToString() != ""
                && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoating].Index].Value.ToString().ToUpper() != "NONE")
                )
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoating].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Style = GetAssemblyID.cellStyleErr;
                }

                //8. Проверка деталей и сборок в статусе Initiated
                if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы")
                && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() == GetAssemblyID.strInitiated)
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Style = GetAssemblyID.cellStyleErr; }


                //8.1 Проверка деталей и сборок в статусах "r_In Work" "r_На проверке" "Подтверждение Ведущего" (статусы потока удаленщиков, не должно быть в таком)
                if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали" ||
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы")
                && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() == "r_In Work" ||
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() == "r_На проверке" ||
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() == "Подтверждение Ведущего"))
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Style = GetAssemblyID.cellStyleErr; }

                //9. Проверка покупных в статусе In Work
                if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Прочие изделия"
                || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Стандартные изделия"
                || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Материалы")
                && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString() == GetAssemblyID.strInWork)
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Style = GetAssemblyID.cellStyleErr; }


                //10. Проверка равенства состояния чертежа и детали или сборки
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDraw].Index].Value.ToString() == "True"
                && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Value.ToString()
                != DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDrawState].Index].Value.ToString())
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strState].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDrawState].Index].Style = GetAssemblyID.cellStyleErr;
                }


                //11. Проверка на заполнение Description
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDescription_ENG].Index].Value.ToString().Length < 3
                && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDescription_RUS].Index].Value.ToString().Length < 3)
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDescription_ENG].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strDescription_RUS].Index].Style = GetAssemblyID.cellStyleErr;
                }


                //12. Проверка заполнения соответствия имени файла маске CUBY-12345678
                //regCuby = @"^CUBY-\d{8}$";
                if ((DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                    || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы")
                        && !IsCUBY)
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strFileName].Index].Style = GetAssemblyID.cellStyleErr;
                }


                //4. Проверка на соответствие обозначения и имени файла
                string configName = DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strConfig].Index].Value.ToString();

                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                    || DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы")
                {
                    if (configName == ".")
                    {
                        if (cuteFileName != DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPartNumber].Index].Value.ToString())
                        {
                            DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPartNumber].Index].Style = GetAssemblyID.cellStyleErr;
                            DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strFileName].Index].Style = GetAssemblyID.cellStyleErr;
                        }
                    }
                    else
                    {
                        if (cuteFileName + configName != DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPartNumber].Index].Value.ToString())
                        {
                            DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPartNumber].Index].Style = GetAssemblyID.cellStyleErr;
                            DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strFileName].Index].Style = GetAssemblyID.cellStyleErr;
                            DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strConfig].Index].Style = GetAssemblyID.cellStyleErr;
                        }
                    }
                }

                //13. Section не пусто
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString().Length < 3)
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Style = GetAssemblyID.cellStyleErr; }//Количество ошибок в строке


                //14. Проверка на заполнение в детали хотя бы одного типа обработки
                if (
                   DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLaserCut].Index].Value.ToString() == "0") //1
                                                                                                              //&& (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Value.ToString() == "0") //2 Вариант что может быть чертеж на деталь или сборку по которому только красят НЕ рассматриваем
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLockOp].Index].Value.ToString() == "0")//3
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strTurning].Index].Value.ToString() == "0")//4
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMetalBend].Index].Value.ToString() == "0")//5
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCasting].Index].Value.ToString() == "0")//6
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMilling].Index].Value.ToString() == "0")//7
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strVacForm].Index].Value.ToString() == "0")//8
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strWelding].Index].Value.ToString() == "0")//9
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSticker].Index].Value.ToString() == "0")//10
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPCB].Index].Value.ToString() == "0")//11
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DPrint].Index].Value.ToString() == "0")//12
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReservePart1].Index].Value.ToString() == "0")//13
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReservePart2].Index].Value.ToString() == "0")//14
                   && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DCuting].Index].Value.ToString() == "0")//15
                   )

                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLaserCut].Index].Style = GetAssemblyID.cellStyleErr;
                    //DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLockOp].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strTurning].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMetalBend].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCasting].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMilling].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strVacForm].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strWelding].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSticker].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strPCB].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DPrint].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReservePart1].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReservePart2].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DCuting].Index].Style = GetAssemblyID.cellStyleErr;
                }

                //15. Проверка на заполнение в сборке хотя бы одного типа обработки
                if (
                  DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Сборочные единицы"
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strGlue].Index].Value.ToString() == "0") //1
                                                                                                         //&& (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Value.ToString() == "0") //2 Вариант что может быть чертеж на деталь или сборку по которому только красят НЕ рассматриваем
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLockOp].Index].Value.ToString() == "0")//3
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strTurning].Index].Value.ToString() == "0")//4
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSoldering].Index].Value.ToString() == "0")//5
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strAssembly].Index].Value.ToString() == "0")//6
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMilling].Index].Value.ToString() == "0")//7
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strWelding].Index].Value.ToString() == "0")//8
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReserveAss1].Index].Value.ToString() == "0")//9
                  && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReserveAss2].Index].Value.ToString() == "0")//10
                   )

                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strGlue].Index].Style = GetAssemblyID.cellStyleErr;
                    //DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strCoatApp].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLockOp].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strTurning].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSoldering].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strAssembly].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMilling].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strWelding].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReserveAss1].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strReserveAss2].Index].Style = GetAssemblyID.cellStyleErr;
                }


                //16. Проверка заполнения материала для деталей
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMaterial].Index].Value.ToString() == "Material <not specified>")
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strMaterial].Index].Style = GetAssemblyID.cellStyleErr; }


                //17. Проверка заполнения Толщины у листовых деталей у которых указана лазерная резка
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strShape].Index].Value.ToString() == "Лист"
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLaserCut].Index].Value.ToString() == "1"
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strThickness].Index].Value.ToString() == "")
                { DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strThickness].Index].Style = GetAssemblyID.cellStyleErr; }

                //18. Проверка на одинаковую толщину strShape и strThickness
                string strSortament2 = DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSortament].Index].Value.ToString();
                string[] sortament2 = strSortament2.Split('х');

                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strShape].Index].Value.ToString() == "Лист"
                && (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strThickness].Index].Value.ToString() != sortament2[0].ToString().Trim()))
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strThickness].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSortament].Index].Style = GetAssemblyID.cellStyleErr;
                } //Количество ошибок в строке


                //19. Проверка на наличие длины трубы при 3D-лазерной резке
                if (DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLengthPipe].Index].Value.ToString() == ""
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strSection].Index].Value.ToString() == "Детали"
                    && DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DCuting].Index].Value.ToString() == "1")
                {
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.strLengthPipe].Index].Style = GetAssemblyID.cellStyleErr;
                    DG.Rows[i].Cells[DG.Columns[GetAssemblyID.str3DCuting].Index].Style = GetAssemblyID.cellStyleErr;
                }
            }
        }


        public void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                Data_output();
            }

            catch (System.Runtime.InteropServices.COMException ex)
            { MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        void Reset()
        {
            this.bindingSource1.Sort = GetAssemblyID.strSection + "," + GetAssemblyID.strPartNumber + "," + GetAssemblyID.strDescription_RUS;
            aFile = (IEdmFile7)vault1.GetObject(EdmObjectType.EdmObject_File, GetAssemblyID.ASMID);
            aFolder = (IEdmFolder5)vault1.GetObject(EdmObjectType.EdmObject_Folder, GetAssemblyID.ASMFolderID);
            if (load == 0) { FillV(aFile.CurrentVersion); VR2("", aFile.CurrentVersion); }
            Data_output();
        }
        private void ComboBox1_MouseWheel(object sender, MouseEventArgs e)
        { if (e is HandledMouseEventArgs ev) { ev.Handled = true; } }

        //     { HandledMouseEventArgs ev = e as HandledMouseEventArgs; if (ev != null) { ev.Handled = true; } }

        private void ComboBox2_MouseWheel(object sender, MouseEventArgs e)
        { if (e is HandledMouseEventArgs ev) { ev.Handled = true; } }

        private void ComboBox3_MouseWheel(object sender, MouseEventArgs e)
        { if (e is HandledMouseEventArgs ev) { ev.Handled = true; } }

        private void ComboBox4_MouseWheel(object sender, MouseEventArgs e)
        { if (e is HandledMouseEventArgs ev) { ev.Handled = true; } }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender; if (load == 1)
            { load = 2; VR2(null, Convert.ToInt32(comboBox1.SelectedItem.ToString())); Reset(); }
        }

        private void ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender; if (load == 1)
            { load = 2; VR2(comboBox4.SelectedItem.ToString(), 0); Reset(); }
        }

        private void AdvancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        { this.bindingSource1.Sort = this.advancedDataGridView1.SortString; ChangeColor(advancedDataGridView1); }

        private void AdvancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        { this.bindingSource1.Filter = this.advancedDataGridView1.FilterString; ChangeColor(advancedDataGridView1); this.label7.Text = advancedDataGridView1.Rows.Count.ToString(); SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors"; }

        private void Export_To_Excel_Click(object sender, EventArgs e)

        {

            Export_to_Excel(advancedDataGridView1);

        }


        private void Export_to_Excel(ADGV.AdvancedDataGridView DG)
        {

            try
            {
                //Папка по умолчанию для сохранения excel
                //while (pathname0.Contains("\\CAD")) { pathname0 = System.IO.Path.GetDirectoryName(pathname0); }    
                //saveFileDialog1.InitialDirectory = pathname0 + "\\Текстовые документы";

                saveFileDialog1.Title = "Save FullBOM as Excel File";
                saveFileDialog1.FileName = GetAssemblyID.name0 + "_v" + comboBox1.SelectedItem.ToString() + "_BOM";
                saveFileDialog1.Filter = "Excel Files(2007)|*.xlsx|Excel Files(2003)|*.xls";
                if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)

                {

                    this.Cursor = Cursors.WaitCursor;

                    Excel.Application ExcelApp = new Excel.Application();
                    Excel.Workbook ExcelWorkBook;
                    Excel.Worksheet ExcelWorkSheet;
                    ExcelWorkBook = ExcelApp.Workbooks.Add(System.Reflection.Missing.Value);

                    ExcelWorkSheet = (Excel.Worksheet)ExcelWorkBook.Worksheets.get_Item(1);


                    // Заносим заголовки в Excel

                    for (int i = 0; i < DG.Columns.Count; i++)
                    { ExcelWorkSheet.Cells[1, i + 1] = DG.Columns[i].HeaderText; }


                    //Заносим данные из датагрид в массив 
                    var dgArray = new object[DG.RowCount, DG.ColumnCount];
                    foreach (DataGridViewRow i in DG.Rows)
                    {
                        if (i.IsNewRow) continue;
                        foreach (DataGridViewCell j in i.Cells)
                        {
                            if (j.Value.ToString() == "True") dgArray[j.RowIndex, j.ColumnIndex] = "1";
                            else if (j.Value.ToString() == "False") dgArray[j.RowIndex, j.ColumnIndex] = "0";
                            else dgArray[j.RowIndex, j.ColumnIndex] = j.Value.ToString();
                        }
                    }


                    //Заносим данные из массива в Excel
                    Excel.Range range1 = (Excel.Range)ExcelWorkSheet.Cells[2, 1];
                    range1 = range1.get_Resize(dgArray.GetLength(0), dgArray.GetLength(1));
                    range1.set_Value(Excel.XlRangeValueDataType.xlRangeValueDefault, dgArray);

                    //Раскрашиваем ячейки, также как в DataGrid
                    for (int i = 0; i < DG.Rows.Count; i++)
                    {
                        for (int j = 0; j < DG.Columns.Count; j++)

                        {
                            if (DG.Rows[i].Cells[j].Style.BackColor != System.Drawing.Color.Empty)
                            {
                                Excel.Range range0 = ExcelWorkSheet.Cells[i + 2, j + 1] as Excel.Range;
                                range0.Cells.Interior.Color = System.Drawing.ColorTranslator.ToOle(GetAssemblyID.colorError);
                            }
                        }

                    }

                    //Форматируем заголовки
                    Excel.Range headers = ExcelWorkSheet.get_Range(ExcelWorkSheet.Cells[1, 1], ExcelWorkSheet.Cells[1, DG.Columns.Count]);
                    headers.Cells.Font.Name = GetAssemblyID.strTypeEx;
                    headers.Cells.Font.Bold = true;
                    headers.Cells.Font.Size = GetAssemblyID.strTypeExSize;
                    headers.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    headers.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headers.EntireColumn.AutoFit();
                    //Форматируем остальные строки
                    range1.Cells.Font.Name = GetAssemblyID.strTypeEx;
                    range1.Cells.Font.Size = GetAssemblyID.strTypeExSize;

                    //Наносим сетку
                    Border(headers, Excel.XlLineStyle.xlContinuous, System.Drawing.Color.Black);
                    Border(range1, Excel.XlLineStyle.xlContinuous, System.Drawing.Color.Black);

                    ExcelWorkBook.SaveAs(saveFileDialog1.FileName.ToString());

                    ExcelWorkBook.Saved = true;
                    ExcelApp.Visible = true;
                    ExcelApp.UserControl = true;
                    this.Cursor = Cursors.Arrow;
                }

            }

            catch
            {

                this.Cursor = Cursors.Arrow;
                MessageBox.Show(" No access to file " + "\n" + saveFileDialog1.FileName.ToString());

            }


        }
        void SumError(out int sumEr)
        {
            sumEr = 0; for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
            { sumEr = sumEr + Convert.ToInt16(advancedDataGridView1.Rows[i].Cells[advancedDataGridView1.Columns[GetAssemblyID.strErC].Index].Value); }
        }

        void Border(Excel.Range Range, Excel.XlLineStyle lineStyle, System.Drawing.Color ColorBorder)
        {
            Range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = lineStyle;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = lineStyle;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = lineStyle;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = lineStyle;
            Range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = lineStyle;
            Range.Borders.Color = System.Drawing.ColorTranslator.ToOle(ColorBorder);
        }


        private void Button2_Click(object sender, EventArgs e)//Refresh
        { this.advancedDataGridView1.ClearSort(true); load = 0; Reset(); }


        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        { ComboBox comboBox = (ComboBox)sender; if (load == 1) { Reset(); } }

        private void Button3_Click(object sender, EventArgs e)//Сброс фильтров
        {
            this.bindingSource1.RemoveFilter();
            ChangeColor(advancedDataGridView1);
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            SumError(out sumEr);
            this.label8.Text = sumEr.ToString() + " errors";
            this.advancedDataGridView1.ClearFilter(true);
        }

        private void VP_Click(object sender, EventArgs e)
        {
            //ВП
            this.bindingSource1.Filter = "([" + GetAssemblyID.strWhereUsed + "] IN ('" + GetAssemblyID.strSUMQTY + "')) AND ([" + GetAssemblyID.strSection + "] IN ('Материалы', 'Прочие изделия','Стандартные изделия'))";
            ChangeColor(advancedDataGridView1);
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors";
        }


        private void PM_Click(object sender, EventArgs e)
        {//PM список покупных без Канбан

            this.bindingSource1.Filter = "([" + GetAssemblyID.strState + "]<>'Kanban' ) AND ([" + GetAssemblyID.strWhereUsed + "] IN ('" + GetAssemblyID.strSUMQTY + "')) AND ([" + GetAssemblyID.strSection + "] IN ('Материалы', 'Прочие изделия','Стандартные изделия'))";
            ChangeColor(advancedDataGridView1);
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors";

        }


        private void VD_Click(object sender, EventArgs e)
        {
            //ВД
            this.bindingSource1.Filter = "([" + GetAssemblyID.strWhereUsed + "] IN ('" + GetAssemblyID.strSUMQTY + "')) AND ([" + GetAssemblyID.strSection + "] IN ('Детали'))";
            ChangeColor(advancedDataGridView1);
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors";
        }
        // TQ BOM
        private void TQ_Click(object sender, EventArgs e)
        {
            this.bindingSource1.Filter = "([" + GetAssemblyID.strWhereUsed + "] IN ('" + GetAssemblyID.strSUMQTY + "'))";
            ChangeColor(advancedDataGridView1);
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors";


        }
        private void Error_Filter_Click(object sender, EventArgs e)
        //Error
        {
            this.Cursor = Cursors.WaitCursor;
            this.bindingSource1.RemoveFilter();
            this.advancedDataGridView1.ClearFilter(true);
            this.bindingSource1.Filter = "([" + GetAssemblyID.strErC + "]<>0 ) AND ([" + GetAssemblyID.strWhereUsed + "] IN ('" + GetAssemblyID.strSUMQTY + "'))";// Количество ошибок в строке
            //this.bindingSource1.Filter = "(" + GetAssemblyID.strErC + "=true)";// Наличие ошибок в строке
            ChangeColor(advancedDataGridView1);
            this.label7.Text = advancedDataGridView1.Rows.Count.ToString();
            SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors";
            this.Cursor = Cursors.Arrow;
        }

        // Открываем деталь в SW по двойному клику на ячейке
        private void AdvancedDataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)

        {
            this.Cursor = Cursors.WaitCursor;

            IEdmSearch5 Search = default(IEdmSearch5);
            Search = (IEdmSearch5)vault2.CreateUtility(EdmUtility.EdmUtil_Search);
            Search.FileName = advancedDataGridView1.Rows[e.RowIndex].Cells[advancedDataGridView1.Columns[GetAssemblyID.strFileName].Index].Value.ToString();
            IEdmSearchResult5 Result = default(IEdmSearchResult5);
            EdmSelItem2 SelItem = new EdmSelItem2();
            Result = Search.GetFirstResult();

            if (Result != null) { SelItem.mlID = Result.ID; SelItem.mlParentID = Result.ParentFolderID; System.Diagnostics.Process.Start("Conisio://" + GetAssemblyID.pdmName + "/open?projectid=" + SelItem.mlParentID + "&documentid=" + SelItem.mlID + "&objecttype=1"); }
            //Если деталь или сборка виртуальные, пытаемся открыть сборку в которую она входит, через Where Used
            else
            {
                string NameOpenVirtFile = advancedDataGridView1.Rows[e.RowIndex].Cells[advancedDataGridView1.Columns[GetAssemblyID.strFileName].Index].Value.ToString();
                Search.FileName = NameOpenVirtFile.Substring(NameOpenVirtFile.LastIndexOf("^") + 1, NameOpenVirtFile.LastIndexOf(".") - NameOpenVirtFile.LastIndexOf("^")) + "sldasm"; SelItem = new EdmSelItem2(); Result = Search.GetFirstResult();
                if (Result != null) { SelItem.mlID = Result.ID; SelItem.mlParentID = Result.ParentFolderID; System.Diagnostics.Process.Start("Conisio://" + GetAssemblyID.pdmName + "/open?projectid=" + SelItem.mlParentID + "&documentid=" + SelItem.mlID + "&objecttype=1"); }
            }
            this.Cursor = Cursors.Arrow;
        }


        private void AdvancedDataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        { if (rowerfilt == 1) { this.label7.Text = advancedDataGridView1.Rows.Count.ToString(); SumError(out sumEr); this.label8.Text = sumEr.ToString() + " errors"; } }

        private void AdvancedDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e) { rowerfilt = 1; }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Проверяем, что клик был по строке (а не по заголовку)
            {
                // Снимаем выделение со всех строк
                //advancedDataGridView1.ClearSelection();

                // Выделяем только кликнутую строку
                advancedDataGridView1.Rows[e.RowIndex].Selected = true;


                // Проходимся по всем ячейкам выбранной строки
                foreach (DataGridViewCell cell in advancedDataGridView1.Rows[e.RowIndex].Cells)
                {
                    // Проверяем, имеет ли ячейка пользовательский цвет
                    if (cell.Style.BackColor != Color.LightSalmon)
                    {
                        // Если имеет, устанавливаем цвет выделения как по умолчанию
                        cell.Style.SelectionBackColor = advancedDataGridView1.DefaultCellStyle.SelectionBackColor;
                        cell.Style.SelectionForeColor = advancedDataGridView1.DefaultCellStyle.SelectionForeColor;
                    }
                    else
                    {
                        // Если нет, оставляем цвет выделения как есть
                        cell.Style.SelectionBackColor = cell.Style.BackColor;
                        cell.Style.SelectionForeColor = Color.Black;
                    }
                }
            }
        }
        private void Convert_to_PDF(ADGV.AdvancedDataGridView DG)
        {
            List<ComponentBom> listDrawingPath = new List<ComponentBom>();
            FormBom fb = null;
            try
            {
                foreach (DataGridViewRow i in DG.Rows)
                {
                    if (i.IsNewRow) continue;
                    DataGridViewCellCollection j = i.Cells;
                    if ((bool)j[GetAssemblyID.strDraw].Value == true && j[GetAssemblyID.strWhereUsed].Value.ToString() == GetAssemblyID.strSUMQTY)
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
            try
            {
               if (DirDialog.ShowDialog() == DialogResult.OK)
                {
                    directory = DirDialog.SelectedPath;
                    return directory;
                }
                return "";

            }
               
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
                return "";
            }
            
        }

        private void toPDF_Click(object sender, EventArgs e)
        {
            Convert_to_PDF(advancedDataGridView1);
        }
    }
}
