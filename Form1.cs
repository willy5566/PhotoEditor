using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoEditor
{
    public partial class Form1 : Form
    {
        Queue<string> qImgPath = new Queue<string>();
        Queue<Image> qImages = new Queue<Image>();
        Queue<string> qImgSavePath = new Queue<string>();
        Queue<Image> qImageSave = new Queue<Image>();
        ManualResetEvent mreLoad = new ManualResetEvent(false);
        ManualResetEvent mreShow = new ManualResetEvent(false);

        Image imgShow;
        int nRotMode = 0;
        Rectangle[] rFocusRegions;
        Rectangle rFocusRegion;
        Rectangle rShowRegion;

        public Form1()
        {
            InitializeComponent();
        }

        bool bFrtLoad = false;

        private void IniObject()
        {
            qImgPath.Clear();
            qImgSavePath.Clear();
            while (qImages.Count>0)
            {
                var img = qImages.Dequeue();
                img.Dispose();
            }
            while (qImageSave.Count > 0)
            {
                var img = qImages.Dequeue();
                img.Dispose();
            }
            rFocusRegions = new Rectangle[2];
            rFocusRegion = new Rectangle();
            rShowRegion = new Rectangle();
            mreLoad.Set();
            mreShow.Set();
        }

        private void thdImgLoad_DoWork()
        {
            while (qImgPath.Count > 0)
            {
                if (qImages.Count > 3)
                {
                    mreLoad.Reset();
                    mreLoad.WaitOne();
                }
                string path = qImgPath.Dequeue();
                Bitmap bmp = new Bitmap(path);
                qImages.Enqueue(bmp);
                qImgSavePath.Enqueue(path);
                if (bFrtLoad)
                {
                    mreShow.Set();
                    bFrtLoad = false;
                }
            }
        }

        private void ShowImage()
        {          
            pnlImgShow.Invoke(new Action(() =>
            {
                if (imgShow.Width >= imgShow.Height)
                {
                    //pbImgShow.Width = 900;
                    //pbImgShow.Height = 675;
                    //pbImgShow.Location = new Point(0, (pbImgShow.Width - pbImgShow.Height));

                    //pnlImgShow.Width = 902;
                    //pnlImgShow.Height = 677;

                    //this.Width = 940;
                    //this.Height = 790;
                }
                else
                {
                    //pbImgShow.Width = 675;
                    //pbImgShow.Height = 900;
                    //pbImgShow.Location = new Point((pbImgShow.Height - pbImgShow.Width), 0);

                    //pnlImgShow.Width = 677;
                    //pnlImgShow.Height = 902;

                    //this.Width = 715;
                    //this.Height = 1015;
                }
                //pbImgShow.Width = imgShow.Width;
                //pbImgShow.Height = imgShow.Height;
                pbImgShowTranslation();
                UpdateFocusRegion();
                pbImgShow.Invalidate();
            })
                        );
        }
        private void thdImgShow_DoWork()
        {
            while (true)
            {
                if (qImages.Count > 0)
                {
                    imgShow = qImages.Dequeue();
                    ShowImage();
                    mreLoad.Set();
                }
                mreShow.Reset();
                mreShow.WaitOne();
            }
        }

        private void SetRotateMode(int nMode)
        {
            if (nMode < 0)
                nMode += 4;
            nMode %= 4;
            switch (nMode)
            {
                case 1:
                    
                    break;

            }
        }

        int nCutMode = 0; // 0:切寬；1:切高
        float fRate;
        private void pbImgShow_Paint(object sender, PaintEventArgs e)
        {
            if (imgShow != null)
            {
                e.Graphics.DrawImage(imgShow, rShowRegion);
                Pen pen = new Pen(Brushes.Blue, 1);
                e.Graphics.DrawRectangle(pen, rFocusRegion);

                SolidBrush b = new SolidBrush(Color.FromArgb(100, 0, 255, 255));
                e.Graphics.FillRectangles(b, rFocusRegions);               
            }
        }

        private void pbImgShowTranslation()
        {
            if (imgShow != null)
            {
                float fWRate = (float)imgShow.Width / pnlImgShow.Width;
                float fHRate = (float)imgShow.Height / pnlImgShow.Height;
                fRate = fWRate > fHRate ? fWRate : fHRate;


                if (imgShow.Width < imgShow.Height)
                {
                    if (imgShow.Height / 3 > imgShow.Width / 2)
                        nCutMode = 1;
                    else
                        nCutMode = 0;
                }
                else
                {
                    if (imgShow.Width / 3 > imgShow.Height / 2)
                        nCutMode = 0;
                    else
                        nCutMode = 1;
                }
                if (nCutMode == 1)
                {
                    int nimgShowW = (int)(imgShow.Width / fRate);
                    int nimgShowH = imgShow.Width >= imgShow.Height ? (int)(nimgShowW / 1.5) : (int)(nimgShowW * 1.5);
                    rFocusRegion.Size = new Size(nimgShowW, nimgShowH);

                }
                else
                {
                    int nimgShowH = (int)(imgShow.Height / fRate);
                    int nimgShowW = imgShow.Width < imgShow.Height ? (int)(nimgShowH / 1.5) : (int)(nimgShowH * 1.5);
                    rFocusRegion.Size = new Size(nimgShowW, nimgShowH);
                }

                rShowRegion.Size = new Size((int)(imgShow.Width / fRate), (int)(imgShow.Height / fRate));

                pbImgShow.Location = new Point((900 - rShowRegion.Width) / 2, (900 - rShowRegion.Height) / 2);
                pbImgShow.Size = rShowRegion.Size;
            }
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;
            Image imgPass = imgShow;
            imgShow = null;
            imgPass.Dispose();
            imgPass = null;

            mreShow.Set();

            rFocusRegion = new Rectangle();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            open.Filter = "Image files (*.jpg)|*.jpg";
            if (open.ShowDialog() != DialogResult.OK)
                return;

            IniObject();

            this.Text = open.SafeFileNames.Length.ToString();
            bFrtLoad = true;

            foreach (var path in open.FileNames)
            {
                qImgPath.Enqueue(path);
            }

            Thread thdImgLoad = new Thread(new ThreadStart(thdImgLoad_DoWork));
            thdImgLoad.IsBackground = true;
            thdImgLoad.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thdImgShow = new Thread(new ThreadStart(thdImgShow_DoWork));
            thdImgShow.IsBackground = true;
            thdImgShow.Start();
        }

        bool bMouseDown = false;
        Point plastClick;
        Point pFocusRegionXY;
        private void pbImgShow_MouseDown(object sender, MouseEventArgs e)
        {
            bMouseDown = rFocusRegion.Contains(e.Location) && e.Button == MouseButtons.Left;
            if (bMouseDown)
            {
                plastClick = e.Location;
                pFocusRegionXY = rFocusRegion.Location;
            }
        }

        private void pbImgShow_MouseUp(object sender, MouseEventArgs e)
        {
            bMouseDown = false;
        }

        private void pbImgShow_MouseMove(object sender, MouseEventArgs e)
        {
            if (bMouseDown)
            {
                if (nCutMode == 0)
                {
                    rFocusRegion.X = pFocusRegionXY.X + e.X - plastClick.X;

                    if (rFocusRegion.X < 0)
                        rFocusRegion.X = 0;
                    if (rFocusRegion.Right >= pbImgShow.Width)
                        rFocusRegion.X = pbImgShow.Width - 1 - rFocusRegion.Width;                   
                }
                else
                {
                    rFocusRegion.Y = pFocusRegionXY.Y + e.Y - plastClick.Y;

                    if (rFocusRegion.Y < 0)
                        rFocusRegion.Y = 0;
                    if (rFocusRegion.Bottom >= pbImgShow.Height)
                        rFocusRegion.Y = pbImgShow.Height - 1 - rFocusRegion.Height;                
                }
                UpdateFocusRegion();
                pbImgShow.Invalidate();
            }
        }

        private void UpdateFocusRegion()
        {
            if (nCutMode == 0)
            {
                rFocusRegions[0] = new Rectangle(
                        0, 0, rFocusRegion.X, rFocusRegion.Height);
                rFocusRegions[1] = new Rectangle(
                        rFocusRegion.Right, 0, pbImgShow.Width - rFocusRegion.Right, rFocusRegion.Height);
            }
            else
            {
                rFocusRegions[0] = new Rectangle(
                        0, 0, rFocusRegion.Width, rFocusRegion.Y);
                rFocusRegions[1] = new Rectangle(
                        0, rFocusRegion.Bottom, rFocusRegion.Width, pbImgShow.Height - rFocusRegion.Bottom);
            }
        }

        //int nSaveCnt = 0;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;

            Image imgPass = imgShow;
            imgShow = null;
            string path = qImgSavePath.Dequeue();

            FileInfo f = new FileInfo(path);
            string sCreationTime = f.CreationTime.ToString();

            string sNewName = "_" + System.IO.Path.GetFileName(path);
            string sNewPath = System.IO.Path.GetDirectoryName(path) + @"\4X6\";
            if (!System.IO.Directory.Exists(sNewPath))
                System.IO.Directory.CreateDirectory(sNewPath);
            RectangleF rSave = new RectangleF(
                fRate * rFocusRegion.X,
                fRate * rFocusRegion.Y,
                fRate * rFocusRegion.Width,
                fRate * rFocusRegion.Height);

            //nSaveCnt++;
            Task.Run(new Action(() =>
            {
                Bitmap bmpSave = new Bitmap((int)rSave.Width, (int)rSave.Height);
                var g = Graphics.FromImage(bmpSave);
                g.DrawImage(imgPass, new RectangleF(0,0, rSave.Width, rSave.Height), rSave, GraphicsUnit.Pixel);

                if (ckbDrawTime.Checked) g.DrawString(sCreationTime, new Font("Ariel", 120, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Red, new Point(10, 10));
                bmpSave.Save(sNewPath + sNewName, System.Drawing.Imaging.ImageFormat.Jpeg);
                g.Dispose();
                bmpSave.Dispose();

                imgPass.Dispose();
                imgPass = null;
            }));
            

            mreShow.Set();

            rFocusRegion = new Rectangle();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.A:
                    btnPass.PerformClick();
                    break;
                case Keys.S:
                    btnSave.PerformClick();
                    break;
                case Keys.Q:
                    btnL90.PerformClick();
                    break;
                case Keys.E:
                    btnR90.PerformClick();
                    break;
            }
        }

        private void btnR90_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;

            rFocusRegion.X = 0;
            rFocusRegion.Y = 0;
            imgShow.RotateFlip(RotateFlipType.Rotate90FlipNone);
            
            ShowImage();
        }

        private void btnL90_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;

            rFocusRegion.X = 0;
            rFocusRegion.Y = 0;
            imgShow.RotateFlip(RotateFlipType.Rotate270FlipNone);
            ShowImage();
        }
    }
}
