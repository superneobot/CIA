using Compress_Image_Archiver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Archiver : Form
    {
        public List<Files> files;
        public List<Image> images;

        public Archiver()
        {
            InitializeComponent();
            files = new List<Files>();
            images = new List<Image>();
        }

        public async Task<List<Files>> LoadFiles() //метод для загрузки файлов
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Multiselect = true;
                op.Filter = "JPEG file (*.jpg)|*.jpg|PNG file (*.png)|*.png|GIF file (*.gif)|*.gif";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    int file = 1;
                    await Task.Run(() =>
                     {
                         for (int i = 0; i < op.FileNames.Length; i++)
                         {
                             var img = Image.FromFile(op.FileNames[i]);
                             Invoke((Action)delegate { status.Text = "Adding file - " + Path.GetFileName(op.FileNames[i]); });
                             files.Add(new Files(file, Path.GetFileName(op.FileNames[i]), Path.GetFullPath(op.FileNames[i]), img, op.FileNames[i]));
                             file++;
                         }
                     });
                }
            }

            return files;
        }

        public void CreatePack(string path, List<Files> data)
        {
            IFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                bf.Serialize(fs, data);
            }
        }

        public void CreateArchive(string path, List<Files> data)
        {
            IFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress))
                {
                    bf.Serialize(gz, data);
                }
            }
        }

        private async void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LoadFiles();
            await Task.Run(() =>
             {
                 int i = 0;
                 foreach (var item in files)
                 {
                     Invoke((Action)delegate { Large.Images.Add(item.Image); });
                     Invoke((Action)delegate
                     {
                         Large.ImageSize = new Size(140, 160);
                         //
                         ListViewItem lvi = new ListViewItem(new string[] { $"{item.Count}", item.FilePath, item.Size });
                         status.Text = "Loading";
                         lvi.ImageIndex = i;
                         LV.Items.Add(lvi);
                     });
                     i++;
                 }
             });
            status.Text = "Done";
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var path = Application.StartupPath + @"/Data/";
            //var file = path + @"123.gzip";
            //DirectoryInfo dir = Directory.CreateDirectory(path);
            if (files.Count != 0)
            {
                using (SaveFileDialog sf = new SaveFileDialog())
                {
                    sf.FileName = Path.GetFileNameWithoutExtension(files[0].FileName);
                    sf.Filter = "Archive file (*.gzip)|*.gzip|CIA file (*.cia)|*.cia";
                    if (sf.ShowDialog() == DialogResult.OK)
                    {
                        if (sf.FilterIndex == 1)
                        {
                            CreateArchive(sf.FileName, files);
                        }
                        if (sf.FilterIndex == 2)
                        {
                            CreatePack(sf.FileName, files);
                        }

                    }

                }
            }

        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            files.Clear();
            images.Clear();
            Large.Images.Clear();
            LV.Clear();
        }

    }
    public class MListView : ListView
    {
        public MListView()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }

}

