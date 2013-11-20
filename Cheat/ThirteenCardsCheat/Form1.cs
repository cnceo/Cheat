using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThirteenCardsCheat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //MouseInputManager.Click(514, 396);

            var cardsTest = new CardsTest();
            var cards = cardsTest.GetNextPlayerCards(13);
            var parser = new CardTypeParser();
            var result = parser.Parse(cards);

            using (var fs = new FileStream("result.txt", FileMode.Truncate))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var resultDisplay = "";
                    cards.ForEach(card => resultDisplay += card.ToString());
                    sw.WriteLine(resultDisplay);

                    CardTypeResult best = null;
                    result.ForEach(ctr =>
                        {
                            if (best == null)
                            {
                                best = ctr;
                            }
                            else
                            {
                                best = best.Compare(ctr) >= 0 ? best : ctr;
                            }
                        });

                    var tmp = "";

                    best.CardTypeHead.GetCards().ToList().ForEach(card => tmp += card.ToString());
                    tmp += "(" + best.CardTypeHead.GetCardEmType() + ")   ";

                    best.CardTypeMiddle.GetCards()
                       .ToList()
                       .ForEach(card => tmp += card.ToString());
                    tmp += "(" + best.CardTypeMiddle.GetCardEmType() + ")   ";

                    best.CardTypeTail.GetCards().ToList().ForEach(card => tmp += card.ToString());
                    tmp += "(" + best.CardTypeTail.GetCardEmType() + ")";

                    sw.WriteLine(tmp);
                }
            }

            InitializeComponent();
        }
    }
}
