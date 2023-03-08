using System.Runtime.InteropServices;
using static System.Windows.Forms.AxHost;
using System.Windows.Forms;
using System.Diagnostics;

namespace overlay
{
    public partial class Form1 : Form
    {
        private string Text = "";
        private string ImgPath = "";
        private bool Hidden = false;
        private Color Color = Color.Yellow;
        private int Size = 10;
        private IntPtr Hook = IntPtr.Zero;
        public Form1()
        {
            InitializeComponent();
            if(File.Exists("text.txt"))
                richTextBox1.Text = File.ReadAllText("text.txt");
            this.AllowTransparency = true;
            button3.BackColor = Color;
            label1.Font = new Font("Arial", Size);
            Hook = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, CheckPress, 0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText("text.txt", richTextBox1.Text);
            this.TopMost = true;
            this.Controls.Remove(richTextBox1);
            this.Controls.Remove(button1);
            Controls.Remove(button2);
            this.TransparencyKey = BackColor;
            this.FormBorderStyle = FormBorderStyle.None;
            var l = new Label() { Text = richTextBox1.Text, ForeColor = Color, Font = new Font("Arial", Size), AutoSize = true, MaximumSize = new Size(200,0) };
            this.Controls.Add(l);
            this.Top = 0;
            this.Left = Screen.PrimaryScreen.Bounds.Width - 200;
            this.Width = 200;
            this.Height = l.Height;
            this.MouseDown += _MouseDown;
            this.MouseMove += _MouseMove;
            l.MouseDown += _MouseDown;
            l.MouseMove += _MouseMove;
        }

        private Point _startPosition;

        private void _MouseDown(object? sender, MouseEventArgs e)
        {
            _startPosition = e.Location;
        }

        private void _MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Location = new Point(Location.X + e.X - _startPosition.X, Location.Y + e.Y - _startPosition.Y);
        }

        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        private IntPtr CheckPress(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }
            if (wParam == (IntPtr)WM.WM_KEYDOWN)
            {
                var key = (Keys)Marshal.ReadInt32(lParam);
                if (key == Keys.NumPad4)
                {
                    if (!Hidden)
                    {
                        this.Hide();
                        Hidden = true;
                    }
                    else if (Hidden)
                    {
                        this.Show();
                        Hidden = false;
                    }
                }
            }
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button3.BackColor =  colorDialog1.Color;
                Color = colorDialog1.Color;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var size = int.Parse((sender as TextBox).Text);
                label1.Font = new Font("Arial", size);
            }
            catch
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(Hook);
        }
    }
}