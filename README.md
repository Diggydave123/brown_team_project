# brown_team_project

IBM Skills Build: Educational Card Game
This project is a prototype for an educational strategy card game that merges interactive gameplay with real-world learning. Developed in collaboration with IBM's Skills Build platform, the game is inspired by Star Trek Adversaries and aims to make learning AI, Cybersecurity, Data Analytics, and Threat Intelligence fun, engaging, and adaptive.

Players collect, upgrade, and battle with cards—unlocking boosts and new abilities by correctly answering quiz questions drawn from official IBM Skills Build badges. The experience is designed to reinforce learning through rewards and immediate feedback, creating a unique fusion of gaming and education.

⚠️ Disclaimer:
This version is a prototype intended for academic and demonstration purposes. While several features such as multiplayer and quiz-driven progression are functional, others like adaptive AI difficulty, feedback explanations, and progression systems are planned for future development.


INSTRUCTIONS FOR STARTING AND PLAYING THE GAME:

1. Watch the Demo Video
Before playing, we recommend watching the demonstration video to see how the prototype works.
If anything remains unclear after watching, please return to this document for more detailed guidance.

2. Running the Game Locally
To start the game:

- Open the project in Unity.
- Go to File → Build and Run (or press Ctrl + B).
- If you're testing alone, you must build and run the game twice to simulate both player instances in multiplayer mode.

3. Turn Management
To end your turn or pass to the opponent, press the "END TURN" button. This must be done every time a player finishes their turn.

At the beginning of your turn, you may need to press "END TURN" once or twice to move past the block phase and enter the control phase, where you regain full command over your cards.

4. Card Mechanics
- To place a card from hand to field: Drag and drop the card into the play area.
- To attack with a card: Click the card once it's on the field.
- To block/defend: Drag and drop your card on top of the attacking enemy card.

Note: Once used, cards become "flat-footed" (exhausted) and cannot be used again until reactivated.

5. Flat-Footed Rule
When a Monster card is placed on the field, it becomes flat-footed and cannot attack or block during the same turn it is summoned.

6. Mana Cards
When a Mana card is used, it moves to the mana deck.

The mana value of a card is shown in the top-right corner of each card.

Cards require sufficient mana to be played.

7. Spell Cards (Question Cards)
Spell cards trigger a multiple-choice question from the IBM Skills Build question bank.

- If answered correctly, a positive effect is applied (e.g., boost or healing).

- If answered incorrectly, a negative effect is applied.

Visual feedback is provided:

- Green for a correct answer

- Red for the selected incorrect answer

8. Damage Calculation
When an attacking card is blocked, damage is calculated as follows:

- If attacker’s attack > defender’s defense: The defending card is sent to the graveyard, and the difference is dealt to the player’s life points.

- If attacker’s attack < defender’s defense: The attacking card is destroyed and sent to the graveyard; no damage is dealt.

- If attacker’s attack = defender’s defense: Both cards are destroyed and sent to the graveyard.

9. Game Over
The game ends when a player’s life points reach 0.
At that point, the system returns to the main menu automatically by pressing continue.
	
