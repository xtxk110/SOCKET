namespace SocketServerCloud
{
    partial class SocketCloud
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_listen = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_inner_iis = new System.Windows.Forms.TextBox();
            this.txt_inner_socket = new System.Windows.Forms.TextBox();
            this.txt_cloud_socket = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lb_count = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_socket = new System.Windows.Forms.ComboBox();
            this.btn_log = new System.Windows.Forms.Button();
            this.txt_message = new System.Windows.Forms.RichTextBox();
            this.btn_clear = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_clear);
            this.panel1.Controls.Add(this.btn_close);
            this.panel1.Controls.Add(this.btn_listen);
            this.panel1.Controls.Add(this.btn_save);
            this.panel1.Controls.Add(this.txt_inner_iis);
            this.panel1.Controls.Add(this.txt_inner_socket);
            this.panel1.Controls.Add(this.txt_cloud_socket);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lb_count);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cb_socket);
            this.panel1.Controls.Add(this.btn_log);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1017, 88);
            this.panel1.TabIndex = 0;
            // 
            // btn_close
            // 
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.Enabled = false;
            this.btn_close.Location = new System.Drawing.Point(691, 16);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 23);
            this.btn_close.TabIndex = 16;
            this.btn_close.Text = "关闭监听";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_listen
            // 
            this.btn_listen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_listen.Location = new System.Drawing.Point(787, 16);
            this.btn_listen.Name = "btn_listen";
            this.btn_listen.Size = new System.Drawing.Size(75, 23);
            this.btn_listen.TabIndex = 15;
            this.btn_listen.Text = "启动监听";
            this.btn_listen.UseVisualStyleBackColor = true;
            this.btn_listen.Click += new System.EventHandler(this.btn_listen_Click);
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.Location = new System.Drawing.Point(895, 49);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 14;
            this.btn_save.Text = "保存配置";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_inner_iis
            // 
            this.txt_inner_iis.Location = new System.Drawing.Point(668, 49);
            this.txt_inner_iis.Name = "txt_inner_iis";
            this.txt_inner_iis.Size = new System.Drawing.Size(194, 25);
            this.txt_inner_iis.TabIndex = 11;
            // 
            // txt_inner_socket
            // 
            this.txt_inner_socket.Location = new System.Drawing.Point(394, 49);
            this.txt_inner_socket.Name = "txt_inner_socket";
            this.txt_inner_socket.Size = new System.Drawing.Size(194, 25);
            this.txt_inner_socket.TabIndex = 12;
            // 
            // txt_cloud_socket
            // 
            this.txt_cloud_socket.Location = new System.Drawing.Point(95, 49);
            this.txt_cloud_socket.Name = "txt_cloud_socket";
            this.txt_cloud_socket.Size = new System.Drawing.Size(194, 25);
            this.txt_cloud_socket.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(592, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "内部IIS:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(293, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "内部SOCKET:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "云SOCKET:";
            // 
            // lb_count
            // 
            this.lb_count.AutoSize = true;
            this.lb_count.Location = new System.Drawing.Point(156, 19);
            this.lb_count.Name = "lb_count";
            this.lb_count.Size = new System.Drawing.Size(15, 15);
            this.lb_count.TabIndex = 7;
            this.lb_count.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "LIVE SOCKET客户端:";
            // 
            // cb_socket
            // 
            this.cb_socket.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_socket.FormattingEnabled = true;
            this.cb_socket.Location = new System.Drawing.Point(198, 15);
            this.cb_socket.Name = "cb_socket";
            this.cb_socket.Size = new System.Drawing.Size(260, 23);
            this.cb_socket.TabIndex = 5;
            // 
            // btn_log
            // 
            this.btn_log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_log.Location = new System.Drawing.Point(894, 16);
            this.btn_log.Name = "btn_log";
            this.btn_log.Size = new System.Drawing.Size(75, 23);
            this.btn_log.TabIndex = 4;
            this.btn_log.Text = "显示日志";
            this.btn_log.UseVisualStyleBackColor = true;
            this.btn_log.Click += new System.EventHandler(this.btn_log_Click);
            // 
            // txt_message
            // 
            this.txt_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_message.Location = new System.Drawing.Point(0, 88);
            this.txt_message.Name = "txt_message";
            this.txt_message.ReadOnly = true;
            this.txt_message.Size = new System.Drawing.Size(1017, 523);
            this.txt_message.TabIndex = 1;
            this.txt_message.Text = "";
            this.txt_message.WordWrap = false;
            // 
            // btn_clear
            // 
            this.btn_clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_clear.Location = new System.Drawing.Point(595, 16);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(75, 23);
            this.btn_clear.TabIndex = 17;
            this.btn_clear.Text = "清除日志";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // SocketCloud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 611);
            this.Controls.Add(this.txt_message);
            this.Controls.Add(this.panel1);
            this.Name = "SocketCloud";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_count;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_socket;
        private System.Windows.Forms.Button btn_log;
        private System.Windows.Forms.RichTextBox txt_message;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.TextBox txt_inner_iis;
        private System.Windows.Forms.TextBox txt_inner_socket;
        private System.Windows.Forms.TextBox txt_cloud_socket;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_listen;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_clear;
    }
}

