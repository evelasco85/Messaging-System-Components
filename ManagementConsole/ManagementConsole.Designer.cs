namespace ManagementConsole
{
    partial class ManagementConsole
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
            this.txtControlBusData = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtControlBusData
            // 
            this.txtControlBusData.Location = new System.Drawing.Point(12, 205);
            this.txtControlBusData.Multiline = true;
            this.txtControlBusData.Name = "txtControlBusData";
            this.txtControlBusData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtControlBusData.Size = new System.Drawing.Size(473, 146);
            this.txtControlBusData.TabIndex = 0;
            // 
            // ManagementConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 595);
            this.Controls.Add(this.txtControlBusData);
            this.Name = "ManagementConsole";
            this.Text = "Management Console";
            this.Load += new System.EventHandler(this.ManagementConsole_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtControlBusData;
    }
}

