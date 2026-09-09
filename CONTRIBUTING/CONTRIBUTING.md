## How to contribute to this project

Anyone can contribute to this repository in accordance with these rules:
<ul>
<li>All communication between contributors must be conducted in accordance with this repository
    <a href="https://codeberg.org/rhorvat/OpenCivOne/src/branch/master/CONTRIBUTING/code_of_conduct.md">Code Of Conduct</a>,</li>
<li>There must not be any copyrighted material contributed to this repository that would violate any of its licenses, or the license of the copyrighted material that is being contributed,</li>
<li>The contributed material must be released under the same license as this repository (MIT license), or a compatible one,</li>
<li>Contributed code changes and 'Default' game behavior must preserve Original Game behavior and design as much as possible,
    except for purposes of adding an additional functionality (which follows in the spirit of the original Game) or bug fixes (There can be additional custom Behavior themes),</li>
<li>The 'Default' Visual and Audio theme must preserve the 'Original Game appearance and feel' as much as possible (There can be additional custom Visual and Audio themes).</li>
</ul>

### Code writing rules

<ul>
<li>AI generated code (<b>AI Slop</b>) is generally not allowed, althrough, some well written AI generated code can be allowed and accepted (The project leaders reserve the right to automatically reject AI written PRs),</li>
<li>Some code has Hungarian Notation variable names (iTotal, bIsReady, sUserName, ...), these should be renamed according to the new naming rules,</li>
<li>Global variables should be Pascal case (example: UserAge, PlayerID, ...),</li>
<li>Local variables should be Camel case (example: userAge, playerID, ...),</li>
<li>Function names should be a Pascal case (sometimes combined with an underscore) (example: DrawRectangle, GenerateMap_Phase1, ...),</li>
<li>Loop variables are generally i, j, k... (it is also allowed to use ii, jj, kk...),</li>
<li>GoTo statements should generally be removed and not used at all if possible.</li>
</ul>

<p><b>Important! Before you write any bugfix, patch and submit any Pull Request, please, create an Issue first.
Otherwise, you may be fixing the issue that has already been fixed
or you are fixing the issue with the component that is currently being replaced.</b></p>

### How to contribute

Currently, you can contribute to this repository in one of the following ways:
<ul>
<li>By testing the game functionality and submitting Issues,</li>
<li>By translating the part(s) of the 'old' code from pseudo assembly language to native C# language,</li>
<li>By designing parts of a 'Default' Visual and Audio theme (which must preserve original Game appearance and feel as much as possible, SVG and MIDI/SoundFonts would be the best),</li>
<li>By designing parts of a 'Custom' Visual and Audio theme(s) (SVG and MIDI/SoundFonts would be the best).</li>
</ul>
