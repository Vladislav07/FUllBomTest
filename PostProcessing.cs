using System;
using System.Linq;
using System.Data;


namespace FullBomHoum
{
    class PostProcessing  // Суммируем строки, сортируем колонки в таблице
    {
        public static void PostProcess(ref DataTable dt)
        {
            DataTable dtSUM = new DataTable();
            dtSUM = dt.Clone();

            int i = 0; int j = 0;
            while (i < dt.Rows.Count - 1)
            {
                j = i + 1; while (j < dt.Rows.Count)

                {
                    if (
                        dt.Rows[i][dt.Columns.IndexOf(GetAssemblyID.strFileName)].ToString() == dt.Rows[j][dt.Columns.IndexOf(GetAssemblyID.strFileName)].ToString()
                        &&
                        dt.Rows[i][dt.Columns.IndexOf(GetAssemblyID.strConfig)].ToString() == dt.Rows[j][dt.Columns.IndexOf(GetAssemblyID.strConfig)].ToString()
                        &&
                        dt.Rows[i][dt.Columns.IndexOf(GetAssemblyID.strWhereUsed)].ToString() == dt.Rows[j][dt.Columns.IndexOf(GetAssemblyID.strWhereUsed)].ToString()
                        )
                    {
                        dt.Rows[i][dt.Columns.IndexOf(GetAssemblyID.strTQTY)] =
                            Convert.ToUInt16(dt.Rows[i][dt.Columns.IndexOf(GetAssemblyID.strTQTY)])
                            +
                            Convert.ToUInt16(dt.Rows[j][dt.Columns.IndexOf(GetAssemblyID.strTQTY)]);
                        dt.Rows.RemoveAt(j);
                    }
                    j = j + 1;
                }
                i = i + 1;
            }

            //Заполняем суммарное количество в Where Used
            for (int k = 0; k < dt.Rows.Count; k++)

            {
                dtSUM.ImportRow(dt.Rows[k]);
                dtSUM.Rows[k][dtSUM.Columns.IndexOf(GetAssemblyID.strQTY)] = DBNull.Value;
                dtSUM.Rows[k][dtSUM.Columns.IndexOf(GetAssemblyID.strTQTY)] = DBNull.Value;
                dtSUM.Rows[k][dtSUM.Columns.IndexOf(GetAssemblyID.strWhereUsed)] = GetAssemblyID.strSUMQTY;
                dtSUM.Rows[k][dtSUM.Columns.IndexOf(GetAssemblyID.strTotalQTY)] = dt.Compute("Sum([" + GetAssemblyID.strTQTY + "])",
                    GetAssemblyID.strFileName + "= '" + dt.Rows[k][dt.Columns.IndexOf(GetAssemblyID.strFileName)].ToString() + "'"
                    + " AND " +
                    GetAssemblyID.strConfig + "= '" + dt.Rows[k][dt.Columns.IndexOf(GetAssemblyID.strConfig)].ToString() + "'"
                    ).ToString();
            }

            //Если в dtSUM что-то есть, то приклеиваем dtSUM к dt
            if (dtSUM.Rows.Count > 0)
            { dt.Merge(dtSUM.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable()); }

            //Перетасовываем колонки для удобства отображения в DataGrid, удаляем TQTY

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strErC)].SetOrdinal(0);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strFileID)].SetOrdinal(1);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strState)].SetOrdinal(2);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strDraw)].SetOrdinal(3);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strDrawState)].SetOrdinal(4);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strDXF)].SetOrdinal(5);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strIgs)].SetOrdinal(6);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strSection)].SetOrdinal(7);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strRev)].SetOrdinal(8);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strLatestVer)].SetOrdinal(9);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strPartNumber)].SetOrdinal(10);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strDescription_RUS)].SetOrdinal(11);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strDescription_ENG)].SetOrdinal(12);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strWhereUsed)].SetOrdinal(13);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strQTY)].SetOrdinal(14);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strTotalQTY)].SetOrdinal(15);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strLengthPipe)].SetOrdinal(16);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strComment)].SetOrdinal(17);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strManufac)].SetOrdinal(18);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strManufacNumb)].SetOrdinal(19);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strConfig)].SetOrdinal(20);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strFoundIn)].SetOrdinal(21);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strFileName)].SetOrdinal(22);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strShape)].SetOrdinal(23);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strSortament)].SetOrdinal(24);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strMaterial)].SetOrdinal(25);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strWeight)].SetOrdinal(26);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strSurfaceArea)].SetOrdinal(27);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strThickness)].SetOrdinal(28);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strCoatApp)].SetOrdinal(29);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strCoating)].SetOrdinal(30);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strCoating2)].SetOrdinal(31);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strCoating3)].SetOrdinal(32);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strLaserCut)].SetOrdinal(33);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strMetalBend)].SetOrdinal(34);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strCasting)].SetOrdinal(35);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.str3DPrint)].SetOrdinal(36);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strVacForm)].SetOrdinal(37);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strSticker)].SetOrdinal(38);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strPCB)].SetOrdinal(39);
            //dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strReservePart1)].SetOrdinal(38);
            //dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strReservePart2)].SetOrdinal(39);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strLockOp)].SetOrdinal(40);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strWelding)].SetOrdinal(41);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strTurning)].SetOrdinal(42);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strMilling)].SetOrdinal(43);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strAssembly)].SetOrdinal(44);

            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strSoldering)].SetOrdinal(45);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strGlue)].SetOrdinal(46);
            //dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strReserveAss1)].SetOrdinal(46);
            //dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strReserveAss2)].SetOrdinal(47);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.strAnnotation)].SetOrdinal(47);
            dt.Columns[dt.Columns.IndexOf(GetAssemblyID.str3DCuting)].SetOrdinal(48);


            dt.Columns.RemoveAt(dt.Columns.IndexOf(GetAssemblyID.strTQTY));

            //Если покрытие 2 пусто, удаляем лишнюю колонку
            int coat = 0;
            for (int z = 0; z < dt.Rows.Count; z++)
            {
                if (dt.Rows[z][dt.Columns.IndexOf(GetAssemblyID.strCoating2)].ToString() != "" && dt.Rows[z][dt.Columns.IndexOf(GetAssemblyID.strCoating2)].ToString().ToUpper() != "NONE")
                {
                    coat = 1;
                    break;
                }
            }

            if (coat == 0)
            {
                dt.Columns.RemoveAt(dt.Columns.IndexOf(GetAssemblyID.strCoating2));
            }

            //Если покрытие 3 пусто, удаляем лишнюю колонку
            coat = 0;
            for (int z = 0; z < dt.Rows.Count; z++)
            {
                if (dt.Rows[z][dt.Columns.IndexOf(GetAssemblyID.strCoating3)].ToString() != "" && dt.Rows[z][dt.Columns.IndexOf(GetAssemblyID.strCoating3)].ToString().ToUpper() != "NONE")
                {
                    coat = 1;
                    break;
                }
            }

            if (coat == 0)
            {
                dt.Columns.RemoveAt(dt.Columns.IndexOf(GetAssemblyID.strCoating3));
            }

        }

    }
}
