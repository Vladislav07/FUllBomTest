using eDrawings.Interop.EModelViewControl;
namespace ExportToPDF
{
    partial class FormBom
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormBom
            // 
            this.ctrlEDrw = new EDrawingsUserControl();
            this.ctrlEDrw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.ctrlEDrw);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "FormBom";
            this.Text = "FormBom";
            this.ResumeLayout(false);
            this.ctrlEDrw.Name = "ctrlEDrw";
            this.ctrlEDrw.EDrawingsControlLoaded += new System.Action<EModelViewControl>(this.OnControlLoaded);

        }

        #endregion
        private EDrawingsUserControl ctrlEDrw;
    }
}

