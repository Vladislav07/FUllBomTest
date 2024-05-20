using System.Windows.Forms;

namespace FullBomHoum
{
    partial class Form1
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.advancedDataGridView1 = new ADGV.AdvancedDataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.VP = new System.Windows.Forms.Button();
            this.VD = new System.Windows.Forms.Button();
            this.Error_Filter = new System.Windows.Forms.Button();
            this.PM = new System.Windows.Forms.Button();
            this.BOM = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.toPDF = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // advancedDataGridView1
            // 
            this.advancedDataGridView1.AllowUserToAddRows = false;
            this.advancedDataGridView1.AllowUserToDeleteRows = false;
            this.advancedDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedDataGridView1.AutoGenerateColumns = false;
            this.advancedDataGridView1.AutoGenerateContextFilters = true;
            this.advancedDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.advancedDataGridView1.DataSource = this.bindingSource1;
            this.advancedDataGridView1.DateWithTime = false;
            this.advancedDataGridView1.Location = new System.Drawing.Point(-12, 59);
            this.advancedDataGridView1.Name = "advancedDataGridView1";
            this.advancedDataGridView1.RowHeadersWidth = 50;
            this.advancedDataGridView1.Size = new System.Drawing.Size(1456, 539);
            this.advancedDataGridView1.TabIndex = 10;
            this.advancedDataGridView1.TimeFilter = false;
            this.advancedDataGridView1.SortStringChanged += new System.EventHandler(this.AdvancedDataGridView1_SortStringChanged);
            this.advancedDataGridView1.FilterStringChanged += new System.EventHandler(this.AdvancedDataGridView1_FilterStringChanged);
            this.advancedDataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AdvancedDataGridView1_CellContentDoubleClick);
            this.advancedDataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.AdvancedDataGridView1_CellEndEdit);
            this.advancedDataGridView1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.AdvancedDataGridView1_RowsRemoved);
            // 
            // bindingSource1
            // 
            this.bindingSource1.AllowNew = true;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(499, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 23);
            this.label1.TabIndex = 7;
            this.label1.Text = "Version:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(155, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "Configuration:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(12, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 23);
            this.label3.TabIndex = 17;
            this.label3.Text = "BOM: ";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(1324, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "v 3.13";
            // 
            // button1
            // 
            this.button1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.button1.Location = new System.Drawing.Point(657, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 32);
            this.button1.TabIndex = 19;
            this.button1.Text = "Export to Excel";
            this.button1.Click += new System.EventHandler(this.Export_To_Excel_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox1.ItemHeight = 16;
            this.comboBox1.Location = new System.Drawing.Point(566, 16);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(63, 24);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            this.comboBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ComboBox1_MouseWheel);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox2.Enabled = false;
            this.comboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox2.ItemHeight = 16;
            this.comboBox2.Location = new System.Drawing.Point(267, 16);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(70, 24);
            this.comboBox2.TabIndex = 8;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBox2_SelectedIndexChanged);
            this.comboBox2.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ComboBox2_MouseWheel);
            // 
            // comboBox3
            // 
            this.comboBox3.Enabled = false;
            this.comboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.ItemHeight = 16;
            this.comboBox3.Location = new System.Drawing.Point(57, 16);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(92, 24);
            this.comboBox3.TabIndex = 18;
            this.comboBox3.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ComboBox3_MouseWheel);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(357, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 17);
            this.label5.TabIndex = 21;
            this.label5.Text = "Revision: ";
            // 
            // comboBox4
            // 
            this.comboBox4.Enabled = false;
            this.comboBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.ItemHeight = 16;
            this.comboBox4.Location = new System.Drawing.Point(431, 16);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(62, 24);
            this.comboBox4.TabIndex = 22;
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.ComboBox4_SelectedIndexChanged);
            this.comboBox4.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ComboBox4_MouseWheel);
            // 
            // button2
            // 
            this.button2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.button2.Location = new System.Drawing.Point(1023, 11);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 31);
            this.button2.TabIndex = 26;
            this.button2.Text = "Refresh";
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1336, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "label6";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1304, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "label7";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.SizeChanged += new System.EventHandler(this.Label7_SizeChanged);
            // 
            // button3
            // 
            this.button3.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.button3.Location = new System.Drawing.Point(903, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(114, 31);
            this.button3.TabIndex = 28;
            this.button3.Text = "Reset all filters";
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // VP
            // 
            this.VP.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.VP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.VP.Location = new System.Drawing.Point(1102, 13);
            this.VP.Name = "VP";
            this.VP.Size = new System.Drawing.Size(37, 31);
            this.VP.TabIndex = 29;
            this.VP.Text = "VP";
            this.VP.Click += new System.EventHandler(this.VP_Click);
            // 
            // VD
            // 
            this.VD.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.VD.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.VD.Location = new System.Drawing.Point(1243, 13);
            this.VD.Name = "VD";
            this.VD.Size = new System.Drawing.Size(37, 31);
            this.VD.TabIndex = 30;
            this.VD.Text = "VD";
            this.VD.Click += new System.EventHandler(this.VD_Click);
            // 
            // Error_Filter
            // 
            this.Error_Filter.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Error_Filter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Error_Filter.Location = new System.Drawing.Point(1377, 13);
            this.Error_Filter.Name = "Error_Filter";
            this.Error_Filter.Size = new System.Drawing.Size(51, 31);
            this.Error_Filter.TabIndex = 31;
            this.Error_Filter.Text = "Error";
            this.Error_Filter.Click += new System.EventHandler(this.Error_Filter_Click);
            // 
            // PM
            // 
            this.PM.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.PM.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.PM.Location = new System.Drawing.Point(1145, 12);
            this.PM.Name = "PM";
            this.PM.Size = new System.Drawing.Size(37, 31);
            this.PM.TabIndex = 32;
            this.PM.Text = "PM";
            this.PM.Click += new System.EventHandler(this.PM_Click);
            // 
            // BOM
            // 
            this.BOM.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BOM.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.BOM.Location = new System.Drawing.Point(1188, 13);
            this.BOM.Name = "BOM";
            this.BOM.Size = new System.Drawing.Size(49, 31);
            this.BOM.TabIndex = 33;
            this.BOM.Text = "BOM";
            this.BOM.Click += new System.EventHandler(this.TQ_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1304, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "label8";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.SizeChanged += new System.EventHandler(this.Label8_SizeChanged);
            // 
            // toPDF
            // 
            this.toPDF.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.toPDF.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.toPDF.Location = new System.Drawing.Point(774, 13);
            this.toPDF.Name = "toPDF";
            this.toPDF.Size = new System.Drawing.Size(106, 32);
            this.toPDF.TabIndex = 35;
            this.toPDF.Text = "Export to PDF";
            this.toPDF.Click += new System.EventHandler(this.toPDF_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1456, 657);
            this.Controls.Add(this.toPDF);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.BOM);
            this.Controls.Add(this.PM);
            this.Controls.Add(this.Error_Filter);
            this.Controls.Add(this.VD);
            this.Controls.Add(this.VP);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.advancedDataGridView1);
            this.Name = "Form1";
            this.Text = " ";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private void createToolTip(Control controlForToolTip, string toolTipText)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.Active = true;
            toolTip.SetToolTip(controlForToolTip, toolTipText);
            toolTip.IsBalloon = true;
        }

        private void ComboBox1_SelectionChangeCommitted1(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion



        private ADGV.AdvancedDataGridView advancedDataGridView1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private Label label5;
        private ComboBox comboBox4;
        private Button button2;
        private Label label6;
        private Label label7;
        private Button button3;
        private Button VP;
        private Button VD;
        private Button Error_Filter;
        private Button PM;
        private Button BOM;
        private Label label8;
        private Button toPDF;
    }
}