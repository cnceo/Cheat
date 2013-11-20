using System.Runtime.Serialization;

namespace GXService.CardRecognize.Contract
{

    //游戏类型
    public enum GameTemplateType
    {
        斗地主手牌 = 0,
        十三张 = 1,
        斗地主出牌 = 2
    }

    public enum CardNum
    {
        未知 = -1,
        _A = 0,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _10,
        _J,
        _Q,
        _K,
        _Joke,
        _BigJoke,
        _Any
    }

    public enum CardColor
    {
        未知 = -1,
        黑桃 = 0,
        红桃 = 1,
        梅花 = 2,
        方块 = 3
    }

    [DataContract(Namespace = "GXService.CardRecognize.Contract")]
    public class Card
    {
        [DataMember]
        public CardNum Num { get; set; }

        [DataMember]
        public CardColor Color { get; set; }
    }
}
