using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Game_Project.Form1;

namespace Game_Project
{
    public class Game // Holds all of the data on the current game
    {
        public List<Card> plr1Cards;
        public List<Card> plr2Cards;

        public int plr1Score;
        public int plr2Score;

        public List<Card> deck;
        public List<Card> table;
    }

    public class Card
    {
        public string type;
        public int score;
        public string colour;
    }
    public class ai_test
    {
        public List<List<Card>> GetCombinations(List<Card> cards)
        {
            List<List<Card>> combis = new List<List<Card>>();
            for (int i = 1; i < cards.Count+1; i++)
            {
                List<List<Card>> toAdd = GetCombisHandler(cards, new List<List<Card>>(), i, 1, new List<Card>);
                foreach (List<Card> list in toAdd)
                {
                    combis.Add(list);
                }
            }
            return combis;
        }

        private List<List<Card>> GetCombisHandler(List<Card> allCards, List<List<Card>> cCombis, int totDepth, int cDepth, List<Card> cList)
        {
            if (totDepth == cDepth)
            {
                foreach (Card card in allCards)
                {
                    
                }
                return cCombis;
            } else
            {
                foreach (Card card in allCards)
                {
                    
                }
            }
        }

        public object[] RunAI() // Returns {"decision", cardI}
        {
            Game game = new Game();
            List<object[]> possibleGames = new List<object[]>(); // {move, score}

            // Add a card to the deck
            object[] cGame = new object[2];
            cGame[0] = "Add Card";
            int addScore = 0;
            int playerCards = 0; // Number of cards of same colour as top of deck that opponent has
            foreach (Card card in game.plr1Cards)
            {
                if (card.colour == game.deck[0].colour)
                {
                    playerCards++;
                }
            }
            addScore -= 5 * (playerCards - 1) < 0? 0: 5 * (playerCards - 1);

            int AICards = 0; // Number of cards of same colour as top of deck that AI has
            foreach (Card card in game.plr2Cards)
            {
                if (card.colour == game.deck[0].colour)
                {
                    AICards++;
                }
            }
            addScore += 5 * (AICards - 1) < 0 ? 0 : 5 * (AICards - 1);

            cGame[1] = addScore;
            possibleGames.Add(cGame);

            // Buying a card

            for (int i = 0; i < game.table.Count; i++)
            {
                Card card = game.table[i];
                if (game.plr2Score < card.score)
                {
                    continue; // AI doesn't have enough money to buy it so don't add it as an option
                }
                int qnty = 2;
                int cBuyScore = 0;
                if (game.table.Count == 5) // Only if a card will be added back to the table on purchase of a card
                {
                    cBuyScore -= 5 * (playerCards - 1) < 0 ? 0 : 5 * (playerCards - 1); // Remove some points based on what it would add to the shop for the other player

                    cBuyScore += 5 * (AICards - 1) < 0 ? 0 : 5 * (AICards - 1); // Add point based on what it would add to the shop for the AI

                    qnty++;
                }

                cBuyScore += 5 * (AICards - 1) < 0 ? 0 : 5 * (AICards - 1); // Add points based on how much it would benefit AI to have the card

                cBuyScore += 5 * (playerCards - 1) < 0 ? 0 : 5 * (playerCards - 1); // Add points based on how much it would hinder the opponent for not having it

                int avg = cBuyScore / qnty;

                cGame = new object[] { new object[] { "Buy", i }, avg };
                possibleGames.Add(cGame);
            }

            // Selling cards
            List<Card> redCards = new List<Card>();
            List<Card> greenCards = new List<Card>();
            List<Card> yellowCards = new List<Card>();
            List<Card> purpleCards = new List<Card>();

            foreach (Card card in game.plr2Cards)
            {
                switch (card.colour)
                {
                    case "Red":
                        redCards.Add(card); break;
                    case "Green":
                        greenCards.Add(card); break;
                    case "Yellow":
                        yellowCards.Add(card); break;
                    case "Purple":
                        purpleCards.Add(card); break;
                }
            }

            if (redCards.Count > 0)
            {
                // Generate all possible combinations of selling red cards
                List<List<Card>> redCombis = new List<List<Card>>();
                for (int len = 1; len < redCards.Count+1; len++)
                {
                    List<Card> cList = new List<Card>();
                    for (int i = 0; i < len; i++)
                    {
                        foreach (Card card in redCards)
                        {
                            if (!cList.Contains(card))
                            {
                                cList.Add(card);
                            }
                        }
                        if (i == len-1)
                        {

                        }
                    }

                }
            }
            // Maybe just look one step into the future since we "can't" tell which cards are going to be on top of the deck next


            return new object[] { "Add Card" };
        }
    }
}
