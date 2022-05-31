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
        Queue<string> qImgPath = new Queue<string>(); // 圖片路徑列隊
        Queue<Image> qImages = new Queue<Image>(); // 圖片列隊
        Queue<string> qImgSavePath = new Queue<string>();  // 圖片路徑列隊存檔用
        Queue<Image> qImageSave = new Queue<Image>();
        ManualResetEvent mreLoad = new ManualResetEvent(false); // 讀檔鎖
        ManualResetEvent mreShow = new ManualResetEvent(false); // 秀圖鎖

        Image imgShow; // 顯示用
        int nRotMode = 0; // 旋轉模式
        Rectangle[] rFocusRegions; // 遮蔽區
        Rectangle rFocusRegion; // 切圖區
        Rectangle rShowRegion; // 秀圖區

        const int MAX_BOUND = 900; // UI秀圖最大範圍

        int nTotal = 0; // 總張數
        int nSave = 0; // 存圖數
        int nPass = 0; // 跳過數

        public Form1()
        {
            InitializeComponent();
        }

        bool bFrtLoad = false;

        /// <summary>
        /// 初始化物件
        /// </summary>
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
                var img = qImageSave.Dequeue();
                img.Dispose();
            }
            rFocusRegions = new Rectangle[2];
            rFocusRegion = new Rectangle();
            rShowRegion = new Rectangle();
            mreLoad.Set();
            mreShow.Set();

            nTotal = nSave = nPass = 0;
        }

        /// <summary>
        /// 讀圖執行續
        /// </summary>
        private void thdImgLoad_DoWork()
        {
            while (qImgPath.Count > 0)
            {
                // 若已經有三張圖讀進記憶體則卡住
                if (qImages.Count > 3)
                {
                    mreLoad.Reset();
                    mreLoad.WaitOne();
                }
                string path = qImgPath.Dequeue();
                Bitmap bmp = new Bitmap(path); // 讀圖
                qImages.Enqueue(bmp); // 加入列隊
                qImgSavePath.Enqueue(path);

                // 初始是卡住狀態所以第一張要解鎖
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
                    //pbImgShow.Width = MAX_BOUND;
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
                    //pbImgShow.Height = MAX_BOUND;
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
                // 若列隊裡有圖則取出第一張秀出來
                if (qImages.Count > 0)
                {
                    imgShow = qImages.Dequeue();
                    ShowImage();
                    mreLoad.Set();
                }

                // 秀完需要卡住，不然會秀到沒圖為止
                // 平常卡住不讓迴圈空跑吃效能
                mreShow.Reset();
                mreShow.WaitOne();
            }
        }

        // 旋轉模式(未完成)
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

        int nCutMode = 0; // 裁切模式 0:切寬；1:切高
        float fRate; // 原圖與秀圖比例
        private void pbImgShow_Paint(object sender, PaintEventArgs e)
        {
            if (imgShow != null)
            {
                e.Graphics.DrawImage(imgShow, rShowRegion); // 秀圖
                Pen pen = new Pen(Brushes.Blue, 1);
                e.Graphics.DrawRectangle(pen, rFocusRegion); // 畫裁切框

                SolidBrush b = new SolidBrush(Color.FromArgb(100, 0, 255, 255));
                e.Graphics.FillRectangles(b, rFocusRegions); // 畫遮蔽區
            }
        }

        // 計算圖片與控制項關係
        private void pbImgShowTranslation()
        {
            if (imgShow != null)
            {
                // 計算比例
                float fWRate = (float)imgShow.Width / pnlImgShow.Width;
                float fHRate = (float)imgShow.Height / pnlImgShow.Height;
                fRate = fWRate > fHRate ? fWRate : fHRate;

                // 計算裁切模式(3:2)
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

                // 計算裁切區
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

                // 計算秀圖區
                rShowRegion.Size = new Size((int)(imgShow.Width / fRate), (int)(imgShow.Height / fRate));

                // 計算相框位置
                pbImgShow.Location = new Point((MAX_BOUND - rShowRegion.Width) / 2, (MAX_BOUND - rShowRegion.Height) / 2);
                pbImgShow.Size = rShowRegion.Size;
            }
        }

        /// <summary>
        /// 跳過
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            nPass++;
            UpdateTitle();
        }

        /// <summary>
        /// 讀取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            open.Filter = "Image files (*.jpg)|*.jpg";
            if (open.ShowDialog() != DialogResult.OK)
                return;

            IniObject();

            nTotal = open.SafeFileNames.Length;
            UpdateTitle();

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
            UpdateTitle();
            Thread thdImgShow = new Thread(new ThreadStart(thdImgShow_DoWork));
            thdImgShow.IsBackground = true;
            thdImgShow.Start();
        }

        bool bMouseDown = false; // 滑鼠按下旗標
        Point plastClick;  // 滑鼠按下時座標
        Point pFocusRegionXY; // 滑鼠按下時裁切區原點
        private void pbImgShow_MouseDown(object sender, MouseEventArgs e)
        {
            // 若滑鼠左鍵點在裁切區內則記錄座標
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
            // 裁切區移動計算
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

        // 根據裁切區更新遮蔽區
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

        // 裁切並存圖
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;

            Image imgSave = imgShow;
            imgShow = null;
            string path = qImgSavePath.Dequeue();

            FileInfo f = new FileInfo(path);
            string sCreationTime = f.CreationTime.ToString(); // 讀取原圖建立時間

            string sNewName = "_" + System.IO.Path.GetFileName(path);
            string sNewPath = System.IO.Path.GetDirectoryName(path) + @"\4X6\";
            if (!System.IO.Directory.Exists(sNewPath))
                System.IO.Directory.CreateDirectory(sNewPath);

            // 將裁切區改為原本倍率
            RectangleF rSave = new RectangleF(
                fRate * rFocusRegion.X,
                fRate * rFocusRegion.Y,
                fRate * rFocusRegion.Width,
                fRate * rFocusRegion.Height);

            // 非同步切圖存檔
            Task.Run(new Action(() =>
            {
                Bitmap bmpSave = new Bitmap((int)rSave.Width, (int)rSave.Height);
                var g = Graphics.FromImage(bmpSave);
                g.DrawImage(imgSave, new RectangleF(0,0, rSave.Width, rSave.Height), rSave, GraphicsUnit.Pixel);

                // 時間水印
                if (ckbDrawTime.Checked) g.DrawString(sCreationTime, new Font("Ariel", 120, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Red, new Point(10, 10));

                bmpSave.Save(sNewPath + sNewName, System.Drawing.Imaging.ImageFormat.Jpeg);
                g.Dispose();
                bmpSave.Dispose();

                imgSave.Dispose();
                imgSave = null;
            }));         

            mreShow.Set();

            rFocusRegion = new Rectangle();

            nSave++;
            UpdateTitle();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // 快捷鍵
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

        /// <summary>
        /// 順時鐘90度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnR90_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;

            rFocusRegion.X = 0;
            rFocusRegion.Y = 0;
            imgShow.RotateFlip(RotateFlipType.Rotate90FlipNone);      
            ShowImage();
        }

        /// <summary>
        /// 逆時鐘90度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnL90_Click(object sender, EventArgs e)
        {
            if (imgShow == null)
                return;

            rFocusRegion.X = 0;
            rFocusRegion.Y = 0;
            imgShow.RotateFlip(RotateFlipType.Rotate270FlipNone);
            ShowImage();
        }

        private void UpdateTitle()
        {
            this.Text = "Total: " + nTotal + ", Save: " + nSave + ", Pass: " + nPass + ", Left: " + (nTotal - nSave - nPass);
        }
    }
}
