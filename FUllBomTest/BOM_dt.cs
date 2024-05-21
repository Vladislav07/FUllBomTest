using EPDM.Interop.epdm;
using System;
using System.Data;
using System.Text.RegularExpressions;


namespace FullBomHoum
{
    public class BOM_dt
    {
        static IEdmVault5 vault1 = new EdmVault5();

        //Из головной сборки получает dt
        public static void BOM(IEdmFile7 aFile, string config, int version, int BomFlag, int qtyAfile, ref DataTable dt)

        {
            IEdmBomView bomView;

            bomView = aFile.GetComputedBOM(GetAssemblyID.strFullBOM, version, config, BomFlag); //1//(int)EdmBomFlag.EdmBf_AsBuilt + //2// (int)EdmBomFlag.EdmBf_ShowSelected);
            bomView.GetRows(out object[] ppoRows);
            bomView.GetColumns(out EdmBomColumn[] ppoColumns);


            //Заполняем таблицу dt данными из BOM
            if (ppoRows.Length > 0)//Отбрасываем пустые сборки
            {
                //Если не заполнено заполняем заголовки колонок
                if (dt.Columns.Contains(GetAssemblyID.strPartNumber) == false)
                {
                    for (int Columni = 0; Columni < ppoColumns.Length; Columni++)
                    {
                        if (ppoColumns[Columni].mbsCaption.Contains(GetAssemblyID.strQTY) == true)
                        {
                            dt.Columns.Add(ppoColumns[Columni].mbsCaption.ToString(), typeof(Int16));
                        }
                        else
                        {
                            dt.Columns.Add(ppoColumns[Columni].mbsCaption.ToString(), typeof(string));
                        }
                    }

                    dt.Columns.Add(GetAssemblyID.strWhereUsed, typeof(string)); //+0 где используется
                    dt.Columns.Add(GetAssemblyID.strTQTY, typeof(Int16));       //+1 суммарное кол-во дет в сборке для временного подсчета
                    dt.Columns.Add(GetAssemblyID.strTotalQTY, typeof(Int16));   //+2 суммарное кол-во дет в сборке
                    dt.Columns.Add(GetAssemblyID.strDraw, typeof(Boolean));     //+3 есть ли чертеж
                    dt.Columns.Add(GetAssemblyID.strDrawState, typeof(string)); //+4 статус чертежа
                    dt.Columns.Add(GetAssemblyID.strDXF, typeof(Boolean));      //+5 есть ли IGS
                    dt.Columns.Add(GetAssemblyID.strIgs, typeof(Boolean));      //+6 есть ли IGS
                    dt.Columns.Add(GetAssemblyID.strErC, typeof(Int16));        //+7 количество ошибок в строке
                }

                //Построчно добавляем строки из BOM в таблицу dt с нужной доп информацией 
                foreach (IEdmBomCell ppoRow in ppoRows)
                {
                    ForColi(ppoRow, ppoColumns, aFile.Name.ToString(), qtyAfile, dt);
                }
            }

        }

        static void ForColi(IEdmBomCell Row, EdmBomColumn[] ppoColumns, string aFileName, int qtyAfile, DataTable dt)
        {
            string f = "";//Found In
            IEdmFile7 bFile;


            bool TrueRowFlag = false;
            // bool TrueRowFlagIGS = false;
            DataRow workRow = dt.NewRow();
            object poValue = null;
            object poComputedValue = null;
            string pbsConfiguration = "";
            bool pbReadOnly = false;
            bool refDXF = false;
            bool refIgs = false;

            if (Row.GetTreeLevel() == 1 || Row.GetTreeLevel() == 0)
            {
                for (int Coli = 0; Coli < ppoColumns.Length; Coli++)

                {
                    //Found In свойство в PDM в спецификации должно быть выше (левее), чем свойство Name     
                    if (ppoColumns[Coli].mbsCaption.Contains(GetAssemblyID.strFoundIn))
                    {
                        Row.GetVar(ppoColumns[Coli].mlVariableID, ppoColumns[Coli].meType, out poValue, out poComputedValue, out pbsConfiguration, out pbReadOnly);
                        f = poComputedValue.ToString();

                    }

                    //Если деталь или сборка, то вносим в таблицу с информацией о наличии чертежа и dxf, иначе игнорим
                    if (ppoColumns[Coli].mbsCaption.Contains(GetAssemblyID.strFileName))
                    {

                        Row.GetVar(ppoColumns[Coli].mlVariableID, ppoColumns[Coli].meType, out poValue, out poComputedValue, out pbsConfiguration, out pbReadOnly);
                        string p = f + "\\" + poComputedValue.ToString();       //Путь к файлу детали или сборки
                        string d = "";                                          //Путь к файлу чертежа


                        if (poComputedValue.ToString().Contains(".sldasm"))
                        {
                            TrueRowFlag = true; d = p.Replace(".sldasm", ".SLDDRW");
                        }
                        else if (poComputedValue.ToString().Contains(".SLDASM"))
                        {
                            TrueRowFlag = true; d = p.Replace(".SLDASM", ".SLDDRW");
                        }
                        else if (poComputedValue.ToString().Contains(".sldprt"))
                        {
                            TrueRowFlag = true; d = p.Replace(".sldprt", ".SLDDRW");
                            //Если dxf файл существует в pdm, он один и зачекинен, то refDXF = true
                            GetReferencedFiles(null, p, 0, "A", ref refDXF, ref refIgs);
                            if (refDXF == true)
                            {
                                workRow[GetAssemblyID.strDXF] = refDXF;
                            }

                            if (refIgs == true)
                            {
                                workRow[GetAssemblyID.strIgs] = refIgs;
                            }
                        }
                        else if (poComputedValue.ToString().Contains(".SLDPRT"))
                        {
                            TrueRowFlag = true; d = p.Replace(".SLDPRT", ".SLDDRW");
                            //Если dxf файл существует в pdm, он один и зачекинен, то refDXF = true
                            GetReferencedFiles(null, p, 0, "A", ref refDXF, ref refIgs);
                            if (refDXF == true)
                            {
                                workRow[GetAssemblyID.strDXF] = refDXF;
                            }

                            if (refIgs == true)
                            {
                                workRow[GetAssemblyID.strIgs] = refIgs;
                            }
                        }


                        //Проверяем есть ли зачекиненный чертеж в папке с деталью с именем соответствующим детали
                        if (!vault1.IsLoggedIn) { vault1.LoginAuto(GetAssemblyID.pdmName, 0); }
                        bFile = (IEdmFile7)vault1.GetFileFromPath(d, out IEdmFolder5 bFolder);
                        if ((bFile != null) && (!bFile.IsLocked)) //true если файл не пусто и зачекинен                                           
                        { workRow[GetAssemblyID.strDraw] = true; workRow[GetAssemblyID.strDrawState] = bFile.CurrentState.Name.ToString(); 

                            // Достаем из чертежа версию ссылки на родителя (VersionRef)
                            IEdmReference5 ref5 = bFile.GetReferenceTree(bFolder.ID);
                            IEdmReference10 ref10 = (IEdmReference10)ref5;
                            IEdmPos5 pos = ref10.GetFirstChildPosition3("A", true, true, (int)EdmRefFlags.EdmRef_File, "", 0);
                            while (!pos.IsNull)
                            {

                                IEdmReference10 @ref = (IEdmReference10)ref5.GetNextChild(pos);
                                Console.Write("VersionRef - " + @ref.VersionRef + "\r\n");   // версия ссылки на родителя, для дебага, можно удалить
                                workRow[GetAssemblyID.strRev] = @ref.VersionRef.ToString();
                            }
                        }
                        // //Получаем и вносим в таблицу ID файла вручную
                        //   cFile = (IEdmFile7)vault1.GetFileFromPath(p, out IEdmFolder5 cFolder);
                        //  if (cFile != null) //true если файл не пусто                                      
                        //  { workRow[GetAssemblyID.strFileID] = cFile.ID.ToString(); }

                    }


                    //Если TrueRowFlag true заполняем QTY
                    if (ppoColumns[Coli].mbsCaption.Contains(GetAssemblyID.strQTY))
                    {
                        Row.GetVar(ppoColumns[Coli].mlVariableID, ppoColumns[Coli].meType, out poValue, out poComputedValue, out pbsConfiguration, out pbReadOnly);
                        workRow[GetAssemblyID.strTQTY] = qtyAfile * Convert.ToInt16(poComputedValue);
                    }

                    //Заполняем колонки
                    Row.GetVar(ppoColumns[Coli].mlVariableID, ppoColumns[Coli].meType, out poValue, out poComputedValue, out pbsConfiguration, out pbReadOnly);
                    workRow[Coli] = poComputedValue.ToString();

                    //Вносим информацию в Where Used
                    switch (Row.GetTreeLevel())
                    {
                        case 0: workRow[GetAssemblyID.strWhereUsed] = ""; break;
                        case 1: workRow[GetAssemblyID.strWhereUsed] = aFileName; break;
                        default: break;
                    }
                }
            }

            else
            { TrueRowFlag = false; }//если не первый левел или не 0 левел


            if (TrueRowFlag == true)
            {
                //ПРОВЕРКИ
                workRow[GetAssemblyID.strErC] = 0;//Количество ошибок в строке

                string regCuby = @"^CUBY-\d{8}$";
                string fileName = workRow[GetAssemblyID.strFileName].ToString();
                string[] parts = fileName.Split('.');
                string cuteFileName = parts[0].ToString();
                bool IsCUBY = Regex.IsMatch(cuteFileName, regCuby);

                //1. Проверка на наличие чертежа
                if (workRow[GetAssemblyID.strDraw].ToString() == ""
                    && (workRow[GetAssemblyID.strSection].ToString() == "Детали"
                    || workRow[GetAssemblyID.strSection].ToString() == "Сборочные единицы")
                    && (workRow[GetAssemblyID.strState].ToString() != GetAssemblyID.strPrelim)
                    && workRow[GetAssemblyID.strNoSHEETS].ToString() != "1")
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; } //Количество ошибок в строке

                //2. Проверка на наличие DXF
                if (workRow[GetAssemblyID.strDXF].ToString() == ""
                    && workRow[GetAssemblyID.strSection].ToString() == "Детали"
                    && workRow[GetAssemblyID.strLaserCut].ToString() == "1")
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //2.1 Проверка на наличие IGS
                if (workRow[GetAssemblyID.strIgs].ToString() == ""
                   && workRow[GetAssemblyID.strSection].ToString() == "Детали"
                   && workRow[GetAssemblyID.str3DCuting].ToString() == "1")
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //3. Проверка на виртуальные детали
                if (workRow[GetAssemblyID.strPartNumber].ToString().Contains("^") == true)
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //5. Проверка заапрувленных покупных
                if (workRow[GetAssemblyID.strState].ToString() != "Kanban")
                {
                    if ((workRow[GetAssemblyID.strState].ToString() != "Approved to use")
                            && (workRow[GetAssemblyID.strSection].ToString() == "Стандартные изделия"
                            || workRow[GetAssemblyID.strSection].ToString() == "Прочие изделия"
                            || workRow[GetAssemblyID.strSection].ToString() == "Материалы"))
                    {
                        if ((workRow[GetAssemblyID.strState].ToString() != "Check library item")
                                && (workRow[GetAssemblyID.strSection].ToString() == "Стандартные изделия"
                                || workRow[GetAssemblyID.strSection].ToString() == "Прочие изделия"
                                || workRow[GetAssemblyID.strSection].ToString() == "Материалы"))
                        {
                            workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1;
                        }
                    }

                }

                if (IsCUBY && (workRow[GetAssemblyID.strSection].ToString() == "Стандартные изделия"
                            || workRow[GetAssemblyID.strSection].ToString() == "Прочие изделия"
                            || workRow[GetAssemblyID.strSection].ToString() == "Материалы"))
                {
                    workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1;
                }

                /*//Проверка массы >0
                float.TryParse(workRow[GetAssemblyID.strWeight].ToString(), System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out float W);
                if (W <= 0)
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC])+1; }//Количество ошибок в строке*/


                //6. Проверка есть галочка нанесения покрытия, но покрытие не указано
                if ((workRow[GetAssemblyID.strCoatApp].ToString() == "1")
                    && (workRow[GetAssemblyID.strCoating].ToString() == ""
                    || workRow[GetAssemblyID.strCoating].ToString().ToUpper() == "NONE"))
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; } //Количество ошибок в строке


                //7. Проверка нет галочки нанесения покрытия, но покрытие указано
                if ((workRow[GetAssemblyID.strCoatApp].ToString() == "0")
                    && (workRow[GetAssemblyID.strCoating].ToString() != ""
                    && workRow[GetAssemblyID.strCoating].ToString().ToUpper() != "NONE"))
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; } //Количество ошибок в строке


                //8. Проверка деталей и сборок в статусе Initiated
                if ((workRow[GetAssemblyID.strSection].ToString() == "Детали" || workRow[GetAssemblyID.strSection].ToString() == "Сборочные единицы")
                    && workRow[GetAssemblyID.strState].ToString() == GetAssemblyID.strInitiated)
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //8.1 Проверка деталей и сборок в статусах "r_In Work" "r_На проверке" "Подтверждение ведущего" (статусы потока удаленщиков, не должно быть в таком)
                if ((workRow[GetAssemblyID.strSection].ToString() == "Детали" || workRow[GetAssemblyID.strSection].ToString() == "Сборочные единицы")
                    && (workRow[GetAssemblyID.strState].ToString() == "r_In Work" ||
                        workRow[GetAssemblyID.strState].ToString() == "r_На проверке" ||
                        workRow[GetAssemblyID.strState].ToString() == "Подтверждение Ведущего"))
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //9. Проверка покупных в статусе In Work
                if ((workRow[GetAssemblyID.strSection].ToString() == "Прочие изделия"
                    || workRow[GetAssemblyID.strSection].ToString() == "Стандартные изделия"
                    || workRow[GetAssemblyID.strSection].ToString() == "Материалы")
                        && workRow[GetAssemblyID.strState].ToString() == GetAssemblyID.strInWork)
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //10. Проверка равенства состояния чертежа и детали или сборки
                if (workRow[GetAssemblyID.strDraw].ToString() == "True"
                    && workRow[GetAssemblyID.strState].ToString() != workRow[GetAssemblyID.strDrawState].ToString())
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //11. Проверка на заполнение Description, Description_RUS
                if (workRow[GetAssemblyID.strDescription_ENG].ToString().Length < 3
                    && workRow[GetAssemblyID.strDescription_RUS].ToString().Length < 3)
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //12. Проверка заполнения соответствия имени файла маске CUBY-12345678
                //regCuby = @"^CUBY-\d{8}$";
                if (!Regex.IsMatch(cuteFileName, regCuby)
                    && (workRow[GetAssemblyID.strSection].ToString() == "Детали" || workRow[GetAssemblyID.strSection].ToString() == "Сборочные единицы"))
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //4. Проверка на соответствие обозначения и имени файла
                string pathFileName = System.IO.Path.GetFileNameWithoutExtension(workRow[GetAssemblyID.strFileName].ToString());
                string CUBY_PN = workRow[GetAssemblyID.strPartNumber].ToString();
                string configName = workRow[GetAssemblyID.strConfig].ToString();

                if (workRow[GetAssemblyID.strSection].ToString() == "Детали" || workRow[GetAssemblyID.strSection].ToString() == "Сборочные единицы")
                {
                    if (configName == ".")
                    {
                        if (CUBY_PN != pathFileName)
                        {
                            workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1;
                        }
                    }
                    else
                    {
                        if (CUBY_PN != pathFileName + configName)
                        {
                            workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1;
                        }
                    }
                }

                //13. Section не пусто
                if (workRow[GetAssemblyID.strSection].ToString().Length < 3)
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //14. Проверка на заполнение в детали хотя бы одного типа обработки
                if (
                         workRow[GetAssemblyID.strSection].ToString() == "Детали"
                     && (workRow[GetAssemblyID.strLaserCut].ToString() == "0") //1
                                                                               //&& (workRow[GetAssemblyID.strCoatApp].ToString() == "0") //2 Вариант что может быть чертеж на деталь или сборку по которому только красят НЕ рассматриваем
                     && (workRow[GetAssemblyID.strLockOp].ToString() == "0")//3
                     && (workRow[GetAssemblyID.strTurning].ToString() == "0")//4
                     && (workRow[GetAssemblyID.strMetalBend].ToString() == "0")//5
                     && (workRow[GetAssemblyID.strCasting].ToString() == "0")//6
                     && (workRow[GetAssemblyID.strMilling].ToString() == "0")//7
                     && (workRow[GetAssemblyID.strVacForm].ToString() == "0")//8
                     && (workRow[GetAssemblyID.strWelding].ToString() == "0")//9
                     && (workRow[GetAssemblyID.strSticker].ToString() == "0")//10
                     && (workRow[GetAssemblyID.strPCB].ToString() == "0")//11
                     && (workRow[GetAssemblyID.str3DPrint].ToString() == "0")//12
                     && (workRow[GetAssemblyID.strReservePart1].ToString() == "0")//13
                     && (workRow[GetAssemblyID.strReservePart2].ToString() == "0")//14
                     && (workRow[GetAssemblyID.str3DCuting].ToString() == "0")
                     )
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //15. Проверка на заполнение в сборке хотя бы одного типа обработки
                if (
                        workRow[GetAssemblyID.strSection].ToString() == "Сборочные единицы"
                    && (workRow[GetAssemblyID.strGlue].ToString() == "0") //1
                                                                          //&& (workRow[GetAssemblyID.strCoatApp].ToString() == "0") //2  Вариант что может быть чертеж на деталь или сборку по которому только красят НЕ рассматриваем
                    && (workRow[GetAssemblyID.strLockOp].ToString() == "0")//3
                    && (workRow[GetAssemblyID.strTurning].ToString() == "0")//4
                    && (workRow[GetAssemblyID.strSoldering].ToString() == "0")//5
                    && (workRow[GetAssemblyID.strAssembly].ToString() == "0")//6
                    && (workRow[GetAssemblyID.strMilling].ToString() == "0")//7
                    && (workRow[GetAssemblyID.strWelding].ToString() == "0")//8
                    && (workRow[GetAssemblyID.strReserveAss1].ToString() == "0")//9
                    && (workRow[GetAssemblyID.strReserveAss2].ToString() == "0")//10
                     )
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //16. Проверка заполнения Материала в деталях
                if (workRow[GetAssemblyID.strSection].ToString() == "Детали"
                    && workRow[GetAssemblyID.strMaterial].ToString() == "Material <not specified>")
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //17. Проверка заполнения Толщины у листовых деталей у которых указана лазерная резка
                if (workRow[GetAssemblyID.strSection].ToString() == "Детали"
                    && workRow[GetAssemblyID.strShape].ToString() == "Лист"
                    && workRow[GetAssemblyID.strLaserCut].ToString() == "1"
                    && workRow[GetAssemblyID.strThickness].ToString() == "")
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке


                //18. Проверка на одинаковую толщину strShape и strThickness
                string strSortament = workRow[GetAssemblyID.strSortament].ToString();
                string[] sortament = strSortament.Split('х');

                if (workRow[GetAssemblyID.strShape].ToString() == "Лист"
                   && (workRow[GetAssemblyID.strThickness].ToString() != sortament[0].ToString().Trim()))
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                //19. Проверка на наличие длины трубы при 3D-лазерной резке
                if (workRow[GetAssemblyID.strLengthPipe].ToString() == ""
                   && workRow[GetAssemblyID.strSection].ToString() == "Детали"
                   && workRow[GetAssemblyID.str3DCuting].ToString() == "1")
                { workRow[GetAssemblyID.strErC] = Convert.ToInt16(workRow[GetAssemblyID.strErC]) + 1; }//Количество ошибок в строке

                dt.Rows.Add(workRow);
            }

            if (Row.GetTreeLevel() == 1)
            {
                if (workRow[dt.Columns.IndexOf(GetAssemblyID.strFileName)].ToString().Contains(".sldasm") || workRow[dt.Columns.IndexOf(GetAssemblyID.strFileName)].ToString().Contains(".SLDASM"))
                {
                    string conf = workRow[dt.Columns.IndexOf(GetAssemblyID.strConfig)].ToString();
                    int vers = Convert.ToInt16(workRow[dt.Columns.IndexOf(GetAssemblyID.strLatestVer)]);//Последняя версия файла в PDM
                                                                                                        // int vers = Convert.ToInt16(workRow[dt.Columns.IndexOf(GetAssemblyID.strFoundInVer)]); //Версия используемая в сборке
                    string pathFile = workRow[dt.Columns.IndexOf(GetAssemblyID.strFoundIn)].ToString() + "\\" + workRow[dt.Columns.IndexOf(GetAssemblyID.strFileName)].ToString();
                    int qtyZfile = Convert.ToInt16(workRow[dt.Columns.IndexOf(GetAssemblyID.strTQTY)]);
                    if (!vault1.IsLoggedIn) { vault1.LoginAuto(GetAssemblyID.pdmName, 0); }
                    IEdmFile7 zFile = (IEdmFile7)vault1.GetFileFromPath(pathFile, out IEdmFolder5 zFolder);

                    if (zFile != null) { BOM(zFile, conf, vers, 0, qtyZfile, ref dt); }//1//(int)EdmBomFlag.EdmBf_AsBuilt + //2// (int)EdmBomFlag.EdmBf_ShowSelected);
                }
            }

            void GetReferencedFiles(IEdmReference10 Reference, string FilePath, int Lev, string ProjectName, ref bool rdxf, ref bool rigs)
            {
                bool Top = false;

                if (Reference == null)
                {
                    Top = true;
                    if (!vault1.IsLoggedIn) { vault1.LoginAuto(GetAssemblyID.pdmName, 0); }

                    IEdmFile5 File = null;
                    File = vault1.GetFileFromPath(FilePath, out IEdmFolder5 ParentFolder);
                    if (File != null)  //true если файл не null(обход виртуальных файлов)
                    {
                        Reference = (IEdmReference10)File.GetReferenceTree(ParentFolder.ID);
                        GetReferencedFiles(Reference, FilePath, Lev + 1, "A", ref rdxf, ref rigs);
                    }
                }
                else
                {
                    IEdmPos5 pos = default(IEdmPos5);
                    IEdmReference10 Reference2 = Reference;
                    pos = Reference2.GetFirstChildPosition3(ProjectName, Top, true, (int)EdmRefFlags.EdmRef_File, "", 0);
                    IEdmReference10 @ref = default(IEdmReference10);

                    string currentFileName = System.IO.Path.GetFileNameWithoutExtension(Reference.Name.ToString());
                    string[] allConfigCurrentFile = getAllConfigFromFile(FilePath, currentFileName);

                    int counterRefConfigDXF = 0;
                    int counterRefConfigIGS = 0;

                    while (!pos.IsNull)
                    {
                        @ref = (IEdmReference10)Reference.GetNextChild(pos);
                        if (@ref.Name.ToString().Contains("Cut-List-Item") || @ref.Name.ToString().Contains("Sheet"))
                        {
                            continue;
                        }
                        else
                        {
                            string refName = @ref.Name.ToString().Substring(0, @ref.Name.ToString().LastIndexOf('.'));
                            if (@ref.Name.ToString().Contains(".dxf") || @ref.Name.ToString().Contains(".DXF"))
                            {
                                if (isExistRefInListConfig(allConfigCurrentFile, refName))
                                {
                                    counterRefConfigDXF++;
                                }
                                else
                                {
                                    counterRefConfigDXF--;
                                }
                            }
                            if (@ref.Name.ToString().Contains(".igs") || @ref.Name.ToString().Contains(".IGS"))
                            {
                                if (isExistRefInListConfig(allConfigCurrentFile, refName))
                                {
                                    counterRefConfigIGS++;
                                }
                                else
                                {
                                    counterRefConfigIGS--;
                                }
                            }
                        }
                    }

                    if (allConfigCurrentFile.Length == counterRefConfigDXF)
                    {
                        rdxf = true;
                    }

                    if (allConfigCurrentFile.Length == counterRefConfigIGS)
                    {
                        rigs = true;
                    }
                }
            }

            string[] getAllConfigFromFile(string filePath, string refFileName)
            {
                IEdmFile14 aFile = default(IEdmFile14);
                IEdmFolder5 ppoRetParentFolder;
                aFile = (IEdmFile14)vault1.GetFileFromPath(filePath, out ppoRetParentFolder);


                IEdmFolder5 folder = default(IEdmFolder5);
                folder = vault1.RootFolder;

                EdmStrLst5 cfgList = default(EdmStrLst5);
                cfgList = aFile.GetConfigurations();

                IEdmPos5 pos = default(IEdmPos5);
                pos = cfgList.GetHeadPosition();
                string cfgName = null;

                string result = "";
                while (!pos.IsNull)
                {
                    cfgName = cfgList.GetNext(pos);

                    if (cfgName == ".")
                    {
                        result = result + refFileName + ";";
                    }
                    else if (!cfgName.Contains("@") && !cfgName.Contains("-welded") && !cfgName.Contains("SM-FLAT-PATTERN"))
                    {

                        result = result + refFileName + cfgName + ";";
                    }
                }

                return result.Substring(0, result.Length - 1).Split(';');
            }

            bool isExistRefInListConfig(string[] listConfig, string refName)
            {
                bool result = false;

                foreach (string name in listConfig)
                {
                    if (name == refName)
                    {
                        result = true;
                        break;
                    }
                }

                return result;
            }
        }
    }
}
