using System;
using System.IO;
using System.Xml.Serialization;
using Image = System.Drawing.Image;

namespace Compress_Image_Archiver
{
    [Serializable]
    public class Files
    {
        [XmlAttribute("Count")]
        public int Count { get; set; }
        [XmlAttribute("File Name")]
        public string FileName { get; set; }
        [XmlAttribute("File Path")]
        public string FilePath { get; set; }
        [XmlAttribute("Image")]
        public Image Image { get; set; }
        [XmlAttribute("Size")]
        public string Size { get; set; }


        public Files(int _count, string _filename, string _filepath, Image _image, string _size)
        {
            Count = _count;
            FileName = _filename;
            FilePath = _filepath;
            Image = _image;
            var l = new FileInfo(_filepath).Length;
            Size = BytesToString(l);
        }

        static String BytesToString(long byteCount)
        {
            string[] suf = { " Байт", " Кб", " Мб", " Гб", " Тб", " Пб", " Еб" }; //
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

    }
}
