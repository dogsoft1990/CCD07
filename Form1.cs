namespace CCD07Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Wx07.WcswBases(textBox1.Text.HexToByte()).ToLook(" ");
        }
    }
}