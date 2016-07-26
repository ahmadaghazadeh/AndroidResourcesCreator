using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AndroidResourcesCreator.Properties;

namespace AndroidResourcesCreator
{
    public partial class Form1 : Form
    {
          DataTable dbApi;
          
        public struct ImageSize
        {
            private readonly int width;
            private readonly int height;
            private readonly string folderName;
            public ImageSize(int width, int height,string folderName)
            {
                this.width = width;
                this.height = height;
                this.folderName = folderName;
            }
            public int Width { get { return width; } }
            public int Height { get { return height; } }
            public string FolderName { get { return folderName; } }
        }

        static readonly IList<ImageSize> DrawerSizes = new ReadOnlyCollection<ImageSize>
        (new[] {
             new ImageSize (384 , 216 ,"drawable-mdpi"),
             new ImageSize (576 , 324 ,"drawable-hdpi"),
             new ImageSize (768 , 432 ,"drawable-xhdpi"),
             new ImageSize (1152  , 648  ,"drawable-xxhdpi")
        });

        static readonly IList<ImageSize> IconSizes = new ReadOnlyCollection<ImageSize>
       (new[] {
             new ImageSize (48   , 48 ,"drawable-mdpi"),
             new ImageSize (72   , 72 ,"drawable-hdpi"),
             new ImageSize (96   , 96 ,"drawable-xhdpi"),
             new ImageSize (144  , 144  ,"drawable-xxhdpi"),
             new ImageSize (192  , 192  ,"drawable-xxxhdpi")
       });


        public Form1()
        {
             
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fileName = Project.getImagePath();
            progressBar1.Value = 0;
            progressBar1.Maximum = DrawerSizes.Count-1;
            for (var i = 0; i < DrawerSizes.Count; i++)
            {
                saveImage(DrawerSizes[i], fileName,true, ImageFormat.Jpeg);
                progressBar1.Value = i;
            }

        }

        private void saveImage(ImageSize imageSize,string path,Boolean isResizeFixedWith, ImageFormat imageFormat)
        {

            var currentPath = Path.GetDirectoryName(path) + "/" + imageSize.FolderName;
            Directory.CreateDirectory(currentPath);
            var fileName = currentPath  +"/" + Path.GetFileNameWithoutExtension(path);
            var ms = new MemoryStream(File.ReadAllBytes(path)); 
            var image = Image.FromStream(ms);
            if (isResizeFixedWith)
            {
                var fixSize = ResizeImageFixedWidth(image, imageSize);
                fixSize.Save(fileName + ".jpg", imageFormat);
            }
            else
            {
                var qualityParam1 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                EncoderParameters encoderParams = new EncoderParameters(1);
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/"+imageFormat.ToString().ToLower());
                encoderParams.Param[0] = qualityParam1;
                ResizeImage(image, imageSize).Save(fileName + ".png", jpegCodec, encoderParams);
                Project.CompressTinyPng(fileName + ".png",dbApi);
            }
            

        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        private Image ResizeImageFixedWidth(Image imgToResize, ImageSize imageSize)
        {
           
            var b = new Bitmap(imageSize.Width, imageSize.Height);
        
            using (Graphics gr = Graphics.FromImage((Image)b))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(imgToResize, new Rectangle(0, 0, imageSize.Width, imageSize.Height));
            }
            return (Image)b;
        }

        private Image ResizeImage(Image imgToResize, ImageSize imageSize)
        {
            var b = new Bitmap(imageSize.Width, imageSize.Height);
             
           
            using (Graphics gr = Graphics.FromImage((Image)b))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.Bicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(imgToResize, new Rectangle(0, 0, imageSize.Width, imageSize.Height));
            }
            return (Image)b;
        }


       

        private void Form1_Load(object sender, EventArgs e)
        {
            dbApi = Project.ReadCSV(Directory.GetCurrentDirectory() + "/TinyApiKey.csv");
            dataGridView1.DataSource = dbApi;
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileName = Project.getImagePath();
            progressBar1.Value = 0;
            progressBar1.Maximum = IconSizes.Count - 1;
            for (var i = 0; i < IconSizes.Count; i++)
            {
                saveImage(IconSizes[i], fileName,false, ImageFormat.Png);
                progressBar1.Value = i;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var fileName = Project.getImagePath();
            progressBar1.Value = 0;
            progressBar1.Maximum = IconSizes.Count - 1;
            for (var i = 0; i < IconSizes.Count; i++)
            {
                saveImage(IconSizes[i], fileName, false, ImageFormat.Png);
                progressBar1.Value = i;
            }
        }

       

        
    }

}
