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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AndroidResourcesCreator.Properties;

namespace AndroidResourcesCreator
{
    public partial class Form1 : Form
    {
        struct ImageSize
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


        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fileName = getImagePath();
            progressBar1.Value = 0;
            progressBar1.Maximum = DrawerSizes.Count-1;
            for (var i = 0; i < DrawerSizes.Count; i++)
            {
                saveImage(DrawerSizes[i], fileName);
                progressBar1.Value = i;
            }

        }

        private void saveImage(ImageSize imageSize,string path)
        {

            var currentPath = Path.GetDirectoryName(path) + "/" + imageSize.FolderName;
            Directory.CreateDirectory(currentPath);
            var fileName = currentPath  +"/" + Path.GetFileNameWithoutExtension(path);
            var ms = new MemoryStream(File.ReadAllBytes(path)); 
            var image = Image.FromStream(ms);
            var fixSize = ResizeImageFixedWidth(image, imageSize);
            fixSize.Save(fileName+".jpg", ImageFormat.Jpeg);

        }

        private Image ResizeImageFixedWidth(Image imgToResize, ImageSize imageSize)
        {
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            var nPercent = ((float)imageSize.Width / (float)sourceWidth);

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

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


       

        private string getImagePath()
        {
            var theDialog = new OpenFileDialog
            {
                Title = Resources.Open_Images_File,
                Filter = Resources.Click_All_files
            };
            var lastPath = (string)Settings.Default["lastPath"];
            theDialog.InitialDirectory = lastPath;
            if (theDialog.ShowDialog() != DialogResult.OK) return null;
            var directoryPath = Path.GetDirectoryName(theDialog.FileName);
            Settings.Default["lastPath"] = directoryPath;
            Settings.Default.Save();
            try
            {
                    
                return theDialog.FileName ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.
                    Click_Error + ex.Message);

            }
            return null;
        }
    }
}
