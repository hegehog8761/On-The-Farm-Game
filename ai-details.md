# AI details #
### Uses a minimax algorithm ###
- Looks 5 steps into the future (ie: AI options, player options, AI, player, AI)
- A turn can consist of a player:
  - Adding a card to the shop
  - Selling any one of their cards
  - Buying any one of the cards in the shop
## Adding to the shop ##
- Only one outcome for this, colour on top of deck is added to the shop, we don't know type or value
- Rank this on how many of the same colour opponent has and how many AI has
- Rank other player's colour being added to vs AI's being added to by how many player and AI of same colour already has
## Selling cards ##
- Player could sell any combination of their cards
- One of a colour
- All of same colour
- Any amount of same colour cards imbetween
- Any colour for above
- Best to probably sell as many of same colur in one turn
## Buying cards ##
- PLayer can buy any one (but only one) of the cards in the shop
- Best to stop opponent from getting lots of the same colour card
- Best to get lots of the same colour card
