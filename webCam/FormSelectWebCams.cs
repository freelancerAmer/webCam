using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecureChat.Common;

namespace SecureChat.Client
{
	public partial class FormSelectWebCams : Form
	{
		
		public FormSelectWebCams(WebCamInfo[] availableWebCams)
		{
			InitializeComponent();
			
			AvailableWebCams = availableWebCams;
			comboWebCams.Items.Clear();
			comboWebCams.Items.AddRange(availableWebCams);
			comboWebCams.SelectedIndex = 0;
		}
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			
			Result = null;
		}

		public WebCamInfo[] AvailableWebCams { get; private set; }
		public WebCamInfo Result { get; private set; }
		private void buttonOk_Click(object sender, EventArgs e)
		{
			Result = AvailableWebCams[comboWebCams.SelectedIndex];
			DialogResult = DialogResult.OK;
		}
	}
}
