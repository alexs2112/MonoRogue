using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    class DFS {
        private List<Point> Origins;
        public List<Node> Nodes;
        private List<Edge> Hallways;

        public DFS(List<Point> origins) {
            Origins = origins;
            Hallways = new List<Edge>();
            Nodes = new List<Node>();
            
            foreach (Point o in origins) { Nodes.Add(new Node(o)); }
        }

        // Copy constructor, shallow cloning doesnt work as Nodes have references to other Nodes
        // This means you can have Nodes of one DFS pointing to the Nodes of another DFS it copied from
        public DFS(DFS other) {
            Origins = other.Origins;
            Hallways = new List<Edge>();
            Nodes = new List<Node>();

            foreach (Point o in Origins) { Nodes.Add(new Node(o)); }
            foreach (Edge e in other.Hallways) { AddHallway(e); }
        }

        public void PrintAdjacencyList() {
            foreach (Node n in Nodes) {
                System.Console.Write($"{n.Position.X}:{n.Position.Y}\t\t");
                foreach (Node n2 in n.Neighbours) {
                    System.Console.Write($"{n2.Position.X}:{n2.Position.Y} ");
                }
                System.Console.Write("\n");
            }
        }

        private Node GetNodeByOrigin(Point origin) {
            foreach (Node n in Nodes) {
                if (n.Position == origin) { return n; }
            }
            throw new System.Exception($"Could not find {origin.X}:{origin.Y} in the list of nodes\n{Nodes.ToString()}");
        }

        public void AddHallway(Edge hallway) {
            Node a = GetNodeByOrigin(hallway.A);
            Node b = GetNodeByOrigin(hallway.B);
            a.Neighbours.Add(b);
            b.Neighbours.Add(a);
            Hallways.Add(hallway);
        }

        // Return True if there is a cycle
        private bool DepthFirstSearch(Node current, List<Node> visited, Node parent) {
            visited.Add(current);

            foreach (Node n in current.Neighbours) {
                if (n.Equals(parent)) { continue; }
                else if (visited.Contains(n)) { return true; }
                else if (DepthFirstSearch(n, visited, current)) { return true; }
            }
            return false;
        }

        // See if adding an edge to the graph will result in a cycle
        public bool ContainsCycleWith(Edge edge) {
            DFS trial = new DFS(this);
            trial.AddHallway(edge);

            Node start = trial.GetNodeByOrigin(edge.A);
            return (trial.DepthFirstSearch(start, new List<Node>(), start));
        }
    }

    struct Node {
        public Point Position;
        public List<Node> Neighbours;

        public Node(Point position) {
            Position = position;
            Neighbours = new List<Node>();
        }

        public bool Equals(Node other) {
            return Position == other.Position;
        }
    }
}
