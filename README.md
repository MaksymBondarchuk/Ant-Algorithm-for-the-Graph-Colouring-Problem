# An Ant Algorithm for the Graph Colouring Problem
Article for scientists https://www.sciencedirect.com/science/article/pii/S0166218X07001023

## Common information
The ant algorithm is a multi-agent system based on the idea of parallel search. In this algorithm a given number of ants move around the nodes of the
graph and changes the colour of each visited node according to a local criterion. At a given iteration
each ant moves from the current node to the adjacent node with the maximum number of violations
(see Fig 1), and replaces the old colour of the node with a new colour that minimises this number.
For a given node i, the number of violations is computed as the number of adjacent nodes to i
with the same colour than i. This action is randomly repeated for each ant: the ant moves to the
worst adjacent node with a certain probability pn (otherwise it moves to any other adjacent node
randomly chosen), and assigns the best possible colour with a probability pc (otherwise any colour
is assigned at random). The probabilistic nature of the algorithm allows the ants to escape from
local minima and obtain bounds close to the absolute minimum. This process, which is carried out
simultaneously by the set of ants, is repeated until the optimal solution is found or the algorithm
converges. The number of ants that move along the graph is an adjustable parameter and increases
with the order of the graph.

<img src="/images/Figure 1.png">
Figure 1: Movement of an ant towards the worst local node (the gures indicate the number of
violations of each node).



> Information is given by Francesc Comellas and Javier Ozon Departament de Matematica Aplicada i Telematica, Universitat Politecnica de Catalunya C/ Jordi Girona 1-3, Campus Nord C3, 08034 Barcelona, Spain

## Algorithm in lab
### Local criterion
pn is determined as M*maxconf/confsoverall, where maxconf - number of conflicts in the worst adjacent node, 
confsoverall - sum of conflicts of adjacent nodes and M is just a constant (currently one) that can be changed in Graph.cs.

### Modification
According to basic algorithm when ant moves to some node it should somehow improve situation there.
To prevent deadlocks in my algorithm if ant sees that there were no changes for 10 previous iterations it 
sets node color as random.

## How does the program work
Firstly user should select Collection file (.col) that determines graph in format:
- "c {comment}"
- "p edge {number of nodes}"
- list of edges where one edge represented as "e {node from} {node to}"
- chromatic number is in the end of file name: "name.{chromatic number}.col" (e.g. file yuzGCPrnd127.13.col has chromatic number 13)

> Note that {node from} {node to} is not indexes of nodes but numbers (numeration starts from one, not zero)

Each graph can be visualized on canvas. But if it contains more than 50 nodes it can slow down program so
user will be questioned if he/she want to draw graph or to just work in console.

After that user can set its preferd numer of ants (by default it equals 1/3 of number of nodes).

Then user can try to color graph in one click by clicking "Color" or watch color process by clicking
"One iteration" where each ant location and number of conflicts for every node will be shown.

If graph is successfully colored message will be shown with number of iterations and resulted
graph will be saved in file with the same name and location as .col but with extencion .log.
This file is consisist of:
- "c ... {original file name}"
- list of nodes where one node represented as "n {node numer} {color number}"

> Note that {node numer} is not indexes of nodes but numbers (numeration starts from one, not zero)
