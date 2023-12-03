namespace LoginForm
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.BtnGoogle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(9, 95);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(226, 171);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(34, 13);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 20);
            this.labelStatus.TabIndex = 2;
            // 
            // BtnGoogle
            // 
            this.BtnGoogle.FlatAppearance.BorderColor = System.Drawing.SystemColors.Info;
            this.BtnGoogle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnGoogle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnGoogle.Image = global::LoginForm.Properties.Resources.icons8_google_48;
            this.BtnGoogle.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.BtnGoogle.Location = new System.Drawing.Point(12, 303);
            this.BtnGoogle.Name = "BtnGoogle";
            this.BtnGoogle.Size = new System.Drawing.Size(219, 54);
            this.BtnGoogle.TabIndex = 3;
            this.BtnGoogle.Text = "Sign up with google";
            this.BtnGoogle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnGoogle.UseVisualStyleBackColor = true;
            this.BtnGoogle.Click += new System.EventHandler(this.BtnGoogle_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(243, 434);
            this.Controls.Add(this.BtnGoogle);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button BtnGoogle;
    }
}

