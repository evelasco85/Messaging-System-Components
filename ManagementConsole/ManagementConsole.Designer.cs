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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnActivateAll = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtServerResponse = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(506, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(520, 617);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Running Data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Monitor Credit Bureau:";
            // 
            // txtCreditBureauStatus
            // 
            this.txtCreditBureauStatus.Location = new System.Drawing.Point(19, 43);
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
            this.label2.Location = new System.Drawing.Point(19, 412);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Load Broker Statistic Data:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Load Broker Response Data:";
            // 
            // txtLoanBrokerStatData
            // 
            this.txtLoanBrokerStatData.Location = new System.Drawing.Point(19, 428);
            this.txtLoanBrokerStatData.Multiline = true;
            this.txtLoanBrokerStatData.Name = "txtLoanBrokerStatData";
            this.txtLoanBrokerStatData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLoanBrokerStatData.Size = new System.Drawing.Size(473, 168);
            this.txtLoanBrokerStatData.TabIndex = 7;
            // 
            // txtLoanBrokerResponseData
            // 
            this.txtLoanBrokerResponseData.Location = new System.Drawing.Point(19, 227);
            this.txtLoanBrokerResponseData.Multiline = true;
            this.txtLoanBrokerResponseData.Name = "txtLoanBrokerResponseData";
            this.txtLoanBrokerResponseData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLoanBrokerResponseData.Size = new System.Drawing.Size(473, 146);
            this.txtLoanBrokerResponseData.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnActivateAll);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtServerResponse);
            this.groupBox2.Location = new System.Drawing.Point(12, 35);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(468, 617);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Orchestration";
            // 
            // btnActivateAll
            // 
            this.btnActivateAll.Location = new System.Drawing.Point(6, 261);
            this.btnActivateAll.Name = "btnActivateAll";
            this.btnActivateAll.Size = new System.Drawing.Size(115, 23);
            this.btnActivateAll.TabIndex = 14;
            this.btnActivateAll.Text = "Activate Clients";
            this.btnActivateAll.UseVisualStyleBackColor = true;
            this.btnActivateAll.Click += new System.EventHandler(this.btnActivateAll_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Server Response:";
            // 
            // txtServerResponse
            // 
            this.txtServerResponse.Location = new System.Drawing.Point(6, 43);
            this.txtServerResponse.Multiline = true;
            this.txtServerResponse.Name = "txtServerResponse";
            this.txtServerResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtServerResponse.Size = new System.Drawing.Size(456, 200);
            this.txtServerResponse.TabIndex = 12;
            // 
            // ManagementConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 701);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ManagementConsole";
            this.Text = "Management Console";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtServerResponse;
        private System.Windows.Forms.Button btnActivateAll;
    }
}

