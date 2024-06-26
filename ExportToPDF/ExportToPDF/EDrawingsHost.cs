﻿

using eDrawings.Interop.EModelViewControl;
using System;
using System.Windows.Forms;

namespace ExportToPDF
{
    public class EDrawingsHost : AxHost
    {
        public event Action<EModelViewControl> ControlLoaded;

        private bool m_IsLoaded;

        public EDrawingsHost() : base("22945A69-1191-4DCF-9E6F-409BDE94D101")
        {
            m_IsLoaded = false;
        }

        protected override void OnCreateControl()
        {
            try
            {
                base.OnCreateControl();

                if (!m_IsLoaded)
                {
                    m_IsLoaded = true;
                    var ctrl = this.GetOcx() as EModelViewControl;
                    ControlLoaded?.Invoke(this.GetOcx() as EModelViewControl);
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message + " -- Edrawings host");
            }
           
        }
    }
}
