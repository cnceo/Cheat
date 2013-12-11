using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GXService.CardRecognize.Client.BroadcastServiceReference;
using GXService.CardRecognize.Client.CardRecognizeServiceReference;
using GXService.Utils;

namespace GXService.CardRecognize.Client
{
    public partial class Form1 : Form
    {
        private readonly CardsRecognizerClient _proxyRecognize = new CardsRecognizerClient();
        private readonly BroadcastClient _proxyBroadcast;
        private readonly BroadcastCallback _broadcastCallback = new BroadcastCallback();
        private string _cardsDisplay = "";

        public Form1()
        {
            InitializeComponent();
            
            _proxyBroadcast = new BroadcastClient(new InstanceContext(_broadcastCallback));

            _proxyBroadcast.ClientCredentials.UserName.UserName = "show";
            _proxyBroadcast.ClientCredentials.UserName.Password = "test";

            _proxyRecognize.ClientCredentials.UserName.UserName = "show";
            _proxyRecognize.ClientCredentials.UserName.Password = "test";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(_cardsDisplay, new Font("Tahoma", 8, FontStyle.Bold), Brushes.Red, 0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var treeView = @"MDIE - [WinIO_KeyBoard]".FindWindow()
                                                     .GetChildWindows()
                                                     .First(w =>
                                                            User32Api.IsWindowVisible(w) &&
                                                            w.GetClassName() == "SysTreeView32");
            var itemText = treeView.GetItemText(treeView.GetRootItem());

            return;



            try
            {
                //_broadcastCallback.BroadcastData += args => rtbAllMessages.Text += args.Message + @"\r\n";
                _proxyBroadcast.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }

            try
            {

                var bmp = Image.FromFile(@"login.bmp") as Bitmap;
                if (null == bmp || _proxyRecognize.ClientCredentials == null)
                {
                    return;
                }

                _proxyRecognize.Open();

                //var process = Process.Start(@"D:\Program Files\拇指通科技\赖子山庄\赖子山庄.exe");
                //process.WaitForInputIdle();
                //Thread.Sleep(5000);
                //var wndBmp = process.MainWindowHandle.Capture();

                var wnd = "赖子山庄".FindWindow();
                if (wnd == IntPtr.Zero)
                {
                    MessageBox.Show("找不到赖子山庄窗口");
                    return;
                }
                var childWnds = wnd.GetChildWindows();
                var index = 0;
                childWnds.ForEach(wndChild => wndChild.GetWindowRect().Capture().Save(string.Format("{0}.bmp", index++)));

                return;

                wnd.SetForeground();
                //wnd = User32Api.FindWindow("Edit", null);
                var wndBmp = wnd.GetWindowRect().Capture();

                wndBmp.Save("ffff.bmp");
                //wndBmp.Clone(new Rectangle{X=8,Y=173,Width=59,Height=24}, wndBmp.PixelFormat).Save("eeee.bmp");
                //var wndBmp = Image.FromFile("qqq.bmp") as Bitmap;

                var beginTime = DateTime.Now.Ticks;
                var rect = _proxyRecognize.Match(wndBmp.Serialize(), bmp.Serialize(), (float) 0.84);
                var endTime = DateTime.Now.Ticks;
                var t = new TimeSpan(endTime - beginTime);

                using (var fs = new FileStream("time.txt", FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(string.Format("{0}秒", t.Seconds));
                    }
                }
                wnd.SetForeground();
                rect.Center().MouseLClick(wnd);
                rect.Center().MouseLClick(wnd);
                //(rect.Center() + new Size(rect.Width, 0)).MouseLClick(wnd);
                //WinIoLab.Singleton.KeyPress(new List<Keys> {Keys.T, Keys.E, Keys.S, Keys.T, Keys.Space});

                wndBmp.Clone(rect, wndBmp.PixelFormat).Save("rect.bmp");
                Thread.Sleep(5000);

                return;
                var result = _proxyRecognize.Recognize(new RecoginizeData
                    {
                        GameTypeTemplate = GameTemplateType.斗地主手牌,
                        CardsBitmap = bmp.Clone(new Rectangle(280, 590, 400, 50), bmp.PixelFormat).Serialize()
                    });

                var begin = DateTime.Now;
                //MouseInputManager.Click(514, 396);

                var cardsTest = new CardsTest();
                var cards = cardsTest.GetNextPlayerCards(13);
                cards = result.Result.Take(13).ToList();
                var resultParseType = _proxyRecognize.ParseCardType(cards.ToArray());
                var end = DateTime.Now;

                using (var fs = new FileStream("result.txt", FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        var resultDisplay = "";
                        cards.ForEach(card => resultDisplay += "{" + card.Color + "," + card.Num + "}");
                        sw.WriteLine(resultDisplay);

                        var tmp = "";

                        resultParseType.CardTypeHead
                                       .Cards
                                       .ToList()
                                       .ForEach(card => tmp += "{" + card.Color + "," + card.Num + "}");
                        tmp += "(" + resultParseType.CardTypeHead.CardTypeEm + ")   ";

                        resultParseType.CardTypeMiddle
                                       .Cards
                                       .ToList()
                                       .ForEach(card => tmp += "{" + card.Color + "," + card.Num + "}");
                        tmp += "(" + resultParseType.CardTypeMiddle.CardTypeEm + ")   ";

                        resultParseType.CardTypeTail
                                       .Cards
                                       .ToList()
                                       .ForEach(card => tmp += "{" + card.Color + "," + card.Num + "}");
                        tmp += "(" + resultParseType.CardTypeTail.CardTypeEm + ")";

                        sw.WriteLine(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }
        }
    }

    public class CardsTest
    {
        private readonly List<Card> _cards = new List<Card>();

        public CardsTest()
        {
            InitCards();
        }

        private void InitCards()
        {
            var nums = new[]
                {
                    CardNum._A, CardNum._2, CardNum._3, CardNum._4,
                    CardNum._5, CardNum._6, CardNum._7, CardNum._8,
                    CardNum._9, CardNum._10, CardNum._J, CardNum._Q,
                    CardNum._K
                };

            var colors = new[]
                {
                    CardColor.黑桃, CardColor.红桃, CardColor.梅花, CardColor.方块
                };

            colors.ToList()
                  .ForEach(color =>
                           nums.ToList()
                               .ForEach(num =>
                                        _cards.Add(new Card { Num = num, Color = color })));
        }

        public List<Card> GetNextPlayerCards(Int32 count)
        {
            var result = new List<Card>();

            if (count > _cards.Count)
            {
                return _cards.ToList();
            }

            var rnd = new Random();
            for (var i = 0; i < count; i++)
            {
                var index = rnd.Next(0, _cards.Count);
                //index = i + 13 * (i / 4);
                result.Add(_cards[index]);
                _cards.RemoveAt(index);
            }

            return result.OrderBy(card => card.Num).ThenBy(card => card.Color).ToList();
        }
    }

    class BroadcastArgs : EventArgs
    {
        public string Message { get; private set; }

        public BroadcastArgs(string message)
        {
            Message = message;
        }
    }

    class BroadcastCallback : IBroadcastCallback
    {
        public delegate void DataBroadcast(BroadcastArgs args);

        public event DataBroadcast BroadcastData;

        protected virtual void OnBroadcastData(byte[] data)
        {
            var handler = BroadcastData;
            if (handler != null)
            {
                handler(new BroadcastArgs(Encoding.UTF8.GetString(data)));
            }
        }

        public void OnDataBroadcast(byte[] data)
        {
            OnBroadcastData(data);
        }
    }
}
