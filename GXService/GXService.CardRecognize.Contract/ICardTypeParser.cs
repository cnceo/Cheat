using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GXService.CardRecognize.Contract
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICardTypeParser
    {
        [OperationContract]
        List<CardType> ParseCardType(List<Card> cards);
    }

    [DataContract]
    public class CardType
    {
        public float Weight { get; set; }

        [DataMember]
        public List<Card> Cards { get; set; }
    }
}
