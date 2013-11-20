using System.Collections.Generic;
using GXService.CardRecognize.Contract;

namespace GXService.CardRecognize.Service.Factory
{
    class TemplateFactory
    {
        public static TemplateFactory Singleton = new TemplateFactory();
        private static readonly Dictionary<GameTemplateType, ITemplate> Templates = new Dictionary<GameTemplateType, ITemplate>(); 

        private TemplateFactory()
        {}

        public ITemplate GetTemplate(GameTemplateType type)
        {
            if (!Templates.ContainsKey(type))
            {
                switch (type)
                {
                    case GameTemplateType.斗地主手牌:
                        Templates.Add(type, new DouDiZhuTemplate());
                        break;

                    case GameTemplateType.斗地主出牌:
                        Templates.Add(type, new DDZThrowTemplate());
                        break;

                    case GameTemplateType.十三张:
                        Templates.Add(type, new ThirteenCardsTemplate());
                        break;
                }
            }

            return Templates[type];
        }
    }
}
