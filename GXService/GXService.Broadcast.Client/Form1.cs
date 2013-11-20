using System;
using System.ComponentModel;
using System.Drawing;
using System.ServiceModel;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using GXService.Broadcast.Client.Broadcast;
//using GXService.Broadcast.Client.CardRecognize;
using GXService.Utils;

namespace GXService.Broadcast.Client
{
    public partial class Form1 : Form
    {
        private readonly BroadcastClient _proxyBroadcast;
        private readonly BroadcastCallback _broadcastCallback = new BroadcastCallback();

        //private readonly CardsRecognizerClient _proxyCardRecognize = new CardsRecognizerClient();

        private string cardsDisplay = "";

        public Form1()
        {
            try
            {
                _broadcastCallback.BroadcastData += args => rtbAllMessages.Text += args.Message + @"\r\n";
                _proxyBroadcast = new BroadcastClient(new InstanceContext(_broadcastCallback));
                _proxyBroadcast.ClientCredentials.UserName.UserName = "show";
                _proxyBroadcast.ClientCredentials.UserName.Password = "te9st";
                _proxyBroadcast.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }

            //_proxyCardRecognize.Start();

            //var bmp = Image.FromFile(@"test.bmp") as Bitmap;

            //var result = _proxyCardRecognize
            //    .Recognize(new RecoginizeData
            //        {
            //            GameTypeTemplate = GameTemplateType.斗地主手牌,
            //            CardsBitmap = Util.Serialize(bmp.Clone(new Rectangle(280, 590, 400, 50), bmp.PixelFormat))
            //        });
            //    result.Result
            //    .ToList()
            //    .ForEach(card =>
            //             cardsDisplay += card.Color.ToString() + card.Num.ToString());

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(cardsDisplay, new Font("Tahoma", 8, FontStyle.Bold), Brushes.Red, 0, 0);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            _proxyBroadcast.Broadcast(Encoding.UTF8.GetBytes(rtbToSendMessage.Text));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_proxyBroadcast.State == CommunicationState.Opened)
            {
                _proxyBroadcast.Close();
            }
            base.OnClosing(e);
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
