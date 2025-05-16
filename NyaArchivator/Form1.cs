
using HuffmanNM;
using System.IO;

namespace NyaArchivator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = Path.GetFileNameWithoutExtension(openFile.FileName);
                CommonData.filepath = Path.GetDirectoryName(openFile.FileName);
                //CommonData.FileOutDictionary = File.CreateText(CommonData.filepath + "\\" + "Codes.txt");
                
                CommonData.FileOutDictionary = new StreamWriter(CommonData.filepath + "\\" + "Codes.txt", false);
                
                Huffman huffman = new Huffman();
                string st1 = filename + ".txt";
                string st2 = filename + ".nya";
                File.Copy(CommonData.filepath + "\\" + st1, st1);
                huffman.CodeFile(st1, st2);
                if (File.Exists(CommonData.filepath + "\\" + st2))
                    File.Delete(CommonData.filepath + "\\" + st2);
                File.Copy(st2, CommonData.filepath + "\\" + st2);
                File.Delete(st1);
                File.Delete(st2);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = Path.GetFileNameWithoutExtension(openFile.FileName);
                string filepath = Path.GetDirectoryName(openFile.FileName);
                Huffman huffman = new Huffman();
                string st1 = filename + ".txt";
                string st2 = filename + ".nya";
                File.Copy(filepath + "\\" + st2, st2);
                huffman.DecodeFile(st2, st1);
                if (File.Exists(filepath + "\\" + st1))
                    File.Delete(filepath + "\\" + st1);
                File.Copy(st1, filepath + "\\" + st1);
                File.Delete(st1);
                File.Delete(st2);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
