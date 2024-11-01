using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Data.SqlClient;

namespace Game_Project
{
    public partial class Form1 : Form
    {
        // Global variables
        static Random rand = new Random();
        static Form form;
        static Game currentGame;


        // Classes are sorted by alphabetical order.

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
            public List<Card> plr1Cards;
            public List<Card> plr2Cards;

            public int plr1Score;
            public int plr2Score;

            public List<Card> deck;
            public List<Card> table;

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
                    DrawCard(1);
                }
                for (int i = 0; i < 5; i++)
                {
                    DrawCard(2);
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

            public void DrawCard(int plr)
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
        }

        // Functions are alphabetically sorted

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form = this;
            MainMenu();
        }

        static public object GetRandomArrayItem(object[] list) // Select a random object from a given array
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
            } catch
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
                } catch (Exception err)
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
                        if (cardsString != "")
                        {
                            string[] cardData = cardsString.Split(',');
                            p2Cards.Add(new Card(cardData[0], int.Parse(cardData[1]), cardData[2]));
                        }
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
            } catch
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
                cardString = datLines[4].Substring(4);
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
                        if (cardsString != "")
                        {
                            string[] cardData = cardsString.Split(',');
                            deck.Add(new Card(cardData[0], int.Parse(cardData[1]), cardData[2]));
                        }
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
                cardString = datLines[4].Substring(4);
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

        public static void PlayGame()
        {
            #region UI Setup 
            // Set up UI
            form.Controls.Clear(); // Ensure that the screen is blank before drawing to it
            Font labelFont = new Font("Lucida Handwriting", (float)9);

            // Title
            Label gameTitle = new Label();
            gameTitle.Font = new Font("Lucida Handwriting", (float)27.72, FontStyle.Underline);
            gameTitle.Text = "On The Farm";
            gameTitle.Location = new Point(239, 9);
            gameTitle.Size = new Size(293, 48);

            // Player Money
            Label gamePlayerMoney = new Label();
            gamePlayerMoney.AutoSize = true;
            gamePlayerMoney.Font = labelFont;
            gamePlayerMoney.Text = "Money: £";
            gamePlayerMoney.Location = new Point(12, 73);
            gamePlayerMoney.Size = new Size(82, 16);

            // Player cards label
            Label gamePlayerCardsLabel = new Label();
            gamePlayerCardsLabel.AutoSize = true;
            gamePlayerCardsLabel.Font = labelFont;
            gamePlayerCardsLabel.Text = "Your Cards:";
            gamePlayerCardsLabel.Location = new Point(12, 99);
            gamePlayerCardsLabel.Size = new Size(86, 16);

            // Player cards list
            FlowLayoutPanel gamePlayerCards = new FlowLayoutPanel();
            gamePlayerCards.Location = new Point(15, 119);
            gamePlayerCards.Size = new Size(218, 319);
            gamePlayerCards.BorderStyle = BorderStyle.FixedSingle;
            gamePlayerCards.BackColor = Color.White;
            gamePlayerCards.AutoScroll = true;


            // Deck label
            Label gameDeckLabel = new Label();
            gameDeckLabel.AutoSize = true;
            gameDeckLabel.Font = labelFont;
            gameDeckLabel.Location = new Point(373, 146);
            gameDeckLabel.Size = new Size(36, 13);
            gameDeckLabel.Text = "Deck:";

            // Deck top card colour
            Label gameDeckCard = new Label();
            gameDeckCard.Location = new Point(362, 172);
            gameDeckCard.Size = new Size(66, 97);

            // Table label
            Label gameTableLabel = new Label();
            gameTableLabel.AutoSize = true;
            gameTableLabel.Font = labelFont;
            gameTableLabel.Text = "Table:";
            gameTableLabel.Location = new Point(377, 321);
            gameTableLabel.Size = new Size(49, 16);

            // Table list
            FlowLayoutPanel gameTableList = new FlowLayoutPanel();
            gameTableList.Location = new Point(266, 340);
            gameTableList.Size = new Size(275, 83);
            gameTableList.BorderStyle = BorderStyle.FixedSingle;
            gameTableList.BackColor = Color.White;
            gameTableList.AutoScroll = true;


            // Save Game Button
            Button gameSaveGame = new Button();
            gameSaveGame.Text = "Save Game";
            gameSaveGame.Location = new Point(706, 9);
            gameSaveGame.Size = new Size(75, 23);
            gameSaveGame.Click += SaveToFile;

            // Opponent cards label
            Label gameOpponentCardsLabel = new Label();
            gameOpponentCardsLabel.AutoSize = true;
            gameOpponentCardsLabel.Font = labelFont;
            gameOpponentCardsLabel.Location = new Point(567, 107);
            gameOpponentCardsLabel.Size = new Size(119, 16);
            gameOpponentCardsLabel.Text = "Opponent's Cards:";

            // Opponent cards list
            FlowLayoutPanel gameOpponentCardsList = new FlowLayoutPanel();
            gameOpponentCardsList.Location = new Point(570, 127);
            gameOpponentCardsList.Size = new Size(218, 311);
            gameOpponentCardsList.BorderStyle = BorderStyle.FixedSingle;
            gameOpponentCardsList.BackColor = Color.White;
            gameOpponentCardsList.AutoScroll = true;


            // Draw all components to the screen
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

            #endregion //UI Setup

            PlayerTurn();
        }

        public static void PlayerTurn()
        {
            #region Update UI Elements

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

            gameDeckCard.BackColor = TextToColour(currentGame.deck[0].colour);

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

            playing = false;

            #endregion // Update UI Elements

            #region Option Buttons

            Button gameBuy = new Button();
            gameBuy.Text = "Buy";
            gameBuy.Location = new Point();


            Button gameSell = new Button();
            gameSell.Text = "Sell";

            Button gameAdd = new Button();
            gameAdd.Text = "Add";

            #endregion
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

        public static void NewGame(object sender, EventArgs e)
        {
            currentGame = new Game();
            PlayGame();
        }

        static public void MainMenu()
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
    }
}
