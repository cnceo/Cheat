using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GXService.CardRecognize.Contract
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICardsRecognizer
    {
        [OperationContract(IsOneWay = true, IsInitiating = true, IsTerminating = false)]
        void Start();

        [OperationContract]
        bool IsMatch(byte[] captureBmpData, byte[] tmplBmpData, float similarityThreshold);

        [OperationContract]
        RecognizeResult Recognize(RecoginizeData data);

        [OperationContract]
        CardTypeResult ParseCardType(List<Card> cards);

        [OperationContract]
        CardTypeResult ParseCardTypeVsEnemy(List<Card> cards, List<Card> cardsEnemy);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        void Stop();
    }

    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class RecognizeResult
    {
        [DataMember]
        public List<Card> Result { get; set; }
    }

    [DataContract]
    public class RecoginizeData
    {
        [DataMember]
        public GameTemplateType GameTypeTemplate { get; set; }

        [DataMember]
        public byte[] CardsBitmap { get; set; }
    }
}
