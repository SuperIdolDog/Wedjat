namespace Wedjat.WinForm
{
    partial class FormLogin
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
            AntdUI.CarouselItem carouselItem1 = new AntdUI.CarouselItem();
            AntdUI.CarouselItem carouselItem2 = new AntdUI.CarouselItem();
            this.pageHeader_Login = new AntdUI.PageHeader();
            this.panel1 = new AntdUI.Panel();
            this.label_bottom = new AntdUI.Label();
            this.panel2 = new AntdUI.Panel();
            this.carousel1 = new AntdUI.Carousel();
            this.panel3 = new AntdUI.Panel();
            this.select_productLine = new AntdUI.Select();
            this.label_line = new AntdUI.Label();
            this.label_pwd = new AntdUI.Label();
            this.input_Password = new AntdUI.Input();
            this.btn_Login = new AntdUI.Button();
            this.label_workid = new AntdUI.Label();
            this.input_WorkId = new AntdUI.Input();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageHeader_Login
            // 
            this.pageHeader_Login.BackColor = System.Drawing.Color.White;
            this.pageHeader_Login.DividerShow = true;
            this.pageHeader_Login.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader_Login.Font = new System.Drawing.Font("Siemens Slab SC", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.pageHeader_Login.ForeColor = System.Drawing.SystemColors.Control;
            this.pageHeader_Login.Location = new System.Drawing.Point(0, 0);
            this.pageHeader_Login.MaximizeBox = false;
            this.pageHeader_Login.Name = "pageHeader_Login";
            this.pageHeader_Login.ShowButton = true;
            this.pageHeader_Login.Size = new System.Drawing.Size(800, 45);
            this.pageHeader_Login.TabIndex = 0;
            this.pageHeader_Login.Text = "Wedjat";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderWidth = 1F;
            this.panel1.Controls.Add(this.label_bottom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 455);
            this.panel1.Name = "panel1";
            this.panel1.Radius = 15;
            this.panel1.RadiusAlign = AntdUI.TAlignRound.TR;
            this.panel1.Size = new System.Drawing.Size(800, 45);
            this.panel1.TabIndex = 20;
            // 
            // label_bottom
            // 
            this.label_bottom.BackColor = System.Drawing.Color.Transparent;
            this.label_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_bottom.Location = new System.Drawing.Point(2, 2);
            this.label_bottom.Margin = new System.Windows.Forms.Padding(0);
            this.label_bottom.Name = "label_bottom";
            this.label_bottom.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_bottom.Size = new System.Drawing.Size(796, 41);
            this.label_bottom.TabIndex = 0;
            this.label_bottom.Text = "© 2025 PCB检测系统 v1.0  By SuperIdolDog\r\n";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.carousel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 45);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Radius = 10;
            this.panel2.RadiusAlign = AntdUI.TAlignRound.Right;
            this.panel2.Size = new System.Drawing.Size(420, 410);
            this.panel2.TabIndex = 21;
            this.panel2.Text = "panel2";
            // 
            // carousel1
            // 
            this.carousel1.Autoplay = true;
            this.carousel1.Dock = System.Windows.Forms.DockStyle.Fill;
            carouselItem1.Img = global::Wedjat.WinForm.Properties.Resources.banner;
            carouselItem2.Img = global::Wedjat.WinForm.Properties.Resources.微信图片_20251025204013_9_13;
            this.carousel1.Image.Add(carouselItem1);
            this.carousel1.Image.Add(carouselItem2);
            this.carousel1.ImageFit = AntdUI.TFit.Fill;
            this.carousel1.Location = new System.Drawing.Point(0, 0);
            this.carousel1.Name = "carousel1";
            this.carousel1.Radius = 10;
            this.carousel1.Size = new System.Drawing.Size(420, 410);
            this.carousel1.TabIndex = 0;
            this.carousel1.Text = "carousel1";
            // 
            // panel3
            // 
            this.panel3.Back = System.Drawing.Color.Transparent;
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.select_productLine);
            this.panel3.Controls.Add(this.label_line);
            this.panel3.Controls.Add(this.label_pwd);
            this.panel3.Controls.Add(this.input_Password);
            this.panel3.Controls.Add(this.btn_Login);
            this.panel3.Controls.Add(this.label_workid);
            this.panel3.Controls.Add(this.input_WorkId);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(420, 45);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(380, 410);
            this.panel3.TabIndex = 22;
            this.panel3.Text = "panel3";
            // 
            // select_productLine
            // 
            this.select_productLine.Items.AddRange(new object[] {
            "DIP1",
            "DIP2"});
            this.select_productLine.List = true;
            this.select_productLine.Location = new System.Drawing.Point(29, 55);
            this.select_productLine.Name = "select_productLine";
            this.select_productLine.Radius = 0;
            this.select_productLine.SelectedIndex = 0;
            this.select_productLine.SelectedValue = "DIP1";
            this.select_productLine.Size = new System.Drawing.Size(322, 53);
            this.select_productLine.TabIndex = 26;
            this.select_productLine.Text = "DIP1";
            // 
            // label_line
            // 
            this.label_line.BackColor = System.Drawing.Color.Transparent;
            this.label_line.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_line.Location = new System.Drawing.Point(29, 17);
            this.label_line.Margin = new System.Windows.Forms.Padding(0);
            this.label_line.Name = "label_line";
            this.label_line.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_line.Size = new System.Drawing.Size(322, 33);
            this.label_line.TabIndex = 25;
            this.label_line.Text = "产线选择:";
            // 
            // label_pwd
            // 
            this.label_pwd.BackColor = System.Drawing.Color.Transparent;
            this.label_pwd.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_pwd.Location = new System.Drawing.Point(29, 209);
            this.label_pwd.Margin = new System.Windows.Forms.Padding(0);
            this.label_pwd.Name = "label_pwd";
            this.label_pwd.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_pwd.Size = new System.Drawing.Size(322, 33);
            this.label_pwd.TabIndex = 21;
            this.label_pwd.Text = "密    码:";
            // 
            // input_Password
            // 
            this.input_Password.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.input_Password.Location = new System.Drawing.Point(29, 245);
            this.input_Password.Name = "input_Password";
            this.input_Password.Radius = 0;
            this.input_Password.Size = new System.Drawing.Size(322, 53);
            this.input_Password.TabIndex = 23;
            this.input_Password.Text = "123456";
            this.input_Password.UseSystemPasswordChar = true;
            // 
            // btn_Login
            // 
            this.btn_Login.Location = new System.Drawing.Point(29, 305);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Radius = 0;
            this.btn_Login.Size = new System.Drawing.Size(322, 89);
            this.btn_Login.TabIndex = 20;
            this.btn_Login.Text = "进入系统";
            this.btn_Login.Type = AntdUI.TTypeMini.Primary;
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // label_workid
            // 
            this.label_workid.BackColor = System.Drawing.Color.Transparent;
            this.label_workid.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_workid.Location = new System.Drawing.Point(29, 113);
            this.label_workid.Margin = new System.Windows.Forms.Padding(0);
            this.label_workid.Name = "label_workid";
            this.label_workid.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_workid.Size = new System.Drawing.Size(322, 33);
            this.label_workid.TabIndex = 22;
            this.label_workid.Text = "工    号:";
            // 
            // input_WorkId
            // 
            this.input_WorkId.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.input_WorkId.Location = new System.Drawing.Point(29, 151);
            this.input_WorkId.Name = "input_WorkId";
            this.input_WorkId.Radius = 0;
            this.input_WorkId.Size = new System.Drawing.Size(322, 53);
            this.input_WorkId.TabIndex = 24;
            this.input_WorkId.Text = "WXXY2024011624";
            // 
            // FormLogin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pageHeader_Login);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormLogin";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.PageHeader pageHeader_Login;
        private AntdUI.Panel panel1;
        private AntdUI.Label label_bottom;
        private AntdUI.Panel panel2;
        private AntdUI.Carousel carousel1;
        private AntdUI.Panel panel3;
        private AntdUI.Select select_productLine;
        private AntdUI.Label label_line;
        private AntdUI.Label label_pwd;
        private AntdUI.Input input_Password;
        private AntdUI.Button btn_Login;
        private AntdUI.Label label_workid;
        private AntdUI.Input input_WorkId;
    }
}