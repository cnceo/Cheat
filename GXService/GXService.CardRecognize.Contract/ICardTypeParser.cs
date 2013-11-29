using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GXService.CardRecognize.Contract
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICardTypeParser
    {
        [OperationContract]
        CardTypeResult ParseCardType(List<Card> cards);

        [OperationContract]
        CardTypeResult ParseCardTypeVsEnemy(List<Card> cards, List<Card> cardsEnemy);
    }
}
