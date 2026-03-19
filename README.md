# Map maker for the signaller simulator
This repository holds the source code a native map editor helper tool for [`signaller`](https://github.com/GitHubDanya/signaller).
It provides a graphical interface for plotting simulation maps instead of having to write JSON manually.

## Shortcuts

* `LMB` - Draw Node
* `R` - Draw edge
* `Ctrl+R` - Delete edge
* `Ctrl+E` - Delete edge

## Usage Guide
When you open the editor you will be greeted with a grid. Each bold grid space represents a `100x100px` field, and each subdivision represents a `25x25px` square.
You can drag the map with LMB.


### Selection Mechanic
You can select nodes and edges by clicking on them. You can have up to 2 nodes selected at a time.

Nodes are selected sequentially and are tied to the order of selection. The current selected node is displayed as yellow, and the second selected node is displayed as a lighter
yellow.

The current selected edge is displayed as orange.


### Drawing Nodes
To draw a node, you can click anywhere on the grid with `LMB`. Nodes get their ID's assigned automatically with the prefix assigned at the `Node` page of the UI.


### Drawing Edges
To draw an edge, ensure that you have:
* 2 nodes selected.
* Assigned a Length and Width in the `Edge` section of the UI.
  
then either click the `'Create'` button in the `Edge` UI section, or use the keyboard shortcut `E`.
This action creates an edge, where the edge spans from the oldest selected node to the newest selected node.

To delete edges, press the `'Delete'` button in the `Edge` UI section, or use the keyboard shortcut `Ctrl + E`.

### Saving Data
To save data, click the `Save JSON` button in the `JSON` section of the UI.
