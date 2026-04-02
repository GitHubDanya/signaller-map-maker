# Map maker for the signaller simulator
This repository holds the source code a native map editor helper tool for [`signaller`](https://github.com/GitHubDanya/signaller).
It provides a graphical interface for plotting simulation maps instead of having to write JSON manually.

## Shortcuts

-- Mode Selection:
* `1` - Select Build Mode
* `2` - Select Movement Mode

-- Build Mode:
* `LMB` - Draw Node
* `Ctrl+E` - Delete Node
* `R` - Draw Edge
* `Ctrl+R` - Delete Edge

-- Movement Mode:
* `W` - Create Movement
* `E` - Create Signal

The editor supports action history.
* `Ctrl+Z` - Undo
* `Ctrl+Y` - Redo

## Usage Guide
When you open the editor you will be greeted with a grid. Each bold grid space represents a `100x100px` field, and each subdivision represents a `25x25px` square.
You can drag the map with `LMB`.

The editor has different modes for various actions. They are:

* **Build Mode** - This mode is responsible for building the map. It is used for creating `Nodes`, `Edges`, `Stations` and `Platforms`. Shortcuts for this mode can
be seen in the `shortcuts` section.

* **Movement Mode** - This mode is responsible for handling anything related to train movement across the map. It is used for defining `Movements` across edges,
creating and modifying `Signals`.

To switch between the modes, use the number keys.

The UI updates dynamically to fit the current `Editor Mode`.

## Using the Build Mode

The map consists of Nodes and Edges to define movement points for trains. Edges are lines that are created between 2 nodes.

**Nodes** are created with mouse clicks. The prefix for the node can be specified in the UI. Nodes without a prefix get assigned the prefix `XX`.

**Edges** are created by either pressing the Create button on the UI or pressing `R`. To create an Edge, you must have 2 nodes selected, and
have populated the `Length`, `Speed` and `Z-Index` fields. It is recommended to always create Edges in the same direction as the expected
train flow through them (for example left-to-right if trains are expected to move to the right). This makes naming more concise and easier to understand.

**Stations** are created automatically with platform creation. Platforms automatically get assigned the station selected in the `Station Name` field,
and if no existing station exists then a new station gets created.

**Platforms** are created by clicking either the `Plat. Above` or `Plat. Below` button. Station assignment works as described in the `Stations` section.

## Using the Movement Mode

**Movements** define how trains move through edges. To create a movement, first select the source edge, then the destination edge, then press `W`.
Existing movements can be checked by hovering over an edge.

**Signals** ensure that a movement is legal. To create a signal, select the edge that the signal is on, then the edge that the signal
should point towards, and then press `E`. Note that there is no need to create multiple signals for each movement if the source edge is the same, the movements are assigned automatically
in the simulator. You can cycle the default signal state by pressing the `Cycle` button.

## Saving and Loading
To save or load, use the corresponding `Save JSON` and `Load JSON` buttons in the UI.
