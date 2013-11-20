using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GXService.CardRecognize.Contract;

namespace GXService.CardRecognize.Service.Factory
{
    class DDZThrowTemplate : ITemplate
    {
        //模板总目录
        private const string TemplateDir = @"Template\斗地主\出牌\";

        //数字模板信息
        private readonly List<CardNumMatchInfo> _cardNumMatchInfos = new List<CardNumMatchInfo>();

        //花色模板信息
        private readonly List<CardColorMatchInfo> _cardColorMatchInfos = new List<CardColorMatchInfo>();

        public DDZThrowTemplate()
        {
            LoadCardNumTemplate();
            LoadCardColorTemplate();
        }

        private void LoadCardNumTemplate()
        {
            //加载牌大小模板
            var cardNums = new[]
                {
                    new KeyValuePair<CardNum, float>(CardNum._A, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._2, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._3, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._4, 0.86f), 
                    new KeyValuePair<CardNum, float>(CardNum._5, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._6, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._7, 0.892f), 
                    new KeyValuePair<CardNum, float>(CardNum._8, 0.889f),
                    new KeyValuePair<CardNum, float>(CardNum._9, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._10, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._J, 0.90f), 
                    new KeyValuePair<CardNum, float>(CardNum._Q, 0.835f),
                    new KeyValuePair<CardNum, float>(CardNum._K, 0.859f), 
                    new KeyValuePair<CardNum, float>(CardNum._Joke, 0.89f), 
                    new KeyValuePair<CardNum, float>(CardNum._BigJoke, 0.89f),
                    new KeyValuePair<CardNum, float>(CardNum._Any, 0.889f)
                };

            cardNums.ToList().ForEach(cardNum =>
                {
                    var bmps = new List<Bitmap>();

                    //对应数字的图像模板目录为：模板总目录+数字名称命名的文件夹
                    Directory.EnumerateFiles(TemplateDir + cardNum.Key.ToString())
                             .ToList()
                             .ForEach(file => bmps.Add(Image.FromFile(file) as Bitmap));

                    _cardNumMatchInfos.Add(new CardNumMatchInfo
                        {
                            Bitmaps = bmps,
                            Num = cardNum.Key,
                            SimilaSimilarityThreshold = cardNum.Value
                        });
                });
        }

        private void LoadCardColorTemplate()
        {
            //加载牌花色模板
            var cardColors = new[]
                {
                    new KeyValuePair<CardColor, float>(CardColor.方块, 0.9f), 
                    new KeyValuePair<CardColor, float>(CardColor.梅花, 0.9f), 
                    new KeyValuePair<CardColor, float>(CardColor.红桃, 0.9f), 
                    new KeyValuePair<CardColor, float>(CardColor.黑桃, 0.9f)
                };

            cardColors.ToList().ForEach(cardColor =>
                {
                    var bmps = new List<Bitmap>();

                    //对应花色的图像模板目录为：模板总目录+花色名称命名的文件夹
                    Directory.EnumerateFiles(TemplateDir + cardColor.Key.ToString())
                             .ToList()
                             .ForEach(file => bmps.Add(Image.FromFile(file) as Bitmap));

                    _cardColorMatchInfos.Add(new CardColorMatchInfo
                        {
                            Bitmaps = bmps,
                            Color = cardColor.Key,
                            SimilaSimilarityThreshold = cardColor.Value
                        });
                });
        }

        public List<CardNumMatchInfo> GetCardNumTemplate()
        {
            return _cardNumMatchInfos;
        }

        public List<CardColorMatchInfo> GetCardColorTemplate()
        {
            return _cardColorMatchInfos;
        }
    }
}
