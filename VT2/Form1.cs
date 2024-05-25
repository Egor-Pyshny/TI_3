using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace VT2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string getkey(string s) {
            StringBuilder res = new StringBuilder();
            int len = 0;
            for (int i = 0; i < s.Length; i++) {
                if (s[i] == '1' || s[i] == '0')
                {
                    res.Append(s[i]);
                    len++;
                }
                if (len == 28) break;
            }
            while (res.Length < 28) { res.Append('1');}
            textBox1.Text = res.ToString();
            return res.ToString();
        }
        private string shl(string s) {
            StringBuilder res = new StringBuilder(s.Substring(1)+"0"); 
            return res.ToString();
        }

        private void cipher(object key) { 
            string temp = key.ToString();
            int newbit;
            byte[] arr = new byte[1];
            FileStream filein = new FileStream(openFileDialog1.FileName, FileMode.Open);
            FileStream fileout = new FileStream(openFileDialog2.FileName, FileMode.Open);
            FileInfo info = new FileInfo(openFileDialog1.FileName);
            int milsec = (int)info.Length / 255;
            float seconds =(float) milsec / 1000;
            BinaryReader reader = new BinaryReader(filein);
            BinaryWriter writer = new BinaryWriter(fileout);
            Form f2 = new Form();
            f2.Visible = true;            
            f2.Size = new Size(760, 475);
            TextBox[] textarr = new TextBox[3];
            Label[] lbl = new Label[4];
            int x = 20, y = 50,j=0;
            for (int i = 0; i < 3; i++)
            {
                textarr[i] = new TextBox();
                lbl[i] = new Label();
                textarr[i].Location = new Point(x, y);
                textarr[i].Size = new Size(220, 350);
                textarr[i].ReadOnly = true;
                textarr[i].Multiline = true;
                textarr[i].ScrollBars = ScrollBars.Vertical;
                lbl[i].Location = new Point(x, y - 25);
                x += 240;
                f2.Controls.Add(textarr[i]);
                f2.Controls.Add(lbl[i]);
            }
            lbl[3] = new Label();
            lbl[3].Location = new Point(20, 410);
            lbl[3].Size = new Size(700, 15);
            lbl[3].Text = "Процесс займет примерно " + seconds.ToString() + " секунд";
            f2.Controls.Add(lbl[3]);
            lbl[0].Text = "Исходный Текст";
            lbl[1].Text = "Ключ";
            lbl[2].Text = "Результат";
            f2.Refresh();
            try
            {
                while (true)
                {
                    StringBuilder keybuilder = new StringBuilder();
                    int _byte = (int)reader.ReadByte();
                    arr[0] = (byte)_byte;
                    if (j < 60)
                    { 
                        textarr[0].AppendText(Encoding.UTF8.GetChars(arr)[0].ToString());
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        newbit = (temp[0] - '0') ^ (temp[25] - '0');
                        keybuilder.Append(temp[0]);
                        temp = shl(temp);
                        StringBuilder tmp = new StringBuilder(temp);
                        tmp.Remove(27, 1);
                        tmp.Append(newbit);
                        temp = tmp.ToString(); 
                    }
                    string k = keybuilder.ToString();
                    int sec = Convert.ToByte(k, 2);
                    int res = _byte ^ sec;
                    keybuilder.Clear();
                    arr[0] = (byte)res;
                    writer.Write((byte)res);
                    if (j < 60)
                    {               
                        textarr[1].AppendText(k+"_");
                        textarr[2].AppendText(Encoding.UTF8.GetChars(arr)[0].ToString());
                        j++;
                    }
                }
            }
            catch (EndOfStreamException) { }
            for (int i = 0; i < 3; i++) { textarr[i].BackColor = Color.FromArgb(148, 209, 153); }
            filein.Close();
            fileout.Close();
            Application.Run(f2);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.FileName != "" && openFileDialog2.FileName != "")
            {
                string text = getkey(textBox1.Text);
                Stopwatch st = new Stopwatch();
                st.Start();
                Thread thread = new Thread(cipher);
                thread.Start(text);
                thread.Join();
                st.Stop();
                textBox2.Text = st.ElapsedMilliseconds.ToString();
            }
            else {
                Form2 exc = new Form2();
                exc.Show();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string temp = getkey(textBox1.Text);
            string text = temp;
            int j = 0;
            while (true)
            {           
                int newbit;
                StringBuilder keybuilder = new StringBuilder();               
                for (int i = 0; i < 8; i++)
                {
                    newbit = (temp[0] - '0') ^ (temp[25] - '0');
                    keybuilder.Append(temp[0]);
                    temp = shl(temp);
                    StringBuilder tmp = new StringBuilder(temp);
                    tmp.Remove(27, 1);
                    tmp.Append(newbit);
                    temp = tmp.ToString();
                    j++;
                    if (temp == text) { textBox2.Text = j.ToString();break; }
                }
                if (temp == text) { 
                    textBox2.Text = j.ToString(); break; 
                }
                keybuilder.Clear();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }
    }
}
