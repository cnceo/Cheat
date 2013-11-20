using System.Collections.Generic;
using System.Drawing;
using GXService.CardRecognize.Contract;

namespace GXService.CardRecognize.Service.Factory
{
    public interface ITemplate
    {
        List<CardNumMatchInfo> GetCardNumTemplate();
        List<CardColorMatchInfo> GetCardColorTemplate();
    }

    public class CardNumMatchInfo
    {
        public CardNum Num { get; set; }
        public float SimilaSimilarityThreshold { get; set; }
        public List<Bitmap> Bitmaps { get; set; }
    }

    public class CardColorMatchInfo
    {
        public CardColor Color { get; set; }
        public float SimilaSimilarityThreshold { get; set; }
        public List<Bitmap> Bitmaps { get; set; }
    }
}
