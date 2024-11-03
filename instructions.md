# Instructions #
## Setup ##
All of this should be handled by the code

At the start of a game:
- There should be 50 cards in the deck
- Draw 5 cards for player 1
- Draw 5 cards for player 2
- Draw 5 cards for the shop
- Each player should be able to see the full information of their cards
- Each player should also be able to see the colour and quantity of the other's cards
- Both players should be able to see the full information of the cards in the shop
- Both players should also be able to see the colour of the next card to be drawn from the deck
- Players should only be able to see how much money they have (and not the other player)
- Players should be able to see how many cards are left in the deck
## Rounds ##
- The player can choose to:
  - [Sell cards](#selling-cards)
  - [Buy a card](#buying-a-card)
  - [Add a card to the shop](#adding-a-card-to-the-shop)
- A player can only play one action one time per round
### Selling cards ###
- Player can sell as many cards of the same colour as they wish
- They can only sell cards of the same colour per round
- They do not have to sell all cards of the same colour but can if they wish
- Players gain a bonus for how many cards of the same colour they have
    - A bonus of 5 × cards - 1
### Buying a card ###
- Players can only buy one card at a time
- Players can only buy cards with equal or lesser value to their money (ie they cannot go into negative money but can have 0 and must pay for cards)
- When a card is bought the shop should be checked to have 5 or more cards in it, if it has less than 5 then cards should be added from the deck to bring it back up to 5 cards. If there remain 5 or more cards once a player has boought one nothing should be done
### Adding a card to the shop ###
- A player can choose to add the card from the top of the deck to the shop
## Ending the game ##
- The game ends when the deck completely runs out (not including the shop)
- The winner is the player with the most amount of money
- Once the deck is empty players do not play any more (and so cannot sell cards), all cards owned by a player which have not already been sold are discarded
