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
            this.txtLoanBrokerResponseData = new System.Windows.Forms.TextBox();
            this.txtLoanBrokerStatData = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtLoanBrokerResponseData
            // 
            this.txtLoanBrokerResponseData.Location = new System.Drawing.Point(12, 170);
            this.txtLoanBrokerResponseData.Multiline = true;
            this.txtLoanBrokerResponseData.Name = "txtLoanBrokerResponseData";
            this.txtLoanBrokerResponseData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLoanBrokerResponseData.Size = new System.Drawing.Size(473, 146);
            this.txtLoanBrokerResponseData.TabIndex = 0;
            // 
            // txtLoanBrokerStatData
            // 
            this.txtLoanBrokerStatData.Location = new System.Drawing.Point(12, 371);
            this.txtLoanBrokerStatData.Multiline = true;
            this.txtLoanBrokerStatData.Name = "txtLoanBrokerStatData";
            this.txtLoanBrokerStatData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLoanBrokerStatData.Size = new System.Drawing.Size(473, 168);
            this.txtLoanBrokerStatData.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Load Broker Response Data:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 355);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Load Broker Statistic Data:";
            // 
            // ManagementConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 595);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLoanBrokerStatData);
            this.Controls.Add(this.txtLoanBrokerResponseData);
            this.Name = "ManagementConsole";
            this.Text = "Management Console";
            this.Load += new System.EventHandler(this.ManagementConsole_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLoanBrokerResponseData;
        private System.Windows.Forms.TextBox txtLoanBrokerStatData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

