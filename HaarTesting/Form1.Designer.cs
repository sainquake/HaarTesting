namespace HaarTesting
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
        	this.components = new System.ComponentModel.Container();
        	this.VideoWindow = new Emgu.CV.UI.ImageBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.button1 = new System.Windows.Forms.Button();
        	this.label2 = new System.Windows.Forms.Label();
        	this.timer1 = new System.Windows.Forms.Timer(this.components);
        	this.button2 = new System.Windows.Forms.Button();
        	this.button3 = new System.Windows.Forms.Button();
        	((System.ComponentModel.ISupportInitialize)(this.VideoWindow)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// VideoWindow
        	// 
        	this.VideoWindow.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
        	this.VideoWindow.Location = new System.Drawing.Point(12, 12);
        	this.VideoWindow.Name = "VideoWindow";
        	this.VideoWindow.Size = new System.Drawing.Size(640, 480);
        	this.VideoWindow.TabIndex = 2;
        	this.VideoWindow.TabStop = false;
        	this.VideoWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VideoWindow_MouseDown);
        	this.VideoWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VideoWindow_MouseUp);
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(55, 510);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(415, 78);
        	this.label1.TabIndex = 3;
        	this.label1.Text = "label1";
        	// 
        	// button1
        	// 
        	this.button1.Location = new System.Drawing.Point(558, 510);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 4;
        	this.button1.Text = "connect";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(55, 618);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(469, 37);
        	this.label2.TabIndex = 5;
        	this.label2.Text = "label2";
        	// 
        	// timer1
        	// 
        	this.timer1.Interval = 20;
        	this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
        	// 
        	// button2
        	// 
        	this.button2.Location = new System.Drawing.Point(558, 551);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(75, 23);
        	this.button2.TabIndex = 6;
        	this.button2.Text = "set null";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.Button2Click);
        	// 
        	// button3
        	// 
        	this.button3.Location = new System.Drawing.Point(558, 593);
        	this.button3.Name = "button3";
        	this.button3.Size = new System.Drawing.Size(75, 23);
        	this.button3.TabIndex = 7;
        	this.button3.Text = "draw";
        	this.button3.UseVisualStyleBackColor = true;
        	this.button3.Click += new System.EventHandler(this.Button3Click);
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(658, 664);
        	this.Controls.Add(this.button3);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.button1);
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.VideoWindow);
        	this.Name = "Form1";
        	this.Text = "Haar finding";
        	this.Load += new System.EventHandler(this.Form1_Load);
        	((System.ComponentModel.ISupportInitialize)(this.VideoWindow)).EndInit();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;

        #endregion

        private Emgu.CV.UI.ImageBox VideoWindow;
    }
}

