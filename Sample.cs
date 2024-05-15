using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EPDM.Interop.epdm;

namespace FullBomHoum
{
   public class Sample
    {
        private void Convert_to_PDF(ADGV.AdvancedDataGridView DG)
        {
            List<string> listDrawingPath = new List<string>();
            try
            {
                foreach (DataGridViewRow i in DG.Rows)
                {
                    if (i.IsNewRow) continue;
                    DataGridViewCellCollection j = i.Cells;
                    if (j[GetAssemblyID.strDraw].Value.ToString() == "1")
                    {
                        listDrawingPath.Add(j[GetAssemblyID.strDraw].Value.ToString());
                    }

                }

            }

            catch
            {

                MessageBox.Show("No access to file");

            }

        }
    }
}
