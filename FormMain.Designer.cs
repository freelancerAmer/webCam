namespace SecureChat.Client
{
	partial class FormMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.splitHorizontal = new System.Windows.Forms.SplitContainer();
			this.listUsers = new System.Windows.Forms.ListBox();
			this.splitVertical = new System.Windows.Forms.SplitContainer();
			this.textConversation = new System.Windows.Forms.RichTextBox();
			this.contextMenuConversation = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textMessage = new System.Windows.Forms.TextBox();
			this.contextMenuListUsers = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clearSelectionsendsToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startWebCamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startSoundCaptureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.receiveSourceCaptureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timerAnimateIcons = new System.Windows.Forms.Timer(this.components);
			this.splitHorizontal.Panel1.SuspendLayout();
			this.splitHorizontal.Panel2.SuspendLayout();
			this.splitHorizontal.SuspendLayout();
			this.splitVertical.Panel1.SuspendLayout();
			this.splitVertical.Panel2.SuspendLayout();
			this.splitVertical.SuspendLayout();
			this.contextMenuConversation.SuspendLayout();
			this.contextMenuListUsers.SuspendLayout();
			this.contextMenuTray.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitHorizontal
			// 
			this.splitHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitHorizontal.Location = new System.Drawing.Point(0, 0);
			this.splitHorizontal.Name = "splitHorizontal";
			// 
			// splitHorizontal.Panel1
			// 
			this.splitHorizontal.Panel1.Controls.Add(this.listUsers);
			// 
			// splitHorizontal.Panel2
			// 
			this.splitHorizontal.Panel2.Controls.Add(this.splitVertical);
			this.splitHorizontal.Size = new System.Drawing.Size(694, 520);
			this.splitHorizontal.SplitterDistance = 167;
			this.splitHorizontal.TabIndex = 1;
			// 
			// listUsers
			// 
			this.listUsers.AllowDrop = true;
			this.listUsers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listUsers.FormattingEnabled = true;
			this.listUsers.Location = new System.Drawing.Point(0, 0);
			this.listUsers.Name = "listUsers";
			this.listUsers.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listUsers.Size = new System.Drawing.Size(167, 511);
			this.listUsers.TabIndex = 1;
			this.listUsers.DragOver += new System.Windows.Forms.DragEventHandler(this.listUsers_DragOver);
			this.listUsers.DragDrop += new System.Windows.Forms.DragEventHandler(this.listUsers_DragDrop);
			this.listUsers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listUsers_MouseDown);
			// 
			// splitVertical
			// 
			this.splitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitVertical.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitVertical.Location = new System.Drawing.Point(0, 0);
			this.splitVertical.Name = "splitVertical";
			this.splitVertical.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitVertical.Panel1
			// 
			this.splitVertical.Panel1.Controls.Add(this.textConversation);
			// 
			// splitVertical.Panel2
			// 
			this.splitVertical.Panel2.Controls.Add(this.textMessage);
			this.splitVertical.Size = new System.Drawing.Size(523, 520);
			this.splitVertical.SplitterDistance = 375;
			this.splitVertical.TabIndex = 0;
			// 
			// textConversation
			// 
			this.textConversation.ContextMenuStrip = this.contextMenuConversation;
			this.textConversation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textConversation.Location = new System.Drawing.Point(0, 0);
			this.textConversation.Name = "textConversation";
			this.textConversation.ReadOnly = true;
			this.textConversation.Size = new System.Drawing.Size(523, 375);
			this.textConversation.TabIndex = 0;
			this.textConversation.Text = "";
			// 
			// contextMenuConversation
			// 
			this.contextMenuConversation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
			this.contextMenuConversation.Name = "contextMenuConversation";
			this.contextMenuConversation.Size = new System.Drawing.Size(111, 26);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
			this.clearToolStripMenuItem.Text = "Clear";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
			// 
			// textMessage
			// 
			this.textMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textMessage.Location = new System.Drawing.Point(0, 0);
			this.textMessage.Multiline = true;
			this.textMessage.Name = "textMessage";
			this.textMessage.Size = new System.Drawing.Size(523, 141);
			this.textMessage.TabIndex = 0;
			this.textMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textMessage_KeyDown);
			// 
			// contextMenuListUsers
			// 
			this.contextMenuListUsers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearSelectionsendsToAllToolStripMenuItem,
            this.startWebCamToolStripMenuItem,
            this.startSoundCaptureToolStripMenuItem,
            this.receiveSourceCaptureToolStripMenuItem});
			this.contextMenuListUsers.Name = "contextMenuListUsers";
			this.contextMenuListUsers.Size = new System.Drawing.Size(196, 114);
			// 
			// clearSelectionsendsToAllToolStripMenuItem
			// 
			this.clearSelectionsendsToAllToolStripMenuItem.Name = "clearSelectionsendsToAllToolStripMenuItem";
			this.clearSelectionsendsToAllToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.clearSelectionsendsToAllToolStripMenuItem.Text = "Clear selection";
			this.clearSelectionsendsToAllToolStripMenuItem.Click += new System.EventHandler(this.clearSelectionsendsToAllToolStripMenuItem_Click);
			// 
			// startWebCamToolStripMenuItem
			// 
			this.startWebCamToolStripMenuItem.Name = "startWebCamToolStripMenuItem";
			this.startWebCamToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.startWebCamToolStripMenuItem.Text = "Start WebCam";
			this.startWebCamToolStripMenuItem.Click += new System.EventHandler(this.startWebCamToolStripMenuItem_Click);
			// 
			// startSoundCaptureToolStripMenuItem
			// 
			this.startSoundCaptureToolStripMenuItem.Name = "startSoundCaptureToolStripMenuItem";
			this.startSoundCaptureToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.startSoundCaptureToolStripMenuItem.Text = "Capture my sound";
			this.startSoundCaptureToolStripMenuItem.Click += new System.EventHandler(this.startSoundCaptureToolStripMenuItem_Click);
			// 
			// receiveSourceCaptureToolStripMenuItem
			// 
			this.receiveSourceCaptureToolStripMenuItem.Name = "receiveSourceCaptureToolStripMenuItem";
			this.receiveSourceCaptureToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
			this.receiveSourceCaptureToolStripMenuItem.Text = "Receive sound capture";
			this.receiveSourceCaptureToolStripMenuItem.Click += new System.EventHandler(this.receiveSoundCaptureToolStripMenuItem_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenuTray;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "SecureChat";
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
			// 
			// contextMenuTray
			// 
			this.contextMenuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
			this.contextMenuTray.Name = "contextMenuTray";
			this.contextMenuTray.Size = new System.Drawing.Size(112, 54);
			// 
			// showToolStripMenuItem
			// 
			this.showToolStripMenuItem.Name = "showToolStripMenuItem";
			this.showToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.showToolStripMenuItem.Text = "Show";
			this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(108, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// timerAnimateIcons
			// 
			this.timerAnimateIcons.Interval = 26;
			this.timerAnimateIcons.Tick += new System.EventHandler(this.timerAnimateIcons_Tick);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(694, 520);
			this.Controls.Add(this.splitHorizontal);
			this.Name = "FormMain";
			this.Text = "SecureChat client";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.splitHorizontal.Panel1.ResumeLayout(false);
			this.splitHorizontal.Panel2.ResumeLayout(false);
			this.splitHorizontal.ResumeLayout(false);
			this.splitVertical.Panel1.ResumeLayout(false);
			this.splitVertical.Panel2.ResumeLayout(false);
			this.splitVertical.Panel2.PerformLayout();
			this.splitVertical.ResumeLayout(false);
			this.contextMenuConversation.ResumeLayout(false);
			this.contextMenuListUsers.ResumeLayout(false);
			this.contextMenuTray.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitHorizontal;
		internal System.Windows.Forms.ListBox listUsers;
		private System.Windows.Forms.SplitContainer splitVertical;
		private System.Windows.Forms.TextBox textMessage;
		internal System.Windows.Forms.RichTextBox textConversation;
		private System.Windows.Forms.ContextMenuStrip contextMenuConversation;
		private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenuStrip contextMenuTray;
		private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		internal System.Windows.Forms.Timer timerAnimateIcons;
		private System.Windows.Forms.ContextMenuStrip contextMenuListUsers;
		private System.Windows.Forms.ToolStripMenuItem clearSelectionsendsToAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startWebCamToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startSoundCaptureToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem receiveSourceCaptureToolStripMenuItem;


	}
}

