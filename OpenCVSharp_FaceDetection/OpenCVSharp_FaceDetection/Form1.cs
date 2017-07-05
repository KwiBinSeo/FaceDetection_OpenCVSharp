using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace OpenCVSharp_FaceDetection
{
    public partial class Form1 : Form
    {
        private IplImage img, img2;
        private IplImage smallImg, gray;
        private CvHaarClassifierCascade cascade;
        private CvMemStorage storage;
        private Bitmap bitmap;
        private int width, height;
        private static int num1 = 100;
        private static int num2 = 20;
        private string imageFile;
        private string cascadeFile;
        private CvColor[] colors = new CvColor[]{
                new CvColor(0,0,255),
                new CvColor(0,128,255),
                new CvColor(0,255,255),
                new CvColor(0,255,0),
                new CvColor(255,128,0),
                new CvColor(255,255,0),
                new CvColor(255,0,0),
                new CvColor(255,0,255),
            };
        const double Scale = 1.14;
        const double ScaleFactor = 1.0850;
        const int MinNeighbors = 2;

        public Form1()
        {
            InitializeComponent();
            label1.Text = "0";
            imageFile = "C://face.jpg";
            cascadeFile = "haarcascade_frontalface_alt.xml";
            label1.Text = "Max : " + trackBar1.Value.ToString();
            label2.Text = "Min : " + trackBar2.Value.ToString();
            FaceDetection();
        }

        // 트랙바 조절_CvSize Max값 조절
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = "Max : " + trackBar1.Value.ToString();
            num1 = trackBar1.Value;
            FaceDetection();
        }

        // CvSize Min값 조절
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = "Min : " + trackBar2.Value.ToString();
            num2 = trackBar2.Value;
            FaceDetection();
        }

        private void FaceDetection()
        {
           
            //CvSize minSize = new CvSize(20, 20);
            //CvSize maxSize = new CvSize(100, 100);
            CvSize minSize = new CvSize(num2, num2);
            CvSize maxSize = new CvSize(num1, num1);

            bitmap = new Bitmap(imageFile); // Bitmap Image 객체 생성
            img = BitmapConverter.ToIplImage(bitmap); // Bitmap to IplImage

            //img = new IplImage(imageFile, LoadMode.Color);
            smallImg = new IplImage(new CvSize(Cv.Round(img.Width / Scale), Cv.Round(img.Height / Scale)), BitDepth.U8, 1);
            gray = new IplImage(img.Size, BitDepth.U8, 1);

            Cv.CvtColor(img, gray, ColorConversion.BgrToGray);
            Cv.Resize(gray, smallImg, Interpolation.Linear);
            Cv.EqualizeHist(smallImg, smallImg);
            //pictureBox1.Image = BitmapConverter.ToBitmap(gray);

            cascade = CvHaarClassifierCascade.FromFile(cascadeFile);
            storage = new CvMemStorage();

            storage.Clear();

            CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(smallImg, cascade, storage, ScaleFactor, MinNeighbors, 0, minSize, maxSize);

            for (int i = 0; i < faces.Total; i++)
            {
                CvRect r = faces[i].Value.Rect;
                CvPoint center = new CvPoint
                {
                    X = Cv.Round((r.X + r.Width * 0.5) * Scale),
                    Y = Cv.Round((r.Y + r.Height * 0.5) * Scale)
                };
                int radius = Cv.Round((r.Width + r.Height) * 0.25 * Scale);
                img.Circle(center, radius, colors[i % 8], 3, LineType.AntiAlias, 0);
            }

            width = pictureBox1.Width;
            height = pictureBox1.Height;

            img2 = Cv.CreateImage(Cv.Size(width, height), BitDepth.U8, 3);
            Cv.Resize(img, img2);
            // 이미지 출력
            pictureBox1.Image = BitmapConverter.ToBitmap(img2);
            //CvWindow.ShowImages(img);
        }

        
    }
}
