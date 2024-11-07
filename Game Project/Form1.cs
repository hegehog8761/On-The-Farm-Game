using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class Form1 : Form
    {
        // Global variables
        static Game currentGame;
        static Form form;
        static GameUI gameUI;
        static Random rand = new Random();

        // Classes are sorted by alphabetical order.

        public class AI
        {
            public object[] Run() // Returns {"decision", cardI}
            {
                List<object[]> possibleGames = new List<object[]>(); // {move, score}

                // Add a card to the deck
                object[] cGame = new object[2];
                cGame[0] = "Add Card";
                int addScore = 0;
                int playerCards = 0; // Number of cards of same colour as top of deck that opponent has
                foreach (Card card in currentGame.plr1Cards)
                {
                    if (card.colour == currentGame.deck[0].colour)
                    {
                        playerCards++;
                    }
                }
                addScore -= 5 * (playerCards - 1) < 0 ? 0 : 5 * (playerCards - 1); // Remove some points based on how much it would benefit the player to have the card on the top of the deck

                int AICards = 0; // Number of cards of same colour as top of deck that AI has
                foreach (Card card in currentGame.plr2Cards)
                {
                    if (card.colour == currentGame.deck[0].colour)
                    {
                        AICards++;
                    }
                }
                addScore += 5 * (AICards - 1) < 0 ? 0 : 5 * (AICards - 1); // Add some points based off of how much it would benefit the AI to have the card on top of the deck

                cGame[1] = addScore; // Current game's score 
                possibleGames.Add(cGame);

                // Buying a card

                for (int i = 0; i < currentGame.table.Count; i++)
                {
                    Card card = currentGame.table[i];
                    if (currentGame.plr2Score < card.score)
                    {
                        continue; // AI doesn't have enough money to buy it so don't add it as an option
                    }
                    int qnty = 2;
                    int cBuyScore = 0;
                    if (currentGame.table.Count == 5) // Only if a card will be added back to the table on purchase of a card
                    {
                        cBuyScore -= 5 * (playerCards - 1) < 0 ? 0 : 5 * (playerCards - 1); // Remove some points based on what it would add to the shop for the other player

                        cBuyScore += 5 * (AICards - 1) < 0 ? 0 : 5 * (AICards - 1); // Add point based on what it would add to the shop for the AI

                        qnty++;
                    }

                    cBuyScore += 5 * (AICards - 1) < 0 ? 0 : 5 * (AICards - 1); // Add points based on how much it would benefit AI to have the card

                    cBuyScore += 5 * (playerCards - 1) < 0 ? 0 : 5 * (playerCards - 1); // Add points based on how much it would hinder the opponent for not having it

                    int avg = cBuyScore / qnty;

                    cGame = new object[] { new object[] { "Buy Card", i }, avg };
                    possibleGames.Add(cGame);
                }

                // Selling cards
                List<Card> redCards = new List<Card>();
                List<Card> greenCards = new List<Card>();
                List<Card> yellowCards = new List<Card>();
                List<Card> purpleCards = new List<Card>();

                foreach (Card card in currentGame.plr2Cards)
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

                // Add actual algorithms for selling scores, need to prioritise selling cards when the deck is down to 5-10 cards
                // Maybe even reduce bias towards buying cards towards the end of the game and just add cards if sell out too soon

                foreach (List<Card> colourList in new List<List<Card>>() { redCards, greenCards, yellowCards, purpleCards })
                {
                    if (colourList.Count > 0)
                    {
                        int colourScore = 5 * (colourList.Count - 1) < 0 ? 0 : 5 * (colourList.Count - 1);
                        int[] colourIs = new int[colourList.Count];
                        for (int i = 0; i < colourList.Count; i++)
                        {
                            colourIs[i] = currentGame.plr2Cards.IndexOf(colourList[i]);
                        }
                        possibleGames.Add(new object[] { new object[] { "Sell Cards", colourIs }, colourScore });
                    }
                }

                // Sort the options best to worse 
                bool changed = true;
                while (changed)
                {
                    changed = false;
                    for (int i = 0; i < possibleGames.Count - 1; i++)
                    {
                        if ((int)possibleGames[i][1] < (int)possibleGames[i + 1][1])
                        {
                            changed = true;
                            object[] oldVal = possibleGames[i];
                            possibleGames[i] = possibleGames[i + 1];
                            possibleGames[i + 1] = oldVal;
                        }
                    }
                }
                if (possibleGames[0][0] == "Add Card")
                {
                    return new object[] { "Add Card" };
                }
                else
                {
                    return (object[])possibleGames[0][0];
                }
            }
        }

        public class Card
        {
            public string type;
            public int score;
            public string colour;

            public Card(string t, int s, string c) // Create a new card with preset values
            {
                type = t;
                score = s;
                colour = c;
            }

            public Card() // Create a new card with random values
            {
                type = (string)GetRandomArrayItem(new string[] { "Cow", "Hay", "Carrots", "Dead Crops", "Wasabi", "Potatoes" });
                int[] scoreRange = new int[2]; // Different cards have a random different score within a range based on its type
                switch (type)
                {
                    case "Cow":
                        scoreRange = new int[] { 100, 150 };
                        break;
                    case "Hay":
                        scoreRange = new int[] { 5, 10 };
                        break;
                    case "Carrots":
                        scoreRange = new int[] { 15, 20 };
                        break;
                    case "Dead Crops":
                        scoreRange = new int[] { 1, 3 };
                        break;
                    case "Wasabi":
                        scoreRange = new int[] { 40, 50 };
                        break;
                    case "Potatoes":
                        scoreRange = new int[] { 5, 30 };
                        break;
                }
                score = rand.Next(scoreRange[0], scoreRange[1] + 1);
                colour = (string)GetRandomArrayItem(new string[] { "Purple", "Green", "Red", "Yellow" });
            }
        }

        public class Game // Holds all of the data on the current game
        {
            #region Variables
            public List<Card> plr1Cards;
            public List<Card> plr2Cards;

            public int plr1Score;
            public int plr2Score;

            public List<Card> deck;
            public List<Card> table;

            List<int> plr1SellingIndexes;

            public bool playing = true;
            #endregion

            public Game()
            {
                // Start new game with random values
                plr1Cards = new List<Card>();
                plr2Cards = new List<Card>();
                plr1Score = 0;
                plr2Score = 0;
                table = new List<Card>();
                deck = new List<Card>();
                // Generate 50 cards to add to the deck

                for (int i = 0; i < 50; i++)
                {
                    deck.Add(new Card());
                }

                // Give each player 5 cards
                for (int i = 0; i < 5; i++)
                {
                    DrawCardDeck(1);
                }
                for (int i = 0; i < 5; i++)
                {
                    DrawCardDeck(2);
                }

                // Add 5 cards to the shop
                FillShop();

            }

            public Game(List<Card> p1C, List<Card> p2C, int p1S, int p2S, List<Card> d, List<Card> t)
            {
                // For loading games which have already been set up
                plr1Cards = p1C;
                plr2Cards = p2C;
                plr1Score = p1S;
                plr2Score = p2S;
                deck = d;
                table = t;
            }

            public void DrawCardDeck(int plr)
            {
                switch (plr)
                {
                    case 1:
                        plr1Cards.Add(deck[0]);
                        break;
                    case 2:
                        plr2Cards.Add(deck[0]);
                        break;
                }
                deck.RemoveAt(0);
            }

            public void FillShop()
            {
                while (table.Count < 5)
                {
                    table.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }

            public void PlayAdd(object sender, EventArgs e)
            {
                // Called only by the player's button press
                Add();
                gameUI.Draw();
                AITurn();
            }

            public void PlayAdd()
            {
                // Called only by the AI's desision
                Add();
                gameUI.Draw();
                PlayerTurn();
            }

            public void Add()
            {
                // Sub function to actually add a card to the table
                table.Add(deck[0]);
                deck.RemoveAt(0);
            }

            public void BuyHandler(object sender, EventArgs e)
            {
                Button pressed = (Button)sender;
                string t = pressed.Text.Split('\n')[0];
                int score = int.Parse(pressed.Text.Split('\n')[1].Trim('£'));
                int cardIndex = -1;
                for (int i = 0; i < currentGame.table.Count; i++)
                {
                    if (currentGame.table[i].type == t && currentGame.table[i].score == score)
                    {
                        cardIndex = i;
                        break;
                    }
                }
                if (cardIndex == -1)
                {
                    throw new Exception("Card selected couldn't be found in table list of cards");
                }
                currentGame.plr1Score -= currentGame.table[cardIndex].score;
                currentGame.plr1Cards.Add(currentGame.table[cardIndex]);
                currentGame.table.RemoveAt(cardIndex);
                FillShop();
                gameUI.Draw();
                // Only player's button press will ever call this
                AITurn();
            }

            public void SellHandler(object sender, EventArgs e)
            {
                if (plr1SellingIndexes.Count == 0)
                {
                    MessageBox.Show("You can't sell 0 cards!");
                    return;
                }

                // Sort card indexes lowest to highest to make the offset function work
                bool changed = true;
                while (changed)
                {
                    changed = false;
                    for (int i = 0; i < plr1SellingIndexes.Count - 1; i++)
                    {
                        if (plr1SellingIndexes[i] > plr1SellingIndexes[i + 1])
                        {
                            changed = true;
                            int oldval = plr1SellingIndexes[i];
                            plr1SellingIndexes[i] = plr1SellingIndexes[i + 1];
                            plr1SellingIndexes[i + 1] = oldval;
                        }
                    }
                }

                int offset = 0;
                foreach (int index in plr1SellingIndexes)
                {
                    plr1Score += plr1Cards[index - offset].score;
                    plr1Cards.RemoveAt(index - offset);
                    offset++;
                }

                // Only a player's button press should ever trigger this function
                AITurn();
            }

            public void SellAddHandler(object sender, EventArgs e)
            {
                int cardIndex = gameUI.gamePlayerCards.Controls.IndexOf((Control)sender);

                if (cardIndex == -1)
                {
                    for (int i = 0; i < gameUI.gamePlayerCards.Controls.Count; i++)
                    {
                        if (gameUI.gamePlayerCards.Controls[i].Text == ((CheckBox)sender).Text)
                        {
                            cardIndex = i;
                        }
                    }
                    if (cardIndex == -1)
                    {
                        throw new Exception("Selected card could not be found in player 1's card list.");
                    }
                }

                int currentlySelected = 0;
                foreach (CheckBox box in gameUI.gamePlayerCards.Controls)
                {
                    if (box.Checked) { currentlySelected++; }
                }

                if (currentlySelected == 1 && ((CheckBox)sender).Checked)
                {
                    // First box to be selected
                    Color colour = ((CheckBox)sender).BackColor;
                    foreach (CheckBox box in gameUI.gamePlayerCards.Controls)
                    {
                        box.Enabled = box.BackColor == colour; // Enable on the butons of the same colour
                    }
                }

                if (currentlySelected == 0 && !((CheckBox)sender).Checked)
                {
                    // Last box to be deselcted
                    foreach (CheckBox box in gameUI.gamePlayerCards.Controls)
                    {
                        box.Enabled = true;
                    }
                }

                if (((CheckBox)sender).Checked)
                {
                    plr1SellingIndexes.Add(cardIndex);
                }
                else
                {
                    plr1SellingIndexes.Remove(cardIndex);
                }
            }

            public void PlayBuy(object sender, EventArgs e)
            {
                // Only ever called by player's button press
                #region Change UI to allow user to select card to buy

                gameUI.Draw(); // Clear the screen to return it to the natural state

                // Add the back button
                Button playBuyBack = new Button();
                playBuyBack.Text = "Back";
                playBuyBack.Location = new Point(238, 119);
                playBuyBack.Size = new Size(75, 23);
                playBuyBack.Click += PlayerTurn;

                form.Controls.Add(playBuyBack);

                // Convert all cards on table to buttons
                gameUI.gameTableList.Controls.Clear(); // Remove cards as labels from the table

                foreach (Card card in currentGame.table)
                {
                    Button cButton = new Button();
                    cButton.Text = $"{card.type}\n£{card.score}";

                    cButton.AutoSize = true;
                    cButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    cButton.BackColor = TextToColour(card.colour);
                    cButton.Font = new Font("Lucida Handwriting", (float)9);
                    cButton.Enabled = card.score <= currentGame.plr1Score; // Enable the button if player has enough money to purchase it
                    cButton.Click += BuyHandler;

                    gameUI.gameTableList.Controls.Add(cButton);
                }

                //// THIS IS ALL FOR THE SELLING FUNCTION, NOT BUYING
                // Draw sell button back on 
                // Change all buttons to selectable box
                // On click of check box if it's the only one, in that case disable all rest of non-same colour 
                // -> If it's only one clicked and it's unclicked then allow user to select any box 

                #endregion
            }

            public void PlayBuy(int cardI)
            {
                // Assume that since it's the AI it shouln't have chosen a card it doens't have enough money for
                Card purchasedCard = deck[cardI];
                deck.RemoveAt(cardI);
                FillShop();
                plr1Score -= purchasedCard.score;
                plr2Cards.Add(purchasedCard);
                gameUI.Draw();
                // Only the AI should ever run this function overflow
                PlayerTurn();
            }

            public void PlaySell(int[] cardIs)
            {
                int offset = 0; // When a card is sold the array is shorted by one so every card is shifted one to the left
                foreach (int cardI in cardIs)
                {
                    plr2Score += plr2Cards[cardI - offset].score;
                    plr2Cards.RemoveAt(cardI - offset);
                    offset++;
                }
                gameUI.Draw();
                // Only the AI should ever call this overflow
                PlayerTurn();
            }

            public void PlaySell(object sender, EventArgs e)
            {
                // Change UI to have back button and (confirm) sell button
                gameUI.Draw();

                Button playSellBack = new Button();
                playSellBack.Text = "Back";
                playSellBack.Location = new Point(238, 119);
                playSellBack.Size = new Size(75, 23);
                playSellBack.Click += PlayerTurn;

                Button playSellConfirm = new Button();
                playSellConfirm.Text = "Sell";
                playSellConfirm.Location = new Point(238, 147);
                playSellConfirm.Size = new Size(75, 23);
                playSellConfirm.Click += SellHandler;

                form.Controls.Add(playSellBack);
                form.Controls.Add(playSellConfirm);

                plr1SellingIndexes = new List<int>();

                // Convert all of the player's cards to select boxes
                gameUI.gamePlayerCards.Controls.Clear();
                foreach (Card card in currentGame.plr1Cards)
                {
                    CheckBox box = new CheckBox();
                    box.BackColor = TextToColour(card.colour);
                    box.Text = $"{card.type}\n£{card.score}";
                    box.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    box.AutoSize = true;
                    box.Font = new Font("Lucida Handwriting", (float)9);
                    box.Click += SellAddHandler;

                    gameUI.gamePlayerCards.Controls.Add(box);
                }
            }
        }

        public class GameUI
        {
            public Label gameTitle;
            public Label gamePlayerMoney;
            public Label gamePlayerCardsLabel;
            public FlowLayoutPanel gamePlayerCards;
            public Label gameDeckLabel;
            public Label gameDeckCard;
            public Label gameTableLabel;
            public FlowLayoutPanel gameTableList;
            public Button gameSaveGame;
            public Label gameOpponentCardsLabel;
            public FlowLayoutPanel gameOpponentCardsList;
            public Label deckLeftLabel;

            public GameUI()
            {
                Font labelFont = new Font("Lucida Handwriting", (float)9);

                // Title
                gameTitle = new Label();
                gameTitle.Font = new Font("Lucida Handwriting", (float)27.72, FontStyle.Underline);
                gameTitle.Text = "On The Farm";
                gameTitle.Location = new Point(239, 9);
                gameTitle.Size = new Size(293, 48);

                // Player Money
                gamePlayerMoney = new Label();
                gamePlayerMoney.AutoSize = true;
                gamePlayerMoney.Font = labelFont;
                gamePlayerMoney.Text = "Money: £";
                gamePlayerMoney.Location = new Point(12, 73);
                gamePlayerMoney.Size = new Size(82, 16);

                // Player cards label
                gamePlayerCardsLabel = new Label();
                gamePlayerCardsLabel.AutoSize = true;
                gamePlayerCardsLabel.Font = labelFont;
                gamePlayerCardsLabel.Text = "Your Cards:";
                gamePlayerCardsLabel.Location = new Point(12, 99);
                gamePlayerCardsLabel.Size = new Size(86, 16);

                // Player cards list
                gamePlayerCards = new FlowLayoutPanel();
                gamePlayerCards.Location = new Point(15, 119);
                gamePlayerCards.Size = new Size(218, 319);
                gamePlayerCards.BorderStyle = BorderStyle.FixedSingle;
                gamePlayerCards.BackColor = Color.White;
                gamePlayerCards.AutoScroll = true;


                // Deck label
                gameDeckLabel = new Label();
                gameDeckLabel.AutoSize = true;
                gameDeckLabel.Font = labelFont;
                gameDeckLabel.Location = new Point(373, 146);
                gameDeckLabel.Size = new Size(36, 13);
                gameDeckLabel.Text = "Deck:";

                // Deck top card colour
                gameDeckCard = new Label();
                gameDeckCard.Location = new Point(362, 172);
                gameDeckCard.Size = new Size(66, 97);

                // Table label
                gameTableLabel = new Label();
                gameTableLabel.AutoSize = true;
                gameTableLabel.Font = labelFont;
                gameTableLabel.Text = "Table:";
                gameTableLabel.Location = new Point(377, 321);
                gameTableLabel.Size = new Size(49, 16);

                // Table list
                gameTableList = new FlowLayoutPanel();
                gameTableList.Location = new Point(266, 340);
                gameTableList.Size = new Size(275, 83);
                gameTableList.BorderStyle = BorderStyle.FixedSingle;
                gameTableList.BackColor = Color.White;
                gameTableList.AutoScroll = true;


                // Save Game Button
                gameSaveGame = new Button();
                gameSaveGame.Text = "Save Game";
                gameSaveGame.Location = new Point(706, 9);
                gameSaveGame.Size = new Size(75, 23);
                gameSaveGame.Click += SaveToFile;

                // Opponent cards label
                gameOpponentCardsLabel = new Label();
                gameOpponentCardsLabel.AutoSize = true;
                gameOpponentCardsLabel.Font = labelFont;
                gameOpponentCardsLabel.Location = new Point(567, 107);
                gameOpponentCardsLabel.Size = new Size(119, 16);
                gameOpponentCardsLabel.Text = "Opponent's Cards:";

                // Opponent cards list
                gameOpponentCardsList = new FlowLayoutPanel();
                gameOpponentCardsList.Location = new Point(570, 127);
                gameOpponentCardsList.Size = new Size(218, 311);
                gameOpponentCardsList.BorderStyle = BorderStyle.FixedSingle;
                gameOpponentCardsList.BackColor = Color.White;
                gameOpponentCardsList.AutoScroll = true;

                // Number of cards left on the deck label
                deckLeftLabel = new Label();
                deckLeftLabel.AutoSize = false;
                deckLeftLabel.Font = labelFont;
                deckLeftLabel.Location = new Point(347, 274);
                deckLeftLabel.Size = new Size(98, 16);
                deckLeftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                deckLeftLabel.Text = $"Cards left: {currentGame.deck.Count}";
            }

            public void Draw()
            {
                form.Controls.Clear();

                form.Controls.Add(gameTitle);
                form.Controls.Add(gamePlayerMoney);
                form.Controls.Add(gamePlayerCardsLabel);
                form.Controls.Add(gamePlayerCards);
                form.Controls.Add(gameDeckLabel);
                form.Controls.Add(gameDeckCard);
                form.Controls.Add(gameTableLabel);
                form.Controls.Add(gameTableList);
                form.Controls.Add(gameSaveGame);
                form.Controls.Add(gameOpponentCardsLabel);
                form.Controls.Add(gameOpponentCardsList);
                form.Controls.Add(deckLeftLabel);

                Update();
            }

            public void Update()
            {

                gamePlayerMoney.Text = $"Money: £{currentGame.plr1Score}";

                gamePlayerCards.Controls.Clear();
                for (int i = 0; i < currentGame.plr1Cards.Count; i++)
                {
                    Label label = new Label();
                    label.AutoSize = true;
                    label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    label.Text = $"{currentGame.plr1Cards[i].type}\n£{currentGame.plr1Cards[i].score}";
                    label.Font = new Font("Lucida Handwriting", (float)8);
                    label.Margin = new Padding(3);
                    label.BackColor = TextToColour(currentGame.plr1Cards[i].colour);

                    gamePlayerCards.Controls.Add(label);
                }

                if (currentGame.deck.Count > 0)
                {
                    gameDeckCard.BackColor = TextToColour(currentGame.deck[0].colour);
                }
                else
                {
                    EndGame();
                }

                gameTableList.Controls.Clear();
                for (int i = 0; i < currentGame.table.Count; i++)
                {
                    Label label = new Label();
                    label.AutoSize = true;
                    label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    label.Text = $"{currentGame.table[i].type}\n£{currentGame.table[i].score}";
                    label.Font = new Font("Lucida Handwriting", (float)8);
                    label.Margin = new Padding(3);
                    label.BackColor = TextToColour(currentGame.table[i].colour);

                    gameTableList.Controls.Add(label);
                }

                gameOpponentCardsList.Controls.Clear();
                for (int i = 0; i < currentGame.plr2Cards.Count; i++)
                {
                    Label label = new Label();
                    label.AutoSize = false;
                    label.Margin = new Padding(3);
                    label.Size = new Size(25, 39);
                    label.BackColor = TextToColour(currentGame.plr2Cards[i].colour);

                    gameOpponentCardsList.Controls.Add(label);
                }

                deckLeftLabel.Text = $"Cards left: {currentGame.deck.Count}";
            }

            public void EndGame()
            {
                currentGame.playing = false;
                form.Controls.Clear();


                int winner = currentGame.plr1Score > currentGame.plr2Score ? 1 : 2;
                winner = currentGame.plr1Score == currentGame.plr2Score ? 0 : winner;

                Label winnerBox = new Label();
                winnerBox.AutoSize = false;
                winnerBox.Text = winner == 0 ? "Everyone Won!" : $"Player {winner} won!";
                winnerBox.Font = new Font("Lucida Handwriting", (float)72);
                winnerBox.Location = new Point(5, 156);
                winnerBox.Size = new Size(783, 124);
                winnerBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                form.Controls.Add(winnerBox);
            }
        }

        // Functions are alphabetically sorted

        public static void AITurn()
        {
            if (currentGame.playing)
            {
                AI mainAI = new AI();
                object[] decision = mainAI.Run();
                if (decision.Length == 1 && decision[0] == "Add Card")
                {
                    currentGame.PlayAdd();
                }
                else if (decision[0] == "Buy Card")
                {
                    currentGame.PlayBuy((int)decision[1]);
                }
                else if (decision[0] == "Sell Cards")
                {
                    currentGame.PlaySell((int[])decision[1]);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form = this;
            MainMenu();
        }

        public static object GetRandomArrayItem(object[] list) // Select a random object from a given array
        {
            int randChoice = rand.Next(0, list.Length);
            return list[randChoice];
        }

        public static void LoadFromFile(object sender, EventArgs e)
        {
            // Get user to select file to attempt to load from
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "OTF Save Files|*.otfsave";
            string data;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                data = File.ReadAllText(ofd.FileName);
            }
            else
            {
                return;
            }

            // Ensure that the file selected is in the correct format
            string[] datLines = data.Split('\n');
            if (datLines.Length != 6)
            {
                MessageBox.Show("Error FR001\nCouldn't read file.");
                return;
            }

            foreach (string line in datLines)
            {
                if (!line.Trim((char)13).EndsWith(";;"))
                {
                    MessageBox.Show("Error FR002\nCouldn't read file.");
                    return;
                }
            }

            string[] startList = new string[] { "p1C:", "p2C:", "p1S:", "p2S:", "d:", "t:" };
            for (int i = 0; i < datLines.Length; i++)
            {
                string line = datLines[i];
                string start = startList[i];
                if (!line.StartsWith(start))
                {
                    MessageBox.Show("Error FR003\nCouldn't read file.");
                    return;
                }
            }

            //// Try to load the file
            // Player one cards
            string cardString;
            try
            {
                cardString = datLines[0].Substring(4);
                cardString = cardString.Substring(0, cardString.Length - 2);
            }
            catch
            {
                MessageBox.Show("Error FR004\nCouldn't read file.");
                return;
            }
            string[] cardsStrings = cardString.Split(';');
            List<Card> p1Cards = new List<Card>();
            foreach (string cardsString in cardsStrings)
            {
                try
                {
                    if (cardsString != "")
                    {
                        if (cardsString != "")
                        {
                            string[] cardData = cardsString.Split(',');
                            p1Cards.Add(new Card(cardData[0], int.Parse(cardData[1]), cardData[2]));
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Error FR005\nCouldn't read file.\n\n\nError:{err.ToString()}");
                    return;
                }
            }

            // Player two cards
            try
            {
                cardString = datLines[1].Substring(4);
                cardString = cardString.Substring(0, cardString.Length - 2);
            }
            catch
            {
                MessageBox.Show("Error FR004\nCouldn't read file.");
                return;
            }
            cardsStrings = cardString.Split(';');
            List<Card> p2Cards = new List<Card>();
            foreach (string cardsString in cardsStrings)
            {
                try
                {
                    if (cardsString != "")
                    {
                        string[] cardData = cardsString.Split(',');
                        p2Cards.Add(new Card(cardData[0], int.Parse(cardData[1]), cardData[2]));
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Error FR005\nCouldn't read file.\n\n\nError:{err.ToString()}");
                    return;
                }
            }

            // Player one score
            string scoreString;
            try
            {
                scoreString = datLines[2].Substring(4);
                scoreString = scoreString.Split(';')[0];
            }
            catch
            {
                MessageBox.Show("Error FR006\nCouldn't read file.");
                return;
            }
            int p1Score;
            try
            {
                p1Score = int.Parse(scoreString);
            }
            catch
            {
                MessageBox.Show("Error FR007\nCouldn't read file.");
                return;
            }

            // Player two score
            try
            {

                scoreString = datLines[3].Substring(4);
                scoreString = scoreString.Split(';')[0];
            }
            catch
            {
                MessageBox.Show("Error FR006\nCouldn't read file.");
                return;
            }
            int p2Score;
            try
            {
                p2Score = int.Parse(scoreString);
            }
            catch
            {
                MessageBox.Show("Error FR007\nCouldn't read file.");
                return;
            }

            // Cards in the deck
            try
            {
                cardString = datLines[4].Substring(2);
                cardString = cardString.Substring(0, cardString.Length - 2);
            }
            catch
            {
                MessageBox.Show("Error FR004\nCouldn't read file.");
                return;
            }
            cardsStrings = cardString.Split(';');
            List<Card> deck = new List<Card>();
            foreach (string cardsString in cardsStrings)
            {
                try
                {
                    if (cardsString != "")
                    {
                        string[] cardData = cardsString.Split(',');
                        deck.Add(new Card(cardData[0], int.Parse(cardData[1]), cardData[2]));
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Error FR005\nCouldn't read file.\n\n\nError:{err.ToString()}");
                    return;
                }
            }

            // Cards on the table
            try
            {
                cardString = datLines[5].Substring(2);
                cardString = cardString.Substring(0, cardString.Length - 2);
            }
            catch
            {
                MessageBox.Show("Error FR004\nCouldn't read file.");
                return;
            }
            cardsStrings = cardString.Split(';');
            List<Card> table = new List<Card>();
            foreach (string cardsString in cardsStrings)
            {
                try
                {
                    if (cardsString != "")
                    {
                        string[] cardData = cardsString.Split(',');
                        table.Add(new Card(cardData[0], int.Parse(cardData[1]), cardData[2]));
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Error FR005\nCouldn't read file.\n\n\nError:{err.ToString()}");
                    return;
                }
            }

            // Create the game object and pass it through to the playing function
            currentGame = new Game(p1Cards, p2Cards, p1Score, p2Score, deck, table);
            PlayGame();
        }

        public static void MainMenu()
        {
            form.Controls.Clear(); // Ensure the screen is blank before drawing to it

            // Title
            Label menuTitle = new Label();
            menuTitle.Font = new Font("Lucida Handwriting", (float)27.72, FontStyle.Underline);
            menuTitle.Text = "On The Farm";
            menuTitle.Location = new Point(239, 9);
            menuTitle.Size = new Size(293, 48);

            // Load game button
            Button menuLoad = new Button();
            menuLoad.Text = "Load game from file";
            menuLoad.Location = new Point(74, 213);
            menuLoad.Size = new Size(148, 42);
            menuLoad.Click += LoadFromFile;

            // New game button
            Button menuNew = new Button();
            menuNew.Text = "Start new game";
            menuNew.Location = new Point(594, 213);
            menuNew.Size = new Size(148, 42);
            menuNew.Click += NewGame;


            // Add all elements to screen
            form.Controls.Add(menuTitle);
            form.Controls.Add(menuLoad);
            form.Controls.Add(menuNew);
        }

        public static void NewGame(object sender, EventArgs e)
        {
            currentGame = new Game();
            PlayGame();
        }

        public static void PlayerTurn()
        {
            if (currentGame.playing)
            {
                gameUI.Draw();
                #region Option Buttons

                Button gameBuy = new Button();
                gameBuy.Text = "Buy";
                gameBuy.Location = new Point(238, 119);
                gameBuy.Size = new Size(75, 23);
                gameBuy.Click += currentGame.PlayBuy;


                Button gameSell = new Button();
                gameSell.Text = "Sell";
                gameSell.Location = new Point(238, 147);
                gameSell.Size = new Size(75, 23);
                gameSell.Click += currentGame.PlaySell;

                Button gameAdd = new Button();
                gameAdd.Text = "Add";
                gameAdd.Location = new Point(238, 175);
                gameAdd.Size = new Size(75, 23);
                gameAdd.Click += currentGame.PlayAdd;


                form.Controls.Add(gameBuy);
                form.Controls.Add(gameSell);
                form.Controls.Add(gameAdd);
                #endregion
            }
        }

        public static void PlayerTurn(object sender, EventArgs e)
        {
            PlayerTurn();
        }

        public static void PlayGame()
        {
            gameUI = new GameUI();
            gameUI.Draw();

            PlayerTurn();
        }

        public static void SaveToFile(object sender, EventArgs e)
        {
            string outString = "";
            // Player 1 cards
            outString += "p1C:";
            foreach (Card card in currentGame.plr1Cards)
            {
                outString += $"{card.type},{card.score},{card.colour};";
            }
            outString += ";\n";

            // Player 2 cards
            outString += "p2C:";
            foreach (Card card in currentGame.plr2Cards)
            {
                outString += $"{card.type},{card.score},{card.colour};";
            }
            outString += ";\n";

            // Player scores
            outString += $"p1S:{currentGame.plr1Score};;\n";
            outString += $"p2S:{currentGame.plr2Score};;\n";

            // Deck cards:
            outString += "d:";
            foreach (Card card in currentGame.deck)
            {
                outString += $"{card.type},{card.score},{card.colour};";
            }
            outString += ";\n";

            // Table cards:
            outString += "t:";
            foreach (Card card in currentGame.table)
            {
                outString += $"{card.type},{card.score},{card.colour};";
            }
            outString += ";";

            // Prompt user to select save location
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "OTF Save Files|*.otfsave";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter stream = new StreamWriter(sfd.FileName, false);
                stream.Write(outString);
                stream.Close();
            }
        }

        public static Color TextToColour(string colour)
        {
            switch (colour)
            {
                case "Red": // Red, Green, Yellow, Purple
                    return Color.Red;
                case "Green":
                    return Color.Green;
                case "Yellow":
                    return Color.Yellow;
                case "Purple":
                    return Color.Purple;
            }
            return Color.White;
        }
    }
}
