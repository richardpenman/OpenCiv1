## First stage project road map

<ul>
<li>Reaching the initial playability of the game (passed),</li>
<li>Rewrite of the game code, functionalities and features (in progress...):</li>
<ul>
<li>Rewrite disassembly to native C#: phase out VCPU and CAPI namespaces entirely,</li>
<li>Recheck the variable types,</li>
<li>Give meaningful names to remaining functions,</li>
<li>Remove comments related to disassembly in rewritten functions,</li>
<li>Reorder functions logically (for example, player related functions should be grouped together, city related functions should be grouped together, etc.) (partially dependent on completing code rewrite),</li>
<li>Introduce the thorough save game data check and possibly fix errors when loading from sve/map files,</li>
<li>Replace static arrays in game state with dynamic and serializable replacements like List, BDictionary, etc. (dependent on completing code rewrite),</li>
<li>Replace bitwise flags with modern replacements like EnumFlagCollection (dependent on completing code rewrite),</li>
<li>Introduce new save game format (compressed '.xsve' for example, dependent on all of the above steps),</li>
<li>Add missing values to game saves (like Random Number Generator state, some flags, counters, etc.) (dependent on New save game format),</li>
<li>Replace game save/load dialogs with grouped game saves by the concrete game (Game Name -> Game Saves),</li>
<li>Introduce a new game debug option 'Save game each turn' for easy debugging and issue submitting.</li>
</ul>
<li>Test game functionalities for game fidelity and bugs,</li>
<li>Archive the game code,</li>
<li>Start the Phase 2 of the project as a new project branch.</li>
</ul>