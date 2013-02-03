namespace EnhancedLibrary.UIControls
{
    partial class WaitForm
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
            if ( disposing && ( components != null ) )
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
            this.m_progress = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.SuspendLayout();
            // 
            // m_progress
            // 
            this.m_progress.CausesValidation = true;
            this.m_progress.Location = new System.Drawing.Point(-1, -2);
            this.m_progress.Name = "m_progress";
            this.m_progress.Size = new System.Drawing.Size(456, 47);
            this.m_progress.TabIndex = 0;
            this.m_progress.TabStop = true;
            this.m_progress.ViewStyle = Infragistics.Win.UltraActivityIndicator.ActivityIndicatorViewStyle.Aero;
            // 
            // WaitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 45);
            this.Controls.Add(this.m_progress);
            this.Name = "WaitForm";
            this.Text = "Por favor aguarde...";
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator m_progress;
    }
}