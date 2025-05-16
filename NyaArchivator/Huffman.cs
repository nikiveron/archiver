
using Microsoft.VisualBasic.ApplicationServices;
using NyaArchivator;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Collections;
using System.Text;

namespace HuffmanNM
{
    class Node
    {
        public byte symbol; //что хранится в узле
        public int freq;    //частота исп узла
        public Node left;   //ссылка на левый узел
        public Node right;   //ссылка на правый узел
        public Node(byte symbol, int freq)
        {
            this.symbol = symbol;
            this.freq = freq;
        }
        public Node(Node left, Node right, int freq)
        {
            this.left = left;
            this.right = right;
            this.freq = freq;
        }
        public static void Preorder(Node r, string[] codes, ref Dictionary<string, string> dic) //прямой обход дерева
        {
            
            if (r != null)
            {
                if (r.symbol != 0)
                {
                    string str = Encoding.ASCII.GetString(new byte[] { r.symbol });
                    dic.Add(codes[(int)r.symbol], str);
                }
                Preorder(r.left, codes, ref dic);
                Preorder(r.right, codes, ref dic);
                
            }
        }
    }
    class Huffman
    {
        public void CodeFile(string dataFilename, string archFilename)
        {
            byte[] data = File.ReadAllBytes(dataFilename);
            byte[] arch = CodeBytes(data);
            File.WriteAllBytes(archFilename, arch);
        }
        public void DecodeFile(string archFilename, string dataFilename)
        {
            byte[] arch = File.ReadAllBytes(archFilename);
            byte[] data = DecodeBytes(arch);
            File.WriteAllBytes(dataFilename, data);
        }
        private byte[] DecodeBytes(byte[] arch)
        {
            ParseHeader(arch, out int datalen, out int startInd, out int[] frequency);
            Node root = CreateHuffmanTree(frequency); //составляем дерево хаффмана
            byte[] data = Decompress(arch, startInd, datalen, root);
            return data;        
        }
        private void ParseHeader(byte[] arch, out int datalen, out int startInd, out int[] frequency)
        {
            datalen = arch[0] |
                (arch[1]<< 8)|
                (arch[2]<< 16)|
                (arch[3]<< 24);
            frequency = new int[256];
            for (int i = 0; i < 256; i++)
                frequency[i] = arch[4 + i];
            startInd = 4 + 256;
        }
        private byte[] Decompress(byte[] arch, int startInd, int dataLen, Node root)
        {
            int size = 0;
            Node cur = root;
            List<byte> data = new List<byte>();
            for (int i = startInd; i < arch.Length; i++)
            {
                for (int j = 1; j <= 128; j <<= 1)
                {
                    bool zero = (arch[i] & j) == 0;
                    if (zero)
                        cur = cur.left;
                    else
                         cur = cur.right;
                    if (cur.left != null)
                        continue;
                    if (size++ < dataLen)
                        data.Add(cur.symbol);
                    cur = root;
                }
            }
            return data.ToArray();
        }
        public Dictionary<string, string> Preorder(Node root, string[] codes)
        {
            Dictionary<string, string> dicBit = new Dictionary<string, string>();
            Node.Preorder(root, codes, ref dicBit);
            return dicBit;
        }
        
        private byte[] CodeBytes(byte[] data)
        {
            int[] frequency = CalculateFreq(data); //создаем частотный словарь
            byte[] header = CreateHeader(data.Length, frequency);
            Node root = CreateHuffmanTree(frequency); //составляем дерево хаффмана
            string[] codes = CreateHuffmanCodes(root); //составляем для каждой буквы свой код
            Dictionary<string, string> dicBit = Preorder(root, codes);
            string[] CodesArray = dicBit.Keys.ToArray();
            string[] CharArray = dicBit.Values.ToArray();
            for(int i = 0; i< CodesArray.Length; i++)
            {
                CommonData.FileOutDictionary.WriteLine("{0} = {1}", CharArray, CodesArray);
                
            }
            byte[] bits = Compress(data, codes);
            return header.Concat(bits).ToArray();
        }
        
        private int[] CalculateFreq(byte[] data) //вычисление частоты появления буквы
        {
            int[] frequency = new int[256]; //пустой словарь, 256 тк столько значений в байте
            foreach (byte d in data)
            {
                frequency[d]++;
            }
            NormalizeFreqs();
            return frequency;
            void NormalizeFreqs()
            {
                int max = frequency.Max();
                if (max <= 255) return;
                for (int i = 0; i < 256; i++)
                    if (frequency[i] > 0)
                        frequency[i] = 1 + frequency[i] * 255 / (max + 1);
            }
        }
        
        private Node CreateHuffmanTree(int[] freqs) //создаем дерево хаффмана
        {
            PriorityQueue<Node> pq = new PriorityQueue<Node>(); //создаем приоритетную очередь
            for (int i = 0; i < 256; i++)
            {
                if (freqs[i] > 0)
                    pq.Enqueue(freqs[i], new Node((byte)i, freqs[i]));
            }
            while (pq.Size() > 1)
            {
                Node left = pq.Dequeue();
                Node right = pq.Dequeue();
                int freq = left.freq + right.freq;
                Node next = new Node(left, right, freq);
                pq.Enqueue((byte)freq, next);
            }
            return pq.Dequeue();
        }
        private string[] CreateHuffmanCodes(Node root) //записываем для каждой буквы свой код
        {
            string[] codes = new string[256];
            Next(root, "");

            return codes;
            void Next(Node node, string code)
            {
                if (node.left == null)
                    codes[node.symbol] = code;
                else
                {
                    Next(node.left, code + "0");
                    Next(node.right, code + "1");
                }
            }
        }
        private byte[] Compress(byte[] data, string[] codes) //перевод фулл кода в десятичное число
        {
            List<byte> result = new List<byte>();
            byte sum = 0;
            byte bit = 1;
            foreach (byte b in data)
            {
                foreach (char c in codes[b])
                {
                    if (c == '1')
                        sum |= bit; //дизъюнкция
                    if (bit < 128)
                        bit <<= 1; //сдвиг на единицу
                    else
                    {
                        result.Add(sum);
                        sum = 0;
                        bit = 1;
                    }
                }
            }
            if (bit > 1)
                result.Add(sum);
            return result.ToArray();
        }
        private byte[] CreateHeader(int length, int[] freqs) //создаем заголовок файла для запоминания длины, чтобы файл декодировать
        {
            List<byte> head = new List<byte>();
            head.Add((byte)(length & 255));
            head.Add((byte)((length >> 8) & 255));
            head.Add((byte)((length >> 16) & 255));
            head.Add((byte)((length >> 24) & 255));
            for (int i = 0; i < 256; i++)
                head.Add((byte)freqs[i]);
            return head.ToArray();
        }
    }
}
