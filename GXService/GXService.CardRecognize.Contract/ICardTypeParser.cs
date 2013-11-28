using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GXService.CardRecognize.Contract
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICardTypeParser
    {
        [OperationContract]
        [ServiceKnownType(typeof(Card))]
        CardTypeResult ParseCardType(CardSet cards);

        [OperationContract]
        [ServiceKnownType(typeof(Card))]
        CardTypeResult ParseCardTypeVsEnemy(CardSet cards, CardSet cardsEnemy);
    }
}
