using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace WPM_v3
{
    public partial class Form1 : Form
    {

        private ListViewItem[] myCache; //array to cache items for the virtual list
        private int firstItem; //stores the index of the first item in the cache
        //private string[][] tags;

        FileInfo[] Dirs_all;
        List<string> Dirs_imgs = new List<string>();
        List<Wallpaper> wallpaper = new List<Wallpaper>();

        private string all_tags = "";

        //private Dictionary<string, int> _mpstriimage = new Dictionary<string, int>();

        public Form1()
        {
            InitializeComponent();

            listView1.LargeImageList = new ImageList();
            listView1.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
            listView1.LargeImageList.ImageSize = TextRenderer.MeasureText("10000", Font);

            listView1.SmallImageList = listView1.LargeImageList;

            wallpaper.Add(new Wallpaper { Wallpaper_filename = "Tags", Tags = "placeholder" });

            getDirs();
                
        }

        //TODO use this to draw thumbnails
        /*
        private ListViewItem _LviItemForIndex(int i)
        {
             int iimage;
             string strName = i.ToString();
             ImageList.ImageCollection imgc =  listView1.LargeImageList.Images;

             // Don't call the ImageList key-lookup methods, because they are
             // slow!  Just maintain the key-to-image mapping ourselves.
             if (!_mpstriimage.TryGetValue(strName, out iimage))
             {
                 iimage = imgc.Count;

                 imgc.Add(_ImageForIndex(i));

                 _mpstriimage.Add(strName, iimage);
              }

             return new ListViewItem(strName, iimage);
        }

        private Image _ImageForIndex(int i)
        {
            Image imageRet =
                new Bitmap(listView1.LargeImageList.ImageSize.Width,
                listView1.LargeImageList.ImageSize.Height,
                PixelFormat.Format24bppRgb);

            using (Graphics gfx = Graphics.FromImage(imageRet))
            {
                gfx.Clear(this.ForeColor);

                TextRenderer.DrawText(gfx, i.ToString(), Font,
                    new Rectangle(new Point(), imageRet.Size),
                    this.BackColor,
                    TextFormatFlags.HorizontalCenter);
            }

            return imageRet;
        }*/

        private void SaveCSV(string nazwaPliku)
        {
            StreamWriter sw = File.CreateText(nazwaPliku);

            foreach (var o in wallpaper)
            {
                sw.WriteLine(o.ToCSV());
            }

            sw.Close();
        }

        private void LoadCSV(string nazwaPliku)
        {
            StreamReader sr = File.OpenText(nazwaPliku);
            string line;

            wallpaper = new List<Wallpaper>();

            while ((line = sr.ReadLine()) != null)
            {
                if (String.IsNullOrWhiteSpace(line)) break;
                wallpaper.Add(Wallpaper.FromCSV(line));
            }

            /*if ((line = sr.ReadLine()) != null && !String.IsNullOrWhiteSpace(line))
            {
                    wallpaper.Add(Wallpaper.FromCSV(line));
            }*/

            sr.Close();
        }

        private void ListboxTagsUpdate()
        {
            listView2.Items.Clear();

            foreach (var o in wallpaper)
            {
                
                listView2.Items.Add(o.Tags);
            }
        }

        private void getDirs()
        {
            Dirs_imgs.Clear();
            ClearCache();
            int counter = 0;
            if (textBox1.Text != "")
            {
                DirectoryInfo directory = new DirectoryInfo(textBox1.Text);

                Dirs_all = directory.GetFiles();

                
                foreach (var dir in Dirs_all)
                {

                    if (dir.Extension == ".jpg")
                    {
                        Dirs_imgs.Add(dir.Name);
                        counter++;
                    }
                    else if (dir.Extension == ".png")
                    {
                        Dirs_imgs.Add(dir.Name);
                        counter++;
                    }
                }
                
            }
            ///
            label2.Text = "Plikow: " + Dirs_all.Length.ToString();
            label1.Text = "Obrazkow: " + Dirs_imgs.Count.ToString();
            label3.Text = "Counter: " + counter.ToString();
            ///
            listView1.VirtualListSize = counter;

        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // If we have the item cached, return it. Otherwise, recreate it.
            if (myCache != null &&
                e.ItemIndex >= firstItem &&
                e.ItemIndex < firstItem + myCache.Length)
            {
                e.Item = myCache[e.ItemIndex - firstItem];
            }
            else
            {
                e.Item = GetListItem(e.ItemIndex);
                //e.Item = _LviItemForIndex(e.ItemIndex);
            }
        }

        private void listView1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            // Only recreate the cache if we need to.
            if (myCache != null &&
                e.StartIndex >= firstItem &&
                e.EndIndex <= firstItem + myCache.Length)
                return;

            firstItem = e.StartIndex;
            int length = e.EndIndex - e.StartIndex + 1;

            myCache = new ListViewItem[length];
            for (int i = 0; i < myCache.Length; i++)
            {
               myCache[i] = GetListItem(firstItem + i);
               //myCache[i] = _LviItemForIndex(firstItem + i);
            }
        }

        private ListViewItem GetListItem(int i)
        {
            //DateTime itemTime = firstItem + TimeSpan.FromDays(i);

            ListViewItem lvi = new ListViewItem(Dirs_imgs[i]);

            //lvi.Tag = itemTime;

            return lvi;
        }

        private void ClearCache()
        {
            myCache = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                getDirs();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedIndices.Count > 0)
            {
                pictureBox1.ImageLocation = textBox1.Text + "\\" + listView1.Items[listView1.SelectedIndices[0]].Text;
                label4.Text = textBox1.Text + "\\" + listView1.Items[listView1.SelectedIndices[0]].Text;
                
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {

                for (int i = 0; i < listView1.SelectedIndices.Count; i++)
                {
                    System.IO.File.Copy(textBox1.Text + "\\" + listView1.Items[listView1.SelectedIndices[i]].Text, textBox2.Text + "\\" + listView1.Items[listView1.SelectedIndices[i]].Text);
                }

            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                System.Diagnostics.Process.Start(textBox1.Text + "\\" + listView1.Items[listView1.SelectedIndices[0]].Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                for (int i = 0; i < listView1.SelectedIndices.Count; i++)
                {
                    //string[] tekst = new string[5];
                    //tekst = textBox3.Text.Split(',');

                    string tekst = textBox3.Text;
                    
                    {
                        wallpaper.Add(
                            new Wallpaper
                            {
                                Wallpaper_filename = listView1.Items[listView1.SelectedIndices[i]].Text,
                                Tags = tekst
                                /*Tag2 = tekst[1],
                                Tag3 = tekst[2],
                                Tag4 = tekst[3],
                                Tag5 = tekst[4],*/
                            }
                        );
                    };
                    
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveCSV(textBox1.Text + "\\tags_db");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Pliki csv (*.csv)|*.csv|Wszystkie pliki (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadCSV(openFileDialog1.FileName);
                ListboxTagsUpdate();
            }
        }
    }
}
