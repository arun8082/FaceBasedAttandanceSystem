namespace testdlibdotnetNuget
{
    partial class DistanceCalculation
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
            this.pnl_1 = new System.Windows.Forms.Panel();
            this.btn_calDistance = new System.Windows.Forms.Button();
            this.pnl_1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_1
            // 
            this.pnl_1.Controls.Add(this.btn_calDistance);
            this.pnl_1.Location = new System.Drawing.Point(12, 12);
            this.pnl_1.Name = "pnl_1";
            this.pnl_1.Size = new System.Drawing.Size(695, 359);
            this.pnl_1.TabIndex = 0;
            // 
            // btn_calDistance
            // 
            this.btn_calDistance.Location = new System.Drawing.Point(13, 323);
            this.btn_calDistance.Name = "btn_calDistance";
            this.btn_calDistance.Size = new System.Drawing.Size(75, 23);
            this.btn_calDistance.TabIndex = 0;
            this.btn_calDistance.Text = "button1";
            this.btn_calDistance.UseVisualStyleBackColor = true;
            // 
            // DistanceCalculation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 383);
            this.Controls.Add(this.pnl_1);
            this.Name = "DistanceCalculation";
            this.Text = "DistanceCalculation";
            this.pnl_1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_1;
        private System.Windows.Forms.Button btn_calDistance;
    }
}