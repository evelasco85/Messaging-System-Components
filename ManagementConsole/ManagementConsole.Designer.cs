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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCreditBureauStatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLoanBrokerStatData = new System.Windows.Forms.TextBox();
            this.txtLoanBrokerResponseData = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCreditBureauStatus);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtLoanBrokerStatData);
            this.groupBox1.Controls.Add(this.txtLoanBrokerResponseData);
            this.groupBox1.Location = new System.Drawing.Point(445, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(621, 617);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Running Data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(88, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Monitor Credit Bureau:";
            // 
            // txtCreditBureauStatus
            // 
            this.txtCreditBureauStatus.Location = new System.Drawing.Point(88, 46);
            this.txtCreditBureauStatus.Multiline = true;
            this.txtCreditBureauStatus.Name = "txtCreditBureauStatus";
            this.txtCreditBureauStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCreditBureauStatus.Size = new System.Drawing.Size(473, 146);
            this.txtCreditBureauStatus.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(88, 415);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Load Broker Statistic Data:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(88, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Load Broker Response Data:";
            // 
            // txtLoanBrokerStatData
            // 
            this.txtLoanBrokerStatData.Location = new System.Drawing.Point(88, 431);
            this.txtLoanBrokerStatData.Multiline = true;
            this.txtLoanBrokerStatData.Name = "txtLoanBrokerStatData";
            this.txtLoanBrokerStatData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLoanBrokerStatData.Size = new System.Drawing.Size(473, 168);
            this.txtLoanBrokerStatData.TabIndex = 7;
            // 
            // txtLoanBrokerResponseData
            // 
            this.txtLoanBrokerResponseData.Location = new System.Drawing.Point(88, 230);
            this.txtLoanBrokerResponseData.Multiline = true;
            this.txtLoanBrokerResponseData.Name = "txtLoanBrokerResponseData";
            this.txtLoanBrokerResponseData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLoanBrokerResponseData.Size = new System.Drawing.Size(473, 146);
            this.txtLoanBrokerResponseData.TabIndex = 6;
            // 
            // ManagementConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 701);
            this.Controls.Add(this.groupBox1);
            this.Name = "ManagementConsole";
            this.Text = "Management Console";
            this.Load += new System.EventHandler(this.ManagementConsole_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCreditBureauStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLoanBrokerStatData;
        private System.Windows.Forms.TextBox txtLoanBrokerResponseData;
    }
}

