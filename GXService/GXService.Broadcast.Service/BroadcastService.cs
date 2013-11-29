using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GXService.Broadcast.Contract;

namespace GXService.Broadcast.Service
{
    class ClientContext
    {
        public string SessionId { get; set; }
        public IBroadcastCallBack BroadcastCallBack { get; set; }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, Namespace = "GXService.Broadcast")]
    public class BroadcastService : IBroadcast
    {
        //客户端信息
        private static readonly Dictionary<string, ClientContext> DicClients = new Dictionary<string, ClientContext>();

        //当前会话ID
        private string _currentSessionId;

        public void Connect()
        {
            _currentSessionId = OperationContext.Current.SessionId;
            var callBack = OperationContext.Current.GetCallbackChannel<IBroadcastCallBack>();
            if (callBack != null && !string.IsNullOrEmpty(_currentSessionId))
            {
                DicClients[_currentSessionId] = new ClientContext
                    {
                        SessionId = _currentSessionId,
                        BroadcastCallBack = callBack
                    };
                OperationContext.Current.Channel.Closing += (sender, args) => Disconnect();
            }
        }

        public void Broadcast(byte[] data)
        {
            DicClients
                .Where(client => client.Key != _currentSessionId)
                .ToList()
                .ForEach(client =>
                         client.Value.BroadcastCallBack.OnDataBroadcast(data));
        }

        public void Disconnect()
        {
            if (!string.IsNullOrEmpty(_currentSessionId) && DicClients.ContainsKey(_currentSessionId))
            {
                DicClients.Remove(_currentSessionId);
            }
        }
    }
}
