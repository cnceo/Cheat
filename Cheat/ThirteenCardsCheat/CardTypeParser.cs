using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirteenCardsCheat
{
    public class CardsTest
    {
        private readonly List<Card> _cards = new List<Card>();

        public CardsTest()
        {
            InitCards();
        }

        private void InitCards()
        {
            var nums = new[]
                {
                    CardNum._A, CardNum._2, CardNum._3, CardNum._4,
                    CardNum._5, CardNum._6, CardNum._7, CardNum._8,
                    CardNum._9, CardNum._10, CardNum._J, CardNum._Q,
                    CardNum._K
                };

            var colors = new[]
                {
                    CardColor.黑桃, CardColor.红桃, CardColor.梅花, CardColor.方块
                };

            colors.ToList()
                  .ForEach(color =>
                           nums.ToList()
                               .ForEach(num =>
                                        _cards.Add(new Card { Num = num, Color = color })));
        }

        public List<Card> GetNextPlayerCards(Int32 count)
        {
            var result = new List<Card>();

            if (count > _cards.Count)
            {
                return _cards.ToList();
            }

            var rnd = new Random();
            for (var i = 0; i < count; i++)
            {
                var index = rnd.Next(0, _cards.Count);
                //index = i + 13 * (i / 4);
                result.Add(_cards[index]);
                _cards.RemoveAt(index);
            }

            return result.OrderBy(card => card.Num).ThenBy(card => card.Color).ToList();
        }
    }
	
    public abstract class CardTypeRecognizer
    {
        protected Dictionary<CardNum, List<Card>> CardsDic = new Dictionary<CardNum, List<Card>>
            {
                {CardNum._2, new List<Card>()},{CardNum._3, new List<Card>()},{CardNum._4, new List<Card>()},
                {CardNum._5, new List<Card>()},{CardNum._6, new List<Card>()},{CardNum._7, new List<Card>()},
                {CardNum._8, new List<Card>()},{CardNum._9, new List<Card>()},{CardNum._10, new List<Card>()},
                {CardNum._J, new List<Card>()},{CardNum._Q, new List<Card>()},{CardNum._K, new List<Card>()},
                {CardNum._A, new List<Card>()}
            };

        //同花顺>；炸弹>；葫芦>；同花>；顺子>；三条>；两对>；一对>5单张
        public abstract List<CardType> Recognize(List<Card> cards);
    }
    
    /// <summary>
    /// 同花
    /// </summary>
    public class FlushCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            var flushDic = new Dictionary<CardColor, List<Card>>
                {
                    {CardColor.黑桃, new List<Card>()},
                    {CardColor.红桃, new List<Card>()},
                    {CardColor.梅花, new List<Card>()},
                    {CardColor.方块, new List<Card>()}
                };

            cards.ForEach(card => flushDic[card.Color].Add(card));

            flushDic.ToList()
                    .ForEach(dic =>
                        {
                            if (dic.Value.Count >= 5)
                            {
                                dic.Value
                                   .Combination(5)
                                   .ToList()
                                   .ForEach(flush => result.Add(new FlushCardType(flush.ToList())));
                            }
                        });

            return result;
        }
    }
    
    /// <summary>
    /// 顺子
    /// </summary>
    public class StraightCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keys = new List<CardNum>();

            CardsDic
                .Where(dic => dic.Key != CardNum._2).ToList()
                .ForEach(dic =>
                {
                    if (dic.Value.Count > 0)
                    {
                        keys.Add(dic.Key);
                    }

                    if (dic.Value.Count <= 0 || keys.Count >= 13)
                    {
                        while (keys.Count >= 5)
                        {
                            CardsDic[keys[0]]
                                .ForEach(
                                    card1 =>
                                    CardsDic[keys[1]]
                                        .ForEach(
                                            card2 =>
                                            CardsDic[keys[2]]
                                                .ForEach(
                                                    card3 =>
                                                    CardsDic[keys[3]]
                                                        .ForEach(
                                                            card4 =>
                                                            CardsDic[keys[4]]
                                                                .ForEach(
                                                                    card5 =>
                                                                    result.Add(new StraightCardType(new List<Card>
                                                                                    {
                                                                                        card1,
                                                                                        card2,
                                                                                        card3,
                                                                                        card4,
                                                                                        card5
                                                                                    }
                                                                        ))
                                                                )))));
                            keys.RemoveAt(0);
                        }

                        keys.Clear();
                    }
                });

            return result;
        }
    }

    /// <summary>
    /// 同花顺
    /// </summary>
    public class StraightFlushCardTypeRecognizer : StraightCardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            base.Recognize(cards)
                .ForEach(cardType =>
                    {
                        if (cardType.GetCards().ToList().TrueForAll(card => card.Color == cardType.GetCards()[0].Color))
                        {
                            result.Add(new StraightFlushCardType(cardType.GetCards().ToList()));
                        }
                    });

            return result;
        }
    }

    /// <summary>
    /// 炸弹
    /// </summary>
    public class BoomCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keysMore1 = new List<CardNum>();
            var keysMore4 = new List<CardNum>();

            CardsDic.ToList()
                    .ForEach(dic =>
                        {
                            if (dic.Value.Count <= 0)
                            {
                                return;
                            }

                            keysMore1.Add(dic.Key);

                            if (dic.Value.Count >= 4)
                            {
                                keysMore4.Add(dic.Key);
                            }
                        });

            keysMore4
                .ForEach(key4 =>
                         keysMore1
                             .Where(key1 => key1 != key4)
                             .ToList()
                             .ForEach(key1 =>
                                      CardsDic[key1]
                                          .ForEach(card1 =>
                                              {
                                                  var cardsType = CardsDic[key4].ToList();
                                                  cardsType.Add(card1);
                                                  result.Add(new BoomCardType(cardsType));
                                              })));
            return result;
        }
    }

    /// <summary>
    /// 葫芦(3带2)
    /// </summary>
    public class GourdCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keysMore2 = new List<CardNum>();
            var keysMore3 = new List<CardNum>();

            CardsDic.ToList()
                    .ForEach(dic =>
                        {
                            if (dic.Value.Count >= 2)
                            {
                                keysMore2.Add(dic.Key);
                                if (dic.Value.Count >= 3)
                                {
                                    keysMore3.Add(dic.Key);
                                }
                            }
                        });

            keysMore3
                .ForEach(key3 =>
                         keysMore2
                             .Where(key2 => key2 != key3)
                             .ToList()
                             .ForEach(key2 =>
                                      CardsDic[key3]
                                          .Combination(3)
                                          .ToList()
                                          .ForEach(c3 =>
                                                   CardsDic[key2]
                                                       .Combination(2)
                                                       .ToList()
                                                       .ForEach(c2 => result.Add(new GourdCardType(c3.Concat(c2).ToList()))))));

            return result;
        }
    }

    /// <summary>
    /// 三张(3带两个单张)
    /// </summary>
    public class ThreeSameCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keysMore3 = new List<CardNum>();
            var keysMore1 = new List<CardNum>();

            CardsDic.ToList()
                .ForEach(dic =>
                    {
                        if (dic.Value.Count < 1)
                        {
                            return;
                        }

                        keysMore1.Add(dic.Key);

                        if (dic.Value.Count >= 3)
                        {
                            keysMore3.Add(dic.Key);
                        }
                    });

            keysMore3
                .ForEach(key3 =>
                         CardsDic[key3]
                             .Combination(3)
                             .ToList()
                             .ForEach(comb3 =>
                                      keysMore1
                                          .Combination(2)
                                          .ToList()
                                          .ForEach(comb2Keys =>
                                              {
                                                  var enumerable = comb2Keys as IList<CardNum> ?? comb2Keys.ToList();
                                                  if (enumerable.Contains(key3))
                                                  {
                                                      return;
                                                  }
                                                  var cardNums = enumerable.ToList();
                                                  CardsDic[cardNums[0]]
                                                      .ForEach(card1 =>
                                                               CardsDic[cardNums[1]]
                                                                   .ForEach(card2 =>
                                                                       {
                                                                           var cardType = comb3.ToList();
                                                                           cardType.Add(card1);
                                                                           cardType.Add(card2);
                                                                           result.Add(new ThreeSameCardType(cardType));
                                                                       }));
                                              })));

            return result;
        }
    }

    public class DoubleCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keysMore2 = new List<CardNum>();
            var keysMore1 = new List<CardNum>();

            CardsDic.ToList()
                    .ForEach(dic =>
                        {
                            if (dic.Value.Count <= 0)
                            {
                                return;
                            }

                            keysMore1.Add(dic.Key);

                            if (dic.Value.Count >= 2)
                            {
                                keysMore2.Add(dic.Key);
                            }
                        });

            keysMore2
                .ForEach(keyDouble =>
                         CardsDic[keyDouble]
                             .Combination(2)
                             .ToList()
                             .ForEach(cardDouble =>
                                      keysMore1
                                          .Combination(3)
                                          .Where(comb3 => !comb3.Contains(keyDouble))
                                          .ToList()
                                          .ForEach(comb3 =>
                                              {
                                                  var cardNums = comb3 as IList<CardNum> ?? comb3.ToList();
                                                  CardsDic[cardNums.ToList()[0]]
                                                      .ToList()
                                                      .ForEach(card3 =>
                                                               CardsDic[cardNums.ToList()[1]]
                                                                   .ToList()
                                                                   .ForEach(card4 =>
                                                                            CardsDic[cardNums.ToList()[2]]
                                                                                .ToList()
                                                                                .ForEach(card5 =>
                                                                                    {
                                                                                        var cardsType =
                                                                                            cardDouble.ToList();
                                                                                        cardsType.AddRange(new List
                                                                                                               <Card>
                                                                                            {
                                                                                                card3,
                                                                                                card4,
                                                                                                card5
                                                                                            });
                                                                                        result.Add(
                                                                                            new OnePairCardType(
                                                                                                cardsType));
                                                                                    })));
                                              })));

            return result;
        }
    }

    public class TwoDoubleCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keysMore2 = new List<CardNum>();
            var keysMore1 = new List<CardNum>();

            CardsDic.ToList()
                    .ForEach(dic =>
                        {
                            if (dic.Value.Count <= 0)
                            {
                                return;
                            }

                            keysMore1.Add(dic.Key);

                            if (dic.Value.Count >= 2)
                            {
                                keysMore2.Add(dic.Key);
                            }
                        });
            if (keysMore2.Count < 2)
            {
                return result;
            }
            keysMore2.Combination(2)
                     .ToList()
                     .ForEach(comb2 =>
                         {
                             var cardNums = comb2 as CardNum[] ?? comb2.ToArray();
                             CardsDic[cardNums.ToList()[0]]
                                 .Combination(2)
                                 .ToList()
                                 .ForEach(cards2First =>
                                          CardsDic[cardNums.ToList()[1]]
                                              .Combination(2)
                                              .ToList()
                                              .ForEach(cards2Second =>
                                                       keysMore1
                                                           .Where(key => !cardNums.Contains(key))
                                                           .ToList()
                                                           .ForEach(cardOneKey =>
                                                                    CardsDic[cardOneKey]
                                                                        .ForEach(card =>
                                                                            {
                                                                                var cardsType = new List<Card>();
                                                                                var enumerable = cards2First as IList<Card> ?? cards2First.ToList();
                                                                                var collection = cards2Second as IList<Card> ?? cards2Second.ToList();
                                                                                if (enumerable.ToList()[0].Num >= collection.ToList()[0].Num)
                                                                                {
                                                                                    cardsType.AddRange(enumerable);
                                                                                    cardsType.AddRange(collection);
                                                                                }
                                                                                else
                                                                                {
                                                                                    cardsType.AddRange(collection);
                                                                                    cardsType.AddRange(enumerable);
                                                                                }
                                                                                cardsType.Add(card);
                                                                                result.Add(new DoublePairCardType
                                                                                               (
                                                                                               cardsType
                                                                                               ));
                                                                            }))));
                         });

            return result;
        }
    }

    public class OnePieceCardTypeRecognizer : CardTypeRecognizer
    {
        public override List<CardType> Recognize(List<Card> cards)
        {
            var result = new List<CardType>();
            CardsDic.Keys.ToList().ForEach(key => CardsDic[key].Clear());

            cards.ForEach(card => CardsDic[card.Num].Add(card));

            var keysMore1 = new List<CardNum>();

            CardsDic
                .ToList()
                .ForEach(dic =>
                    {
                        if (dic.Value.Count > 0)
                        {
                            keysMore1.Add(dic.Key);
                        }
                    });

            if (keysMore1.Count > 5)
            {
                keysMore1
                    .Combination(5)
                    .ToList()
                    .ForEach(comb5 =>
                        {
                            var cardNums = comb5 as IList<CardNum> ?? comb5.ToList();
                            CardsDic[cardNums.ToList()[0]]
                                .ToList()
                                .ForEach(card1 =>
                                         CardsDic[cardNums.ToList()[1]]
                                             .ToList()
                                             .ForEach(card2 =>
                                                      CardsDic[cardNums.ToList()[2]]
                                                          .ToList()
                                                          .ForEach(card3 =>
                                                                   CardsDic[cardNums.ToList()[3]]
                                                                       .ToList()
                                                                       .ForEach(card4 =>
                                                                                CardsDic[cardNums.ToList()[4]]
                                                                                    .ToList()
                                                                                    .ForEach(card5 =>
                                                                                             result.Add(new NoTypeCardType
                                                                                                            (
                                                                                                            new List
                                                                                                                <Card>
                                                                                                                {
                                                                                                                    card1,
                                                                                                                    card2,
                                                                                                                    card3,
                                                                                                                    card4,
                                                                                                                    card5
                                                                                                                }
                                                                                                            )))))));
                        });
            }

            return result;
        }
    }

    public class CardTypeParser
    {
        private readonly List<CardTypeRecognizer> _recognizers =
            new List<CardTypeRecognizer>
                {
                    new StraightFlushCardTypeRecognizer(),
                    new BoomCardTypeRecognizer(),
                    new GourdCardTypeRecognizer(),
                    new FlushCardTypeRecognizer(),
                    new StraightCardTypeRecognizer(),
                    new ThreeSameCardTypeRecognizer(),
                    new TwoDoubleCardTypeRecognizer(),
                    new DoubleCardTypeRecognizer(),
                    new OnePieceCardTypeRecognizer()
                };

        public List<CardTypeResult> Parse(List<Card> cards)
        {
            var result = new List<CardTypeResult>();
            var resultTmp = new List<CardType>();
            var tmp = cards.ToList();

            _recognizers.Where(rec => !(rec is OnePieceCardTypeRecognizer))
                        .ToList()
                        .ForEach(rec => resultTmp.AddRange(rec.Recognize(tmp)));

            resultTmp.ForEach(bodyType =>
                {
                    var tmpCards = tmp.FindAll(card => !bodyType.GetCards().Contains(card)).ToList();
                    _recognizers
                        .ForEach(rec =>
                                 rec.Recognize(tmpCards)
                                    .ForEach(tailType =>
                                             result.Add(new CardTypeResult(
                                                            HeadCardTypeFactory.GetSingleton()
                                                                               .GetHeadCardType(tmp.FindAll(
                                                                                   card =>
                                                                                   !bodyType.GetCards().Contains(card) &&
                                                                                   !tailType.GetCards().Contains(card))
                                                                                                   .ToList()),
                                                            bodyType, tailType))));
                });

            return result;
        }
    }
}