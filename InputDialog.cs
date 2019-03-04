using System;
using System.Drawing;
using System.Windows.Forms;

namespace Torn.UI
{
	/// <summary>
	/// Get a string from the user. Why the f*** does .NET not have one of these built in?
	/// </summary>
	public partial class InputDialog : Form
	{
		public string Response { get { return textBox1.Text; } set { textBox1.Text = value; } }

		public InputDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		public InputDialog(string title, string message = "", string defaultResponse = ""): this()
		{
			Text = title;
			label1.Text = message;
			textBox1.Text = defaultResponse;
		}

		public static string GetInput(string title, string message, string defaultResponse = "")
		{
			var id = new InputDialog(title, message, defaultResponse);
			return id.ShowDialog() == DialogResult.OK ? id.Response : defaultResponse;
		}

		public static Boolean ConditionalInput(string title, string message, ref string response)
		{
			var id = new InputDialog(title, message, "");
			Boolean b = (id.ShowDialog() == DialogResult.OK);
			
			if (b)
				response = id.Response;
			return b;
		}

		public static Boolean UpdateInput(string title, string message, ref string response)
		{
			var id = new InputDialog(title, message, response);
			Boolean b = (id.ShowDialog() == DialogResult.OK);
			
			if (b)
				response = id.Response;
			return b;
		}

		public static int GetInteger(string title, string message, int defaultResponse = 0)
		{
			var id = new InputDialog(title, message);
			id.SetNumeric(defaultResponse);
			return id.ShowDialog() == DialogResult.OK ? (int)id.numericUpDown1.Value : defaultResponse;
		}

		public static bool GetInteger(string title, string message, ref int response)
		{
			var id = new InputDialog(title, message);
			id.SetNumeric(response);
			Boolean b = (id.ShowDialog() == DialogResult.OK);
			
			if (b)
				response = (int)id.numericUpDown1.Value;
			return b;
		}

		public static double? GetDouble(string title, string message, double? defaultResponse = 0)
		{
			var id = new InputDialog(title, message);
			id.SetNumeric(defaultResponse == null ? 0 : (decimal)defaultResponse);
			return id.ShowDialog() == DialogResult.OK ? (double)id.numericUpDown1.Value : defaultResponse;
		}

		void SetNumeric(decimal value)
		{
			textBox1.Visible = false;
			numericUpDown1.Visible = true;
			numericUpDown1.Focus();
			numericUpDown1.Value = value;
		}
	}
}
