using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using AForge.Imaging;
using AForge.Imaging.Filters;
using GXService.CardRecognize.Contract;
using GXService.CardRecognize.Service.Factory;
using GXService.Utils;

namespace GXService.CardRecognize.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CardRecognizeService : ICardsRecognizer
    {
        private readonly List<CardTypeRecognizer> _recognizers = new List<CardTypeRecognizer>
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

        //灰度化并二值化
        private readonly FiltersSequence _seq = new FiltersSequence
            {
                Grayscale.CommonAlgorithms.BT709,
                new OtsuThreshold()
            };

        //模板匹配对象
        private readonly ExhaustiveTemplateMatching _tm = new ExhaustiveTemplateMatching();

        //模板模型对象
        private ITemplate _template;

        public void Start()
        {
            OperationContext.Current.Channel.Closing += (sender, args) => GC.Collect();
        }

        public void Stop()
        {
        }

        public RecognizeResult Recognize(RecoginizeData data)
        {
            _template = TemplateFactory.Singleton.GetTemplate(data.GameTypeTemplate);
            if (null == _template)
            {
                throw new FaultException("未设置模板模型对象");
            }

            var result = new RecognizeResult
            {
                Result = new List<Card>()
            };
            var cardsBitmap = Util.Deserialize(data.CardsBitmap) as Bitmap;

            if (cardsBitmap == null)
            {
                throw new FaultException("待识别的图片不合法或为空");
            }

            //获取玩家二值化的图片
            var bmpCards = cardsBitmap;
            var bmpNums = _seq.Apply(bmpCards.Clone(new Rectangle(0, 0, bmpCards.Width, bmpCards.Height / 2),
                                                    bmpCards.PixelFormat));
            var bmpColors = bmpCards;

            //遍历所有的模板，记录匹配出来的区域及牌大小
            var matches = new Dictionary<Rectangle, KeyValuePair<float, CardNum>>();

            _template.GetCardNumTemplate().ForEach(
                tmpl =>
                tmpl.Bitmaps
                    .ForEach(
                        bmpTmpl =>
                        {
                            _tm.SimilarityThreshold = tmpl.SimilaSimilarityThreshold;
                            _tm.ProcessImage(bmpNums, bmpTmpl)
                               .ToList()
                               .ForEach(match =>
                               {
                                   //如果匹配到的当前结果在matches集合中存在，则将牌值添加到牌值集合中
                                   var keyRect = matches.Keys
                                                        .ToList()
                                                        .FirstOrDefault(
                                                            key =>
                                                            key.Contains(new Point(
                                                                             match
                                                                                 .Rectangle
                                                                                 .Left +
                                                                             match
                                                                                 .Rectangle
                                                                                 .Width / 2,
                                                                             match
                                                                                 .Rectangle
                                                                                 .Top +
                                                                             match
                                                                                 .Rectangle
                                                                                 .Height / 2)));

                                   if (!keyRect.IsEmpty)
                                   {
                                       if (matches[keyRect].Key < match.Similarity)
                                       {
                                           matches[keyRect] = new KeyValuePair<float, CardNum>(
                                               match.Similarity, tmpl.Num);
                                       }
                                   }
                                   else
                                   {
                                       matches.Add(match.Rectangle,
                                                   new KeyValuePair<float, CardNum>(match.Similarity, tmpl.Num));
                                   }
                               });
                        }));

            //一个区域对应一个牌值对象
            matches.OrderBy(m => m.Key.Left)
                   .ToList()
                   .ForEach(match =>
                   {
                       var colorBmp =
                           bmpColors.Clone(new Rectangle(match.Key.X, 0, match.Key.Width, bmpColors.Height),
                                           bmpColors.PixelFormat);

                       result.Result.Add(new Card
                       {
                           Num = match.Value.Value,
                           Color = MatchColor(colorBmp)
                       });
                   });

            GC.Collect();

            return result;
        }

        private CardColor MatchColor(Bitmap colorBmp)
        {
            var cardColorMatch = new KeyValuePair<float, CardColor>(0, CardColor.未知);

            //识别
            _template.GetCardColorTemplate()
                     .ForEach(
                         tmpl =>
                         {
                             _tm.SimilarityThreshold = tmpl.SimilaSimilarityThreshold;

                             tmpl.Bitmaps
                                 .ForEach(
                                     bmp =>
                                     _tm.ProcessImage(colorBmp, bmp)
                                        .ToList()
                                        .ForEach(match =>
                                        {
                                            if (cardColorMatch.Key < match.Similarity)
                                            {
                                                cardColorMatch =
                                                    new KeyValuePair<float, CardColor>(
                                                        match.Similarity,
                                                        tmpl.Color);
                                            }
                                        }));
                         });

            GC.Collect();

            return cardColorMatch.Value;
        }
        
        public CardTypeResult ParseCardType(List<Card> cards)
        {
            return GetBestResult(ParseCardTypeResult(cards));
        }

        public CardTypeResult ParseCardTypeVsEnemy(List<Card> cards, List<Card> cardsEnemy)
        {
            return GetBestResult(ParseCardType(cards), ParseCardTypeResult(cards));
        }

        private List<CardTypeResult> ParseCardTypeResult(IEnumerable<Card> cards)
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
                                        {
                                            var headType = HeadCardTypeFactory.GetSingleton()
                                                                              .GetHeadCardType(tmp.FindAll(
                                                                                  card =>
                                                                                  !bodyType.GetCards().Contains(card) &&
                                                                                  !tailType.GetCards().Contains(card))
                                                                                                  .ToList());
                                            if (tailType.Compare(bodyType, EmRegionCompare.Tail) >= 0 && bodyType.CompareTypeRule(headType) >= 0)
                                            {
                                                result.Add(new CardTypeResult(headType, bodyType, tailType));   
                                            }
                                        }));
                });

            return result;
        }

        private static CardTypeResult GetBestResult(List<CardTypeResult> results)
        {
            CardTypeResult best = null;
            results.ForEach(ctr =>
            {
                best = best == null
                           ? ctr
                           : (best.Compare(ctr) >= 0
                                  ? best
                                  : ctr);
            });
            return best;
        }

        private static CardTypeResult GetBestResult(CardTypeResult bestResult, List<CardTypeResult> resultsEnemy)
        {
            resultsEnemy.ForEach(res =>
            {
                bestResult = bestResult.Compare(res) >= 0
                                 ? bestResult
                                 : res;
            });
            return bestResult;
        }
    }
}
