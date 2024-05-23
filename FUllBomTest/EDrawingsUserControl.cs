
using System;
using System.Windows.Forms;
using eDrawings.Interop.EModelViewControl;

namespace FullBomHoum
{
    public partial class EDrawingsUserControl : UserControl
    {
        public event Action<EModelViewControl> EDrawingsControlLoaded;

        public EDrawingsUserControl()
        {
            InitializeComponent();
        }

        public void LoadEDrawings()
        {
            try
            {
                var host = new EDrawingsHost();
                host.ControlLoaded += OnControlLoaded;
                this.Controls.Add(host);
                host.Dock = DockStyle.Fill;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message + " --loadDrawings");
            }
           
        }
        
        private void OnControlLoaded(EModelViewControl ctrl)
        {
            EDrawingsControlLoaded?.Invoke(ctrl);
        }
    }
}
