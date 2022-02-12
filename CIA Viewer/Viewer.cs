using Compress_Image_Archiver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace CIA_Viewer
{
    public partial class Viewer : Form
    {
        public List<Files> files;
        public List<Image> images;

        public Viewer()
        {
            files = new List<Files>();
            images = new List<Image>();

            InitializeComponent();
        }

        public Viewer(string path)
        {
            InitializeComponent();

            images = new List<Image>();
            files = new List<Files>();

            OpenWith(path);
        }

        private async void OpenWith(string path)
        {
            PictureBox b = new PictureBox();
            b.Image = Properties.Resources.loading;
            b.Size = new Size(198, 198);
            b.Location = new Point((this.Width / 4) + 150, this.Height / 4);
            LV.Controls.Add(b);
            await Task.Run(() =>
            {
                if (path.Contains(".cia"))
                {
                    List<Files> result = null;
                    if (ReadPack(path, out result, Path.GetFileName(path)))
                    {
                        files = result as List<Files>;
                        Action a = () =>
                        {
                            status.Text = "Готов";
                            Text = $"CIA Viewer - [{path}]";
                        };
                        Invoke(a);
                    }
                }
                if (path.Contains(".gzip"))
                {
                    List<Files> result = null;
                    if (ReadArchive(path, out result))
                    {
                        files = result as List<Files>;
                        Action a = () =>
                        {
                            status.Text = "Готов";
                            Text = $"CIA Viewer - [{path}]";
                        };
                        Invoke(a);
                    }
                }
            });
            b.Hide();
            LoadAndShow();
        }

        public bool ReadArchive(string path, out List<Files> data)
        {
            data = null;
            if (System.IO.File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (GZipStream gz = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        try
                        {
                            data = (List<Files>)formatter.Deserialize(gz);
                        }
                        catch (Exception)
                        {
                            Invoke((Action)delegate { MessageBox.Show(this, $"Неверный формат входного файла!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); });
                        }
                    }
                }
            }
            return data != null;
        }

        public bool ReadPack(string path, out List<Files> data, string file)
        {
            data = null;
            if (System.IO.File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                   // Invoke((Action)delegate { status.Text = $"Загрузка файла {file}"; });                      
                    try
                    {
                        data = (List<Files>)formatter.Deserialize(stream);
                    }
                    catch (Exception)
                    {
                        Invoke((Action)delegate { MessageBox.Show(this, $"Неверный формат входного файла!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); });
                    }
                }
            }
            return data != null;
        }

        private async void open_strip_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Filter = "CIA файл (*.cia)|*.cia|Файл архива CIA (*.gzip)|*.gzip|Все файлы (*.*)|*.*";
                //op.DefaultExt = "All files (*.*)|*.*";
                op.Multiselect = true;
                if (op.ShowDialog() == DialogResult.OK)
                {
                    Clear();
                    PictureBox b = new PictureBox();
                    b.Image = Properties.Resources.loading;
                    b.Size = new Size(198, 198);
                    b.Location = new Point((this.Width / 4)+150, this.Height / 4);
                    LV.Controls.Add(b);
                    await Task.Run(() =>
                    {
                        if (op.FilterIndex == 2)
                        {
                            List<Files> result = null;
                            if (ReadArchive(op.FileName, out result))
                            {
                                files = result as List<Files>;
                                Action a = () =>
                                {
                                    status.Text = "Готов";
                                    Text = $"CIA Viewer - [{op.FileName}]";
                                };
                                Invoke(a);
                            }
                        }
                        if (op.FilterIndex == 1)
                        {
                            List<Files> result = null;
                            if (ReadPack(op.FileName, out result, Path.GetFileName(op.FileName)))
                            {
                                files = result as List<Files>;
                                Action a = () =>
                                 {
                                     status.Text = "Готов";
                                     Text = $"CIA Viewer - [{op.FileName}]";
                                 };
                                Invoke(a);
                            }
                        }
                        if (op.FilterIndex == 3)
                        {
                           if(op.FileName.Contains(".cia"))
                            {
                                List<Files> result = null;
                                if (ReadPack(op.FileName, out result, Path.GetFileName(op.FileName)))
                                {
                                    files = result as List<Files>;
                                    Action a = () =>
                                    {
                                        status.Text = "Готов";
                                        Text = $"CIA Viewer - [{op.FileName}]";
                                    };
                                    Invoke(a);
                                }
                            }
                            if (op.FileName.Contains(".gzip"))
                            {
                                List<Files> result = null;
                                if (ReadArchive(op.FileName, out result))
                                {
                                    files = result as List<Files>;
                                    Action a = () =>
                                    {
                                        status.Text = "Готов";
                                        Text = $"CIA Viewer - [{op.FileName}]";
                                    };
                                    Invoke(a);
                                }
                            }
                        }
                    });
                    b.Hide();
                }
            }                                
            if (files.Count > 0)
            {
              LoadAndShow();
            }
            
        }

        public async void LoadAndShow()
        {
            // progress.Maximum = arhive.Count;
            int a = 1;
            await Task.Run(() =>
            {
                for (int i = 0; i < files.Count; i++)
                {
                    Large.Images.Add(files[i].Image);
                    status.Text = $"Загружено {i} изображений";
                }
            });
            await Task.Run(() =>
            {
                for (int i = 0; i < files.Count; i++)
                {
                    ListViewItem lvitem = new ListViewItem(files[i].FileName, i);
                    Invoke((Action)delegate
                   {
                       LV.Items.Add(lvitem);
                       status.Text = $"{i}";
                   });
                    a++;
                };
            });
            status.Text = $"Готов";
        }

        private void clear_strip_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            files.Clear();
            images.Clear();
            Large.Images.Clear();
            LV.Items.Clear();
            Text = "CIA Viewer";
        }

        private void LV_DoubleClick(object sender, EventArgs e)
        {
            if (LV.SelectedIndices.Count >= 0)
            {
                var index = LV.Items.IndexOf(LV.SelectedItems[0]);
                Form view = new Form();
                view.StartPosition = FormStartPosition.CenterScreen;
                view.FormBorderStyle = FormBorderStyle.FixedSingle;
                view.MaximizeBox = false;
                view.MinimizeBox = false;
                PictureBox box = new PictureBox();
                box.Image = files[index].Image;
                box.Dock = DockStyle.Fill;
                box.SizeMode = PictureBoxSizeMode.StretchImage;
                Button bsave = new Button();
                bsave.FlatStyle = FlatStyle.Flat;
                bsave.FlatAppearance.BorderSize = 0;
                bsave.FlatAppearance.MouseOverBackColor = Color.Transparent;
                bsave.FlatAppearance.MouseDownBackColor = Color.Transparent;
                bsave.BackColor = Color.Transparent;
                bsave.Cursor = Cursors.Hand;
                bsave.Size = new Size(32, 32);
                bsave.Location = new Point(10, 10);
                bsave.Image = Properties.Resources.save;
                bsave.Click += delegate
                {
                    using (SaveFileDialog sf = new SaveFileDialog())
                    {
                        sf.Filter = "Файл изображения JPEG (*.jpg)|*.jpg|Все файлы (*.*)|*.*";
                        sf.FileName = files[index].FileName;
                        if (sf.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                box.Image.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            catch
                            {
                                MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                };
                box.Controls.Add(bsave);

                view.Text = $"{files[index].FileName} - Оригинальный размер({files[index].Image.Width}x{files[index].Image.Height})";
                view.Controls.Add(box);
                //  view.Size = new Size(files[index].Image.Width / 2, files[index].Image.Height / 2);
                if (files[index].Image.Width > 3000)
                {
                    view.Size = new Size(files[index].Image.Width / 4, files[index].Image.Height / 4);
                }
                else
                {
                    view.Size = new Size(files[index].Image.Width / 2, files[index].Image.Height/2);
                }
                
                view.Show();
            }
        }
    }
}
