using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AgacOdevi.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AgacOdevi
{
    
    public partial class Form1 : Form
    {
        Tree tree;
        public Form1()
        {
            
            InitializeComponent();
            tree = new Tree();
            this.panel1.Paint += new PaintEventHandler(this.panel1_Paint);  // Paint olayına bağla
        }
        public class Node
        {
            public int data;
            public Node right;
            public Node left;
            public int x;
            public int y;
            public Node(int data)
            {

                this.data = data;
                this.right = null;
                this.left = null;
                this.x = 0;
                this.y = 0;
            }

        }
        // Ağaç çizecek olan Paint olayını yazıyoruz
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Eğer ağacın root düğümü varsa, ağacı çizmeye başla
            if (tree.root != null)
            {
                // Koordinatları belirle ve ağacı çiz
                tree.setNodeCoordinates(tree.root, tree.offsetX, tree.offsetY);
                DrawTree(tree.root, e.Graphics, panel1.Width, panel1.Height);
            }
        }
        private void DrawTree(Node root, Graphics g, int width, int height)
        {
            if (root == null) return;

            // Ağacın derinliği ve düğüm sayısını hesapla
            int totalDepth = tree.getHeight(root);  // Ağaç derinliği
            int totalWidth = tree.countNodes(root);  // Ağaçtaki toplam düğüm sayısı

            // X ve Y ekseninde ölçek faktörlerini ayarla
            int scaleX = (totalWidth > 1) ? (width - 200) / (totalWidth - 1) : 1;  // X ekseninde düğümler arası mesafe
            int scaleY = (totalDepth > 1) ? (height - 200) / (totalDepth - 1) : 1;  // Y ekseninde düğümler arası mesafe

            // Düğümün ekran koordinatlarını hesapla
            SetNodeCoordinates(root, scaleX, scaleY, width / 2, 40);  // Düğüm koordinatlarını ayarla

            // Düğüm çizme ve alt ağaçları çizme
            DrawNodeAndConnections(root, g, scaleX, scaleY, width, height);
        }
        private void SetNodeCoordinates(Node node, int scaleX, int scaleY, int offsetX, int offsetY)
        {
            if (node == null) return;

            node.x = offsetX;
            node.y = offsetY;

            // Sol alt ağaç için koordinatları ayarla
            if (node.left != null)
            {
                SetNodeCoordinates(node.left, scaleX, scaleY, offsetX - scaleX, offsetY + scaleY);
            }

            // Sağ alt ağaç için koordinatları ayarla
            if (node.right != null)
            {
                SetNodeCoordinates(node.right, scaleX, scaleY, offsetX + scaleX, offsetY + scaleY);
            }
        }
        private void DrawNodeAndConnections(Node node, Graphics g, int scaleX, int scaleY, int width, int height)
        {
            if (node == null) return;

            // Düğümün ekran koordinatlarını hesapla
            int drawX = node.x;
            int drawY = node.y;

            // Düğümü çiz
            g.FillEllipse(Brushes.LightBlue, drawX - 20, drawY - 20, 40, 40);  // Düğüm çemberi
            g.DrawString(node.data.ToString(), this.Font, Brushes.Black, drawX - 10, drawY - 10);  // Düğüm verisi

            // Sol alt ağacı çiz
            if (node.left != null)
            {
                // Sol çizgi: Çizgiyi kısaltmak için biraz daha yakınlaştırıyoruz
                int leftX = node.left.x;
                int leftY = node.left.y;

                // Çizgiyi biraz daha kısalt
                int shortenFactor = 10;  // Çizgiyi kısaltma oranı
                g.DrawLine(Pens.Black, drawX + shortenFactor, drawY + shortenFactor, leftX + shortenFactor, leftY + shortenFactor);  // Sol çizgi
                DrawNodeAndConnections(node.left, g, scaleX, scaleY, width, height);  // Sol alt ağacı çiz
            }

            // Sağ alt ağacı çiz
            if (node.right != null)
            {
                // Sağ çizgi: Çizgiyi kısaltmak için biraz daha yakınlaştırıyoruz
                int rightX = node.right.x;
                int rightY = node.right.y;

                // Çizgiyi biraz daha kısalt
                int shortenFactor = 10;  // Çizgiyi kısaltma oranı
                g.DrawLine(Pens.Black, drawX + shortenFactor, drawY + shortenFactor, rightX + shortenFactor, rightY + shortenFactor);  // Sağ çizgi
                DrawNodeAndConnections(node.right, g, scaleX, scaleY, width, height);  // Sağ alt ağacı çiz
            }
        }
        public class Tree
        {
            public Node root;
            public int offsetX = 2;
            public int offsetY = 2;

            public Tree()
            {
                root = null;
            }
            
            public Node newNode(int data)
            {
                return new Node(data);
            }
            public int countNodes(Node root)
            {
                if (root == null)
                    return 0;  // Eğer düğüm null ise, 0 döndür

                // Sol ve sağ alt ağaçtaki düğüm sayıları ile kendi düğüm sayısını toplar
                return 1 + countNodes(root.left) + countNodes(root.right);
            }
            public int getHeight(Node root)
            {
                if (root == null)
                {
                    return 0;  // Eğer düğüm null ise, yükseklik 0 (boş ağaç)
                }

                // Sol ve sağ alt ağacın yüksekliğini hesapla
                int leftHeight = getHeight(root.left);
                int rightHeight = getHeight(root.right);

                // En yüksek olan alt ağacın yüksekliğine 1 ekleyerek döndür
                return Math.Max(leftHeight, rightHeight) + 1;
            }
            public Node insert(Node root, int data)
            {
                if (root == null)
                {
                    return newNode(data);  // Eğer kök boşsa, yeni node ekle
                }
                if (data < root.data)
                {
                    root.left = insert(root.left, data);  // Sol alt ağaca ekle
                }
                else
                {
                    root.right = insert(root.right, data);  // Sağ alt ağaca ekle
                }
                return root;
            }
            public void preOrder(Node root, System.Windows.Forms.TextBox textBox4)
            {
                if (root != null)
                {
                    textBox4.AppendText(root.data + " ");
                    preOrder(root.left, textBox4);
                    preOrder(root.right, textBox4);
                }
            }
            public void inOrder(Node root, System.Windows.Forms.TextBox textBox5)
            {
                if (root != null)
                {
                    inOrder(root.left, textBox5);
                    textBox5.AppendText(root.data + " ");
                    inOrder(root.right, textBox5);
                }

            }
            public void postOrder(Node root, System.Windows.Forms.TextBox textBox6)
            {
                if (root != null)
                {
                    postOrder(root.left, textBox6);
                    postOrder(root.right, textBox6);
                    textBox6.AppendText(root.data + " ");


                }
            }
            
            
            public int countLeaves(Node root)
            {
                if (root == null)
                {
                    return 0;  // Eğer düğüm null ise, yaprak sayısı 0
                }

                // Eğer düğümün sol ve sağ çocuğu null ise, bu bir yaprak düğümdür
                if (root.left == null && root.right == null)
                {
                    return 1;  // Yaprak düğüm sayısını 1 artır
                }

                // Sol ve sağ alt ağacı gezerek toplam yaprak sayısını hesapla
                return countLeaves(root.left) + countLeaves(root.right);
            }
            public Node minValueNode(Node node)
            {
                Node current = node;

                // En küçük elemanı bulana kadar sol çocukları gez
                while (current != null && current.left != null)
                {
                    current = current.left;
                }
                return current;
            }

            // Düğüm silme metodu
            public Node deleteNode(Node root, int key)
            {
                if (root == null)
                {
                    return root;  // Eğer kök null ise, hiçbir şey yapma
                }

                // Silinecek düğüm, kökün solunda mı, sağında mı olduğunu kontrol et
                if (key < root.data)
                {
                    root.left = deleteNode(root.left, key);  // Sol alt ağaçta ara
                }
                else if (key > root.data)
                {
                    root.right = deleteNode(root.right, key);  // Sağ alt ağaçta ara
                }
                else
                {
                    // Silinecek düğüm bulundu
                    // 1. Durum: Düğümün hiç çocuğu yoksa (yaprak düğüm)
                    if (root.left == null && root.right == null)
                    {
                        return null;  // Düğümü sil
                    }
                    // 2. Durum: Düğümün bir çocuğu varsa
                    else if (root.left == null)
                    {
                        return root.right;  // Düğümün yerine sağ çocuğunu koy
                    }
                    else if (root.right == null)
                    {
                        return root.left;  // Düğümün yerine sol çocuğunu koy
                    }
                    // 3. Durum: Düğümün iki çocuğu varsa
                    else
                    {
                        // Sağ alt ağacın en küçük elemanını al
                        Node temp = minValueNode(root.right);

                        // Düğümün verisini, sağ alt ağacın en küçük elemanı ile değiştir
                        root.data = temp.data;

                        // Sağ alt ağacın en küçük elemanını sil
                        root.right = deleteNode(root.right, temp.data);
                    }
                }
                return root;
            }
            public void setNodeCoordinates(Node root, int offsetX, int offsetY)
            {
                if (root != null)
                {
                    root.x = offsetX;
                    root.y = offsetY;

                    // Sol ve sağ çocukların koordinatlarını ayarlama
                    if (root.left != null)
                    {
                        root.left.x = root.x - 1;
                        root.left.y = root.y + 1;
                        setNodeCoordinates(root.left, root.left.x, root.left.y);  // Sol çocuğun koordinatlarını ayarla
                    }

                    if (root.right != null)
                    {
                        root.right.x = root.x + 1;
                        root.right.y = root.y + 1;
                        setNodeCoordinates(root.right, root.right.x, root.right.y);  // Sağ çocuğun koordinatlarını ayarla
                    }
                }
            }
            
            


        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(textBox1.Text);  // Kullanıcıdan alınan değer
            int s1 = Convert.ToInt32(textBox1.Text);  // User input value
            tree.root = tree.insert(tree.root, s1);   // Insert the node

            textBox1.Text = ("Eklendi: " + s1);       // Display "Eklendi" message

            // Optionally, show the updated tree structure
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int s1 = Convert.ToInt32(textBox2.Text);  // Kullanıcıdan alınan silinecek düğüm verisi
            tree.root = tree.deleteNode(tree.root, s1);  // Düğümü ağacından sil

            textBox2.Text = ("Silindi: " + s1);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            // Ağacı gezmeden önce root'un null olmadığından emin ol
            if (tree.root != null)
            {
                tree.preOrder(tree.root, textBox4);
                tree.inOrder(tree.root, textBox5);
                tree.postOrder(tree.root, textBox6);
            }
            else
            {
                textBox4.AppendText("Ağaç boş.\n");
                textBox5.AppendText("Ağaç boş.\n");
                textBox6.AppendText("Ağaç boş.\n");
            }
            int totalNodes = tree.countNodes(tree.root);  // Toplam düğüm sayısını al
            textBox7.Text = "Toplam Düğüm Sayısı: " + totalNodes.ToString();
            int height = tree.getHeight(tree.root);  // Ağaç yüksekliğini al
            textBox8.Text = "Ağaç Yüksekliği: " + height.ToString();
            int leafCount = tree.countLeaves(tree.root);  // Yaprak sayısını al
            textBox9.Text = "Yaprak Sayısı: " + leafCount.ToString();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Invalidate();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
