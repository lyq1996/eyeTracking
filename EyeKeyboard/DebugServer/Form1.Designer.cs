namespace DebugServer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_up = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button_down = new System.Windows.Forms.Button();
            this.button_left = new System.Windows.Forms.Button();
            this.button_right = new System.Windows.Forms.Button();
            this.button_center = new System.Windows.Forms.Button();
            this.button_init = new System.Windows.Forms.Button();
            this.button_ensure = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.button_simg = new System.Windows.Forms.Button();
            this.button_bimg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_up
            // 
            this.button_up.Location = new System.Drawing.Point(98, 268);
            this.button_up.Name = "button_up";
            this.button_up.Size = new System.Drawing.Size(80, 37);
            this.button_up.TabIndex = 0;
            this.button_up.Text = "7.Up";
            this.button_up.UseVisualStyleBackColor = true;
            this.button_up.Click += new System.EventHandler(this.Button_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBox1.Size = new System.Drawing.Size(252, 207);
            this.textBox1.TabIndex = 1;
            // 
            // button_down
            // 
            this.button_down.Location = new System.Drawing.Point(98, 354);
            this.button_down.Name = "button_down";
            this.button_down.Size = new System.Drawing.Size(80, 37);
            this.button_down.TabIndex = 2;
            this.button_down.Text = "8.Down";
            this.button_down.UseVisualStyleBackColor = true;
            this.button_down.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_left
            // 
            this.button_left.Location = new System.Drawing.Point(12, 311);
            this.button_left.Name = "button_left";
            this.button_left.Size = new System.Drawing.Size(80, 37);
            this.button_left.TabIndex = 3;
            this.button_left.Text = "2.Left";
            this.button_left.UseVisualStyleBackColor = true;
            this.button_left.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_right
            // 
            this.button_right.Location = new System.Drawing.Point(184, 311);
            this.button_right.Name = "button_right";
            this.button_right.Size = new System.Drawing.Size(80, 37);
            this.button_right.TabIndex = 4;
            this.button_right.Text = "1.Right";
            this.button_right.UseVisualStyleBackColor = true;
            this.button_right.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_center
            // 
            this.button_center.Location = new System.Drawing.Point(98, 311);
            this.button_center.Name = "button_center";
            this.button_center.Size = new System.Drawing.Size(80, 37);
            this.button_center.TabIndex = 5;
            this.button_center.Text = "0.Center";
            this.button_center.UseVisualStyleBackColor = true;
            this.button_center.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_init
            // 
            this.button_init.Location = new System.Drawing.Point(12, 354);
            this.button_init.Name = "button_init";
            this.button_init.Size = new System.Drawing.Size(80, 37);
            this.button_init.TabIndex = 7;
            this.button_init.Text = "4.Init";
            this.button_init.UseVisualStyleBackColor = true;
            this.button_init.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_ensure
            // 
            this.button_ensure.Location = new System.Drawing.Point(184, 354);
            this.button_ensure.Name = "button_ensure";
            this.button_ensure.Size = new System.Drawing.Size(80, 37);
            this.button_ensure.TabIndex = 8;
            this.button_ensure.Text = "3.Ensure";
            this.button_ensure.UseVisualStyleBackColor = true;
            this.button_ensure.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(12, 225);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(252, 37);
            this.button_start.TabIndex = 9;
            this.button_start.Text = "Start";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_simg
            // 
            this.button_simg.Enabled = false;
            this.button_simg.Location = new System.Drawing.Point(12, 268);
            this.button_simg.Name = "button_simg";
            this.button_simg.Size = new System.Drawing.Size(80, 37);
            this.button_simg.TabIndex = 10;
            this.button_simg.Text = "5.simg";
            this.button_simg.UseVisualStyleBackColor = true;
            this.button_simg.Click += new System.EventHandler(this.Button_Click);
            // 
            // button_bimg
            // 
            this.button_bimg.Enabled = false;
            this.button_bimg.Location = new System.Drawing.Point(184, 268);
            this.button_bimg.Name = "button_bimg";
            this.button_bimg.Size = new System.Drawing.Size(80, 37);
            this.button_bimg.TabIndex = 11;
            this.button_bimg.Text = "6.bimg";
            this.button_bimg.UseVisualStyleBackColor = true;
            this.button_bimg.Click += new System.EventHandler(this.Button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 401);
            this.Controls.Add(this.button_bimg);
            this.Controls.Add(this.button_simg);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.button_ensure);
            this.Controls.Add(this.button_init);
            this.Controls.Add(this.button_center);
            this.Controls.Add(this.button_right);
            this.Controls.Add(this.button_left);
            this.Controls.Add(this.button_down);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button_up);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "DebugServer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_up;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button_down;
        private System.Windows.Forms.Button button_left;
        private System.Windows.Forms.Button button_right;
        private System.Windows.Forms.Button button_center;
        private System.Windows.Forms.Button button_init;
        private System.Windows.Forms.Button button_ensure;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_simg;
        private System.Windows.Forms.Button button_bimg;
    }
}

