# Test save games

Tests load these with `GameHarness.Load("Name")`, which reads `NAME.SVE` and `NAME.MAP` (upper case,
because the game upper-cases save file names). To play one in the game, copy both files into your
Civilization folder and rename them to a save slot, for example `CIVIL0.SVE` and `CIVIL0.MAP`.

## BUGSCENARIOS

Units placed to reproduce unit movement, combat, transport, trade route and AI movement bugs. The tests in
`UnitMovementTests`, `AIUnitMovementTests` and `TurnTests` use these units.

| Units | Position | Action | Expected (original game) |
|---|---|---|---|
| Trireme #34, sentried Militia in Samarkand | 71,17 | Move the Trireme right | Two Militia sail with it |
| Carrier #37, Fighter #38 | 72,15 | Move the Carrier up-right | The Fighter is carried |
| Trireme #39, Militia #40 | 71,19 | Move the Trireme right | The Militia is carried |
| Fighter #41 | 74,16 | Fly up-right onto land | Keeps 9 of its 10 moves |
| Fighter #43, Carrier #42 | 74,18 / 75,18 | Fly the Fighter right onto the Carrier | Turn ends, fuel refilled |
| Bomber #44, barbarian Militia | 69,15 / 70,14 | Attack up-right | Bomber's turn ends |
| Cruiser #46, barbarian Militia | 67,13 / 68,13 | Attack right | Cruiser keeps 5 moves |
| Triremes #47 to #50 | 67,21 70,21 70,12 72,12 | End the turn | Never lost next to land |
| Caravan #51 (home Samarkand, already trades with Moscow) | 52,18 | Move down into Moscow | Still one trade route with Moscow |
| Battleship #52 | 52,26 | Move down-right | Only water revealed two squares away |
