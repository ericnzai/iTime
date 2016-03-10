namespace iTimeService.Common
{
    partial class frmAxBioBridge
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAxBioBridge));
            this.axBioBridgeSDK1 = new AxBioBridgeSDKLib.AxBioBridgeSDK();
            ((System.ComponentModel.ISupportInitialize)(this.axBioBridgeSDK1)).BeginInit();
            this.SuspendLayout();
            // 
            // axBioBridgeSDK1
            // 
            this.axBioBridgeSDK1.Enabled = true;
            this.axBioBridgeSDK1.Location = new System.Drawing.Point(116, 95);
            this.axBioBridgeSDK1.Name = "axBioBridgeSDK1";
            this.axBioBridgeSDK1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axBioBridgeSDK1.OcxState")));
            this.axBioBridgeSDK1.Size = new System.Drawing.Size(173, 50);
            this.axBioBridgeSDK1.TabIndex = 0;
            // 
            // frmAxBioBridge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.axBioBridgeSDK1);
            this.Name = "frmAxBioBridge";
            this.Text = "frmAxBioBridge";
            ((System.ComponentModel.ISupportInitialize)(this.axBioBridgeSDK1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxBioBridgeSDKLib.AxBioBridgeSDK axBioBridgeSDK1;
    }
}