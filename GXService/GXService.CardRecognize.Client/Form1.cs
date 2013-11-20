using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GXService.CardRecognize.Client.CardRecognize;
using GXService.CardRecognize.Client.CardTypeParse;
using GXService.CardRecognize.Client.Broadcast;
using GXService.Utils;
using Card = GXService.CardRecognize.Client.CardTypeParse.Card;
using CardColor = GXService.CardRecognize.Client.CardTypeParse.CardColor;
using CardNum = GXService.CardRecognize.Client.CardTypeParse.CardNum;

namespace GXService.CardRecognize.Client
{
    public partial class Form1 : Form
    {
        private readonly CardsRecognizerClient _proxyRecognize = new CardsRecognizerClient();
        private readonly CardTypeParserClient _proxyParser = new CardTypeParserClient();
        private readonly BroadcastClient _proxyBroadcast;
        private readonly BroadcastCallback _broadcastCallback = new BroadcastCallback();
        private string _cardsDisplay = "";

        public Form1()
        {
            InitializeComponent();


            try
            {
                //_broadcastCallback.BroadcastData += args => rtbAllMessages.Text += args.Message + @"\r\n";
                _proxyBroadcast = new BroadcastClient(new InstanceContext(_broadcastCallback));
                _proxyBroadcast.ClientCredentials.UserName.UserName = "show";
                _proxyBroadcast.ClientCredentials.UserName.Password = "test";
                _proxyBroadcast.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }

            try
            {
                _proxyParser.ClientCredentials.UserName.UserName = "show";
                _proxyParser.ClientCredentials.UserName.Password = "test";

                var begin = DateTime.Now;
                var resultParseType = _proxyParser.ParseCardType(new[]
                {
                    new Card{Color = CardColor.黑桃, Num = CardNum._3}, 
                    new Card{Color = CardColor.红桃, Num = CardNum._2}, 
                    new Card{Color = CardColor.黑桃, Num = CardNum._4}, 
                    new Card{Color = CardColor.黑桃, Num = CardNum._5}, 
                    new Card{Color = CardColor.梅花, Num = CardNum._7}, 
                    new Card{Color = CardColor.方块, Num = CardNum._7}, 
                    new Card{Color = CardColor.红桃, Num = CardNum._8}, 
                    new Card{Color = CardColor.梅花, Num = CardNum._8}, 
                    new Card{Color = CardColor.黑桃, Num = CardNum._10}, 
                    new Card{Color = CardColor.方块, Num = CardNum._J}, 
                    new Card{Color = CardColor.黑桃, Num = CardNum._J}, 
                    new Card{Color = CardColor.梅花, Num = CardNum._Q}, 
                    new Card{Color = CardColor.方块, Num = CardNum._K}
                });
                var end = DateTime.Now;

                using (var sw = new StreamWriter(@"result.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine("总共花费" + (end - begin).TotalSeconds + "秒");
                    resultParseType.ToList()
                                   .ForEach(type =>
                                   {
                                       type.Cards
                                           .ToList()
                                           .ForEach(card =>
                                                    sw.WriteLine(card.Color.ToString() + card.Num.ToString()));
                                       sw.WriteLine();
                                   });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }

            return;

            var bmp = Image.FromFile(@"test.bmp") as Bitmap;
            if (null == bmp || _proxyRecognize.ClientCredentials == null)
            {
                return;
            }

            _proxyRecognize.ClientCredentials.UserName.UserName = "show";
            _proxyRecognize.ClientCredentials.UserName.Password = "test";

            try
            {
                while (true)
                {
                    var result = _proxyRecognize.Recognize(new RecoginizeData
                    {
                        GameTypeTemplate = GameTemplateType.斗地主手牌,
                        CardsBitmap = Util.Serialize(bmp.Clone(new Rectangle(280, 590, 400, 50), bmp.PixelFormat))
                    });

                    result.Result
                          .ToList()
                          .ForEach(card => _cardsDisplay += card.Color.ToString() + card.Num.ToString());

                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(_cardsDisplay, new Font("Tahoma", 8, FontStyle.Bold), Brushes.Red, 0, 0);
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
