using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hot_Notepad
{
    public partial class Form1 : Form
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        bool hide = false;

        public Form1()
        {
            InitializeComponent();
            int id = 0;     // The id of the hotkey. 
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Alt, Keys.N.GetHashCode());
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        string npath = Environment.CurrentDirectory + "\\Hot_Notepad.txt";

        private void Form1_Load(object sender, EventArgs e)//when program start
        {
            start();
        }

        private void start() {//when program start
            try
            {
                string readText = File.ReadAllText(npath);
                tnote.Text = readText;
            }
            catch
            {
                fStartMsg();
                using (FileStream fs = File.Create(npath))
                {
                }
            }
            
        }

        private void end() {//do with the Program
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(npath)))
            {
                foreach (string line in tnote.Lines)
                    outputFile.WriteLine(line);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//Closed Program
        {
            end();
            UnregisterHotKey(this.Handle, 0);
        }

        
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                hR(true);          
            }
        }

        private bool hR(bool arg) {//True or False - (Set Location at cursor)
            if (hide == false)
            {// do something
                this.Hide();
                hide = true;
            }
            else
            {
                this.Show();
                hide = false;
                if (arg == true)
                {
                    this.Location = new Point(Cursor.Position.X - (this.Width / 2), Cursor.Position.Y - (this.Height / 2));
                }
                end();
            }
            return false;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            hide = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fStartMsg();
        }

        private void fStartMsg() { //Help And Start Message
            MessageBox.Show("Notepad have autosave at closed and hide\nAll notes are kept at Hot_Notepad.txt\n\nALT+N - show or hide\nDouble click on icon(tray) - show or hide notepad\nRight click(tray) - show stripmenu(Help and exit)\n\nv. 0.0.1 Alpha"); 
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            hR(false);
        }
    }
}
