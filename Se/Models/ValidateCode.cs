using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Models
{
    public static class ValidateCode
    {
        /// <summary>
        /// 验证码的最大长度
        /// </summary>
        public static int MaxLength
        {
            get { return 10; }
        }
        /// <summary>
        /// 验证码的最小长度
        /// </summary>
        public static int MinLength
        {
            get { return 1; }
        }

        private const float PI = 3.14159265358979f;
        private const float PI2 = 6.28318530717959f;

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString GetValidateCode(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("/Home/CheckCode");
        }
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="codeCount">指定验证码的长度</param>
        /// <returns></returns>
        public static string CreateValidateCode(int codeCount)
        {
            string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(60);
                if (temp == t)
                {
                    return CreateValidateCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        //函数名称:TwistImage 
        //函数参数: dMultValue－波形的幅度倍数；dPhase波形的起始相位，取值区间[0-2*PI)；bXDir－扭曲方式
        //函数功能:正弦曲线Wave扭曲图片。函数可以迭加使用，以获得不同方式不同程度的效果
        //这个天才的函数，已经无法考证出处了。感谢原作者！
        private static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            double dBaseAxisLen = bXDir ? Convert.ToDouble(destBmp.Height) : Convert.ToDouble(destBmp.Width);
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? PI2 * Convert.ToDouble(j) / dBaseAxisLen : PI2 * Convert.ToDouble(i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    //取得当前点的颜色
                    int nOldX = 0;
                    int nOldY = 0;
                    nOldX = bXDir ? i + Convert.ToInt32(dy * 1) : i;
                    nOldY = bXDir ? j : j + Convert.ToInt32(dy * 1);
                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }
        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 30);
            //扭曲验证字符。TwistImage参数可自行修改
            int Twist1, Twist2;
            Random rnd3d = new Random();

            double S3d = rnd3d.NextDouble();
            if (S3d > 0.9)
            {
                Twist1 = 0;
                Twist2 = 0;
            }
            else
            {
                Random rnd4 = new Random();
                Twist1 = Convert.ToInt32(rnd4.NextDouble() * 4);  //扭曲参数随机生成
                Twist2 = Convert.ToInt32(rnd4.NextDouble() * 4); //扭曲参数随机生成
            }
            //扭曲图片
            image = TwistImage(image, true, -Twist1, -Twist2);



            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 14, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);

                //在生产的画面的基础上画一些横线
                Pen blackPen = new Pen(Color.Black, 0);
                Random rand = new Random();
                for (int i = 0; i < 2; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    g.DrawLine(blackPen, x, y, image.Width, y);
                }
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }


        /// <summary>
        /// 得到验证码图片的长度
        /// </summary>
        /// <param name="validateNumLength">验证码的长度</param>
        /// <returns></returns>
        public static int GetImageWidth(int validateNumLength)
        {
            return (int)(validateNumLength * 12.5);
        }
        /// <summary>
        /// 得到验证码的高度
        /// </summary>
        /// <returns></returns>
        public static double GetImageHeight()
        {
            return 20;
        }

    }
}