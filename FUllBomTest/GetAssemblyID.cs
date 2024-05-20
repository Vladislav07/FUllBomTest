using System;
using EdmLib;
using System.Windows.Forms;
using System.Collections;




namespace FullBomHoum
{
    public class GetAssemblyID
    {
        public static int ASMID;
        public static int ASMFolderID;
        public static string name0;
        //public static string strFoundInVer;   //= "Found In Version";
        public static string pdmName;           //= "test";
        public static string strFullBOM;        //= "FullBOM";//Название Bills of Materials
        public static string strFileName;       //= "File_Name";
        public static string strConfig;         //= "Конфигурация";
        public static string strLatestVer;      //= "Latest Version";
        public static string strFoundIn;        //= "Found In";        
        public static string strDXF;            //= "DXF";
        public static string strDraw;           //= "Drawing";
        public static string strDrawState;      //= "Drawing State";
        public static string strQTY;            //= "QTY";
        public static string strTQTY;           //= "TQTY";
        public static string strTotalQTY;       //= "Total QTY";
        public static string strSUMQTY;         //= "TOTAL QTY";//Суммарное количество
        public static string strWhereUsed;      //= "Where Used";
        public static string strPartNumber;     //= "Обозначение" CUBY_PN;
        public static string strSection;        //= "Раздел";
        public static string strLaserCut;       //= "Лазерная резка";
        public static string strDescription_RUS;//= "Наименование";
        public static string strConfigPoint;    //= ".";
        public static string strTypeEx;         //= "Tahoma";//Шрифт Excel
        public static string strTypeExSize;     //= "10";//Шрифт Excel
        public static string strColErr;         // Цвет ошибки
        public static string strErC;            // Столбец ошибок
        public static string strState;          // State
        public static string strWeight;         // Масса
        public static string strCoating;        // Покрытие
        public static string strCoating2;       // Покрытие2
        public static string strCoating3;       // Покрытие3
        public static string strCoatApp;        // Нанесение покрытия
        public static string strComment;        // Примечание
        public static string strFileID;         // File ID
        public static string strDescription_ENG;//= "Description";
        public static string strInitiated;      // Initiated
        public static string strInWork;         // In work
        public static string strLockOp;         //Locksmith Operations
        public static string strTurning;        //Turning
        public static string strMetalBend;      //Metal Bending
        public static string strCasting;        //Casting
        public static string strMilling;        //Milling
        public static string strVacForm;        //Vacuum Forming
        public static string strWelding;        //Welding
        public static string strSticker;        //Sticker
        public static string str3DPrint;        //3D printing
        public static string strReservePart1;   //Резерв 1 для детали
        public static string strReservePart2;   //Резерв 2 для сборки
        public static string strGlue;           //Glue
        public static string strSoldering;      //Soldering
        public static string strAssembly;       //Assembly
        public static string strReserveAss1;    //Резерв 1 для сборки
        public static string strReserveAss2;    //Резерв 2 для сборки
        public static string strRev;            //Ревизия
        public static string strManufac;        //=Manufacturer
        public static string strManufacNumb;    //=Manufacturer_number
        public static string strShape;          //Shape
        public static string strSortament;      //Sortament
        public static string strMaterial;       //материал
        public static string strSurfaceArea;    //Площадь
        public static string strThickness;      //Толщина
        public static string strAnnotation;     //Аннотация
        public static string strPrelim;         //= Preliminary design;
        public static string strPCB;            //= Печатная плата;
        public static string str3DCuting;       //= 3D cutting
        public static string strIgs;            //= IGS files
        public static string strNoSHEETS;       //= NoSHEETS;
        public static string strLengthPipe;     //= Длина трубы при лазерной резке;

        public static DataGridViewCellStyle cellStyleErr = new DataGridViewCellStyle();
        public static System.Drawing.Color colorError = new System.Drawing.Color();

   


        public void OnCmd(string pathAss)
        {
            try
            {
                {
                    #region setVar
                    //Заполнение названий свойств из файлаC:\Users\belov\source\FB
                    System.IO.StreamReader objReader = new System.IO.StreamReader(@"C:\Users\v.belov\source\LocalFullBom\FullBOM_2.0.8.cfg"); //("C:\\Users\\belov\\source\\FB\\FullBOM_2.0.2.cfg");
                    string sLine = "";
                    ArrayList arrText = new ArrayList();
                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (sLine != null)
                            arrText.Add(sLine);
                    }
                    objReader.Close();
                    strTQTY = "TQTY";                           //= "TQTY";

                    pdmName = arrText[0].ToString();            //PDM name
                    strFullBOM = arrText[1].ToString();         //= "FullBOM";//Название Bills of Materials // New specification FullBOM_CUBY
                    strConfigPoint = arrText[2].ToString();     //= ".";
                    strTypeEx = arrText[3].ToString();          //= "Tahoma";//Шрифт Excel
                    strTypeExSize = arrText[4].ToString();      //= "10";//Шрифт Excel

                    strColErr = arrText[5].ToString();          // Цвет ошибки
                    strErC = arrText[6].ToString();             //=Столбец ошибок Er;
                    strFileID = arrText[7].ToString();          // ID файла
                    strState = arrText[8].ToString();           //=Состояние;
                    strDraw = arrText[9].ToString();            //= "Drawing";

                    strDrawState = arrText[10].ToString();         //= "Drawing State";
                    strDXF = arrText[11].ToString();            //= "DXF";
                    strSection = arrText[12].ToString();        //= "Раздел";
                    strRev = arrText[13].ToString();            //= "Ревизия";
                    strLatestVer = arrText[14].ToString();      //= "Latest Version";

                    strPartNumber = arrText[15].ToString();     //= "Обозначение";
                    strDescription_RUS = arrText[16].ToString();     //= "Наименование";
                    strDescription_ENG = arrText[17].ToString();     //= "Description";
                    strWhereUsed = arrText[18].ToString();      //= "Where Used";
                    strQTY = arrText[19].ToString();            //= "QTY";

                    strTotalQTY = arrText[20].ToString();       //= "Total QTY";
                    strSUMQTY = arrText[21].ToString();         //= "TOTAL QTY";//Суммарное количество
                    strComment = arrText[22].ToString();        //=Примечание/Remark;
                    strManufac = arrText[23].ToString();        //=Manufacturer
                    strManufacNumb = arrText[24].ToString();    //=Manufacturer_number

                    strConfig = arrText[25].ToString();         //= "Конфигурация";
                    strFoundIn = arrText[26].ToString();        //= "Found In";
                    strFileName = arrText[27].ToString();       //= "File_Name";
                    strShape = arrText[28].ToString();          //Shape
                    strSortament = arrText[29].ToString();      //sortament

                    strMaterial = arrText[30].ToString();       //материал
                    strWeight = arrText[31].ToString();         //=Масса;
                    strSurfaceArea = arrText[32].ToString();    //Площадь
                    strThickness = arrText[33].ToString();      //Толщина
                    strCoatApp = arrText[34].ToString();        //=Нанесение покрытия;

                    strCoating = arrText[35].ToString();        //=Покрытие;
                    strCoating2 = arrText[36].ToString();       //=Покрытие2;
                    strCoating3 = arrText[37].ToString();       //=Покрытие3;
                    strLaserCut = arrText[38].ToString();       //= "Лазерная резка";
                    strMetalBend = arrText[39].ToString();      //Metal Bending

                    strCasting = arrText[40].ToString();        //Casting
                    str3DPrint = arrText[41].ToString();        //3D printing                    
                    strVacForm = arrText[42].ToString();        //Vacuum Forming
                    strSticker = arrText[43].ToString();        //Sticker
                    strPCB = arrText[44].ToString();            // PCB

                    strReservePart1 = arrText[45].ToString();   //Резерв 1 для детали
                    strReservePart2 = arrText[46].ToString();   //Резерв 2 для детали                   
                    strLockOp = arrText[47].ToString();         //Locksmith Operations
                    strWelding = arrText[48].ToString();        //Welding
                    strTurning = arrText[49].ToString();        //Turning

                    strMilling = arrText[50].ToString();        //Milling
                    strAssembly = arrText[51].ToString();       //Assembly 
                    strSoldering = arrText[52].ToString();      //Soldering
                    strGlue = arrText[53].ToString();           //Glue
                    strReserveAss1 = arrText[54].ToString();    //Резерв 1 для сборки

                    strReserveAss2 = arrText[55].ToString();    //Резерв 2 для сборки
                    strAnnotation = arrText[56].ToString();     //Аннотация
                    strInitiated = arrText[57].ToString();      //= "Initiated";
                    strInWork = arrText[58].ToString();         //= "In work;
                    strPrelim = arrText[59].ToString();         //= "Preliminary design";


                     str3DCuting = arrText[60].ToString();       //= 3D cuttings;
                     strIgs = arrText[61].ToString();            //= isIgs;
                    strNoSHEETS = arrText[62].ToString();       //= NoSHEETS;
                    strLengthPipe = arrText[63].ToString();     //= Длина трубы при лазерной резке;

                    colorError = System.Drawing.Color.FromName(strColErr); //Цвет ошибки
                    cellStyleErr.BackColor = colorError; //Стиль ячеек содержащих ошибки

                    #endregion
                    string FileName = pathAss;
                        //@"C:\CUBY_PDM\Work\Other\Без проекта\CUBY-V1.1\CAD\Завод контейнер\16 - Участок обработки листового металла и трубы\Концепт участка обработки листового металла и трубы\Кран балка листа\CUBY-00153564.sldasm"; //"A:\\My\\10-тонный электрический подъемный кран\\ramka-1.sldasm";
                    string e = System.IO.Path.GetExtension(FileName);
                    name0 = System.IO.Path.GetFileNameWithoutExtension(FileName);

                    IEdmFile5 file5 = null;
                    IEdmFolder5 folder5 = null;
                    EdmVault5 v = new EdmVault5();
                    if (!v.IsLoggedIn)
                    {
                        v.LoginAuto("CUBY_PDM", 0);
                       // MessageBox.Show("Ok!!!");
                    }

                    file5 = v.GetFileFromPath(FileName, out folder5);

                    if ((e == ".sldasm") || (e == ".SLDASM"))   //replace slddrw
                    {


                        ASMID = file5.ID;
                        ASMFolderID = folder5.ID;

                        Application.Run(new Form1());
                    }
                    else
                    {

                        MessageBox.Show("Select the assembly model file (.SLDASM)");
                        Application.Exit();
                    }
                }

            }

            catch (System.Runtime.InteropServices.COMException ex)
            { MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message); }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }

        }
    }
}
