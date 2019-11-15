namespace testdlibdotnetNuget
{
    partial class CalculateDistance
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_enroll = new System.Windows.Forms.Button();
            this.btn_calDistance = new System.Windows.Forms.Button();
            this.pic_image2 = new System.Windows.Forms.PictureBox();
            this.pic_image1 = new System.Windows.Forms.PictureBox();
            this.lbl_status = new System.Windows.Forms.Label();
            this.lbl_picture1 = new System.Windows.Forms.Label();
            this.lbl_picture2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_image2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_image1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbl_picture2);
            this.panel1.Controls.Add(this.lbl_picture1);
            this.panel1.Controls.Add(this.lbl_status);
            this.panel1.Controls.Add(this.btn_enroll);
            this.panel1.Controls.Add(this.btn_calDistance);
            this.panel1.Controls.Add(this.pic_image2);
            this.panel1.Controls.Add(this.pic_image1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(719, 341);
            this.panel1.TabIndex = 0;
            // 
            // btn_enroll
            // 
            this.btn_enroll.Location = new System.Drawing.Point(511, 292);
            this.btn_enroll.Name = "btn_enroll";
            this.btn_enroll.Size = new System.Drawing.Size(75, 34);
            this.btn_enroll.TabIndex = 3;
            this.btn_enroll.Text = "Enrollment";
            this.btn_enroll.UseVisualStyleBackColor = true;
            this.btn_enroll.Visible = false;
            this.btn_enroll.Click += new System.EventHandler(this.btn_enroll_Click);
            // 
            // btn_calDistance
            // 
            this.btn_calDistance.Location = new System.Drawing.Point(615, 292);
            this.btn_calDistance.Name = "btn_calDistance";
            this.btn_calDistance.Size = new System.Drawing.Size(87, 34);
            this.btn_calDistance.TabIndex = 2;
            this.btn_calDistance.Text = "Calculate Distance";
            this.btn_calDistance.UseVisualStyleBackColor = true;
            this.btn_calDistance.Click += new System.EventHandler(this.btn_calDistance_Click);
            // 
            // pic_image2
            // 
            this.pic_image2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_image2.Location = new System.Drawing.Point(371, 3);
            this.pic_image2.Name = "pic_image2";
            this.pic_image2.Size = new System.Drawing.Size(344, 256);
            this.pic_image2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_image2.TabIndex = 1;
            this.pic_image2.TabStop = false;
            this.pic_image2.Click += new System.EventHandler(this.pic_image2_Click);
            // 
            // pic_image1
            // 
            this.pic_image1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_image1.Location = new System.Drawing.Point(3, 3);
            this.pic_image1.Name = "pic_image1";
            this.pic_image1.Size = new System.Drawing.Size(344, 256);
            this.pic_image1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_image1.TabIndex = 0;
            this.pic_image1.TabStop = false;
            this.pic_image1.Click += new System.EventHandler(this.pic_image1_Click);
            // 
            // lbl_status
            // 
            this.lbl_status.AutoSize = true;
            this.lbl_status.Location = new System.Drawing.Point(13, 303);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(43, 13);
            this.lbl_status.TabIndex = 4;
            this.lbl_status.Text = "Status: ";
            // 
            // lbl_picture1
            // 
            this.lbl_picture1.AutoSize = true;
            this.lbl_picture1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_picture1.Location = new System.Drawing.Point(147, 266);
            this.lbl_picture1.Name = "lbl_picture1";
            this.lbl_picture1.Size = new System.Drawing.Size(58, 13);
            this.lbl_picture1.TabIndex = 5;
            this.lbl_picture1.Text = "Picture 1";
            // 
            // lbl_picture2
            // 
            this.lbl_picture2.AutoSize = true;
            this.lbl_picture2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_picture2.Location = new System.Drawing.Point(508, 266);
            this.lbl_picture2.Name = "lbl_picture2";
            this.lbl_picture2.Size = new System.Drawing.Size(58, 13);
            this.lbl_picture2.TabIndex = 6;
            this.lbl_picture2.Text = "Picture 2";
            // 
            // CalculateDistance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 365);
            this.Controls.Add(this.panel1);
            this.Name = "CalculateDistance";
            this.Text = "CalculateDistance";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_image2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_image1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_calDistance;
        private System.Windows.Forms.PictureBox pic_image2;
        private System.Windows.Forms.PictureBox pic_image1;
        private System.Windows.Forms.Button btn_enroll;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Label lbl_picture1;
        private System.Windows.Forms.Label lbl_picture2;
    }
}