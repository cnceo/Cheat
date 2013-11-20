namespace GXService.Broadcast.Client
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbNickName = new System.Windows.Forms.TextBox();
            this.rtbAllMessages = new System.Windows.Forms.RichTextBox();
            this.rtbToSendMessage = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbNickName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "用户信息";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "昵称:";
            // 
            // tbNickName
            // 
            this.tbNickName.Location = new System.Drawing.Point(47, 28);
            this.tbNickName.Name = "tbNickName";
            this.tbNickName.Size = new System.Drawing.Size(121, 21);
            this.tbNickName.TabIndex = 1;
            // 
            // rtbAllMessages
            // 
            this.rtbAllMessages.Location = new System.Drawing.Point(12, 97);
            this.rtbAllMessages.Name = "rtbAllMessages";
            this.rtbAllMessages.ReadOnly = true;
            this.rtbAllMessages.Size = new System.Drawing.Size(369, 259);
            this.rtbAllMessages.TabIndex = 1;
            this.rtbAllMessages.Text = "";
            // 
            // rtbToSendMessage
            // 
            this.rtbToSendMessage.Location = new System.Drawing.Point(13, 378);
            this.rtbToSendMessage.Name = "rtbToSendMessage";
            this.rtbToSendMessage.Size = new System.Drawing.Size(368, 112);
            this.rtbToSendMessage.TabIndex = 2;
            this.rtbToSendMessage.Text = "";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(306, 501);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 536);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.rtbToSendMessage);
            this.Controls.Add(this.rtbAllMessages);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "GXService.Broadcast.Client";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNickName;
        private System.Windows.Forms.RichTextBox rtbAllMessages;
        private System.Windows.Forms.RichTextBox rtbToSendMessage;
        private System.Windows.Forms.Button btnSend;
    }
}

