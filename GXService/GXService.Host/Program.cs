using System;
using System.ServiceModel;
using GXService.Broadcast.Service;
using GXService.CardRecognize.Service;

namespace GXService.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var hostBroadcast = new ServiceHost(typeof(BroadcastService));
                hostBroadcast.Opened += (sender, eventArgs) => Console.WriteLine("数据广播服务器启动完成!");
                if (hostBroadcast.State != CommunicationState.Opened)
                {
                    hostBroadcast.Open();
                }

                var hostCardRecognize = new ServiceHost(typeof(CardRecognizeService));
                hostCardRecognize.Opened += (sender, eventArgs) => Console.WriteLine("扑克牌识别服务器启动完成!");
                if (hostCardRecognize.State != CommunicationState.Opened)
                {
                    hostCardRecognize.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString());
            }

            Console.ReadKey();
        }
    }
}
