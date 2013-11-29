using System.ServiceModel;

namespace GXService.Broadcast.Contract
{
    public interface IBroadcastCallBack
    {
        [OperationContract(IsOneWay = true)]
        void OnDataBroadcast(byte[] data);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IBroadcastCallBack))]
    public interface IBroadcast
    {
        [OperationContract(IsOneWay = true, IsInitiating = true, IsTerminating = false)]
        void Connect();

        [OperationContract]
        void Broadcast(byte[] data);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        void Disconnect();
    }
}
