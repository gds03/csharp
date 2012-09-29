namespace EnhancedLibrary.UI
{
    partial class SearchableDbComboUI
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
            this.searchableDbCombo1 = new EnhancedLibrary.UIControls.SearchableDbCombo();
            ( (System.ComponentModel.ISupportInitialize) ( this.searchableDbCombo1 ) ).BeginInit();
            this.SuspendLayout();
            // 
            // searchableDbCombo1
            // 
            this.searchableDbCombo1.ColumnName = null;
            this.searchableDbCombo1.ConnectionString = null;
            this.searchableDbCombo1.HasEnrollmentBehavior = true;
            this.searchableDbCombo1.LoadingText = "A carregar..";
            this.searchableDbCombo1.Location = new System.Drawing.Point(61, 26);
            this.searchableDbCombo1.Name = "searchableDbCombo1";
            this.searchableDbCombo1.Size = new System.Drawing.Size(144, 21);
            this.searchableDbCombo1.TabIndex = 0;
            this.searchableDbCombo1.TableName = null;
            // 
            // SearchableDbComboUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 80);
            this.Controls.Add(this.searchableDbCombo1);
            this.Name = "SearchableDbComboUI";
            this.Text = "Form1";
            ( (System.ComponentModel.ISupportInitialize) ( this.searchableDbCombo1 ) ).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UIControls.SearchableDbCombo searchableDbCombo1;
    }
}

