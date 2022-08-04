// Implementation of A* pathfinding from this tutorial: http://trystans.blogspot.com/2011/09/roguelike-tutorial-13-aggressive.html
using static System.Math;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class Pathfinder {
        private List<Point> Open;
        private List<Point> Closed;
        private Dictionary<Point, Point> Parents;
        private Dictionary<Point, int> TotalCost;
        private Creature Creature;

        public Pathfinder(Creature creature) {
            Creature = creature;
            Open = new List<Point>();
            Closed = new List<Point>();
            Parents = new Dictionary<Point, Point>();
            TotalCost = new Dictionary<Point, int>();
        }

        private int HeuristicCost(Point source, Point dest) {
            return Max(Abs(source.X - dest.X), Abs(source.Y - dest.Y));
        }

        private int CostToGetTo(Point source) {
            if (Parents.ContainsKey(source)) {
                if (Creature.World.GetCreatureAt(source) != null) {
                    return 3 + CostToGetTo(Parents[source]); 
                } else {
                    return 1 + CostToGetTo(Parents[source]); 
                }
            }
            else { return 0; }
        }

        private int Cost(Point source, Point dest) {
            if (TotalCost.ContainsKey(source)) { return TotalCost[source]; }

            int cost = CostToGetTo(source) + HeuristicCost(source, dest);
            TotalCost.Add(source, cost);
            return cost;
        }

        private Point GetClosestPoint(Point end) {
            Point closest = Open[0];
            foreach (Point p in Open) {
                if (Cost(p, end) < Cost(closest, end)) { closest = p; }
            }
            return closest;
        }

        private void Reparent(Point child, Point parent) {
            if (Parents.ContainsKey(child)) {
                Parents[child] = parent;
            } else {
                Parents.Add(child, parent);
            }
            TotalCost.Remove(child);
        }

        private void ReparentNeighbour(Point closest, Point neighbour) {
            Reparent(neighbour, closest);
            Open.Add(neighbour);
        }

        private void ReparentNeighbourIfNecessary(Point closest, Point neighbour) {
            Point original = Parents[neighbour];
            int currentCost = CostToGetTo(neighbour);
            Reparent(neighbour, closest);
            int reparentCost = CostToGetTo(neighbour);

            if (reparentCost < currentCost) { Open.Remove(neighbour); }
            else { Reparent(neighbour, original); }
        }

        private List<Point> GetNeighbours(Point p) {
            List<Point> res = new List<Point>();
            res.Add(new Point(p.X - 1, p.Y));
            res.Add(new Point(p.X + 1, p.Y));
            res.Add(new Point(p.X, p.Y - 1));
            res.Add(new Point(p.X, p.Y + 1));

            if (Constants.AllowDiagonalMovement) {
                res.Add(new Point(p.X - 1, p.Y - 1));
                res.Add(new Point(p.X - 1, p.Y + 1));
                res.Add(new Point(p.X + 1, p.Y - 1));
                res.Add(new Point(p.X + 1, p.Y + 1));
            }
            return res;
        }

        private void CheckNeighbours(Point end, Point closest) {
            foreach (Point neighbour in GetNeighbours(closest)) {
                if (
                    Closed.Contains(neighbour)
                 || !Creature.CanEnter(neighbour)
                 && !neighbour.Equals(end)) {
                     continue;
                    }
                
                if (Open.Contains(neighbour)) { ReparentNeighbourIfNecessary(closest, neighbour); }
                else { ReparentNeighbour(closest, neighbour); }
            }
        }

        private List<Point> CreatePath(Point start, Point end) {
            List<Point> path = new List<Point>();

            while (!end.Equals(start)) {
                path.Add(end);
                end = Parents[end];
            }

            path.Reverse();
            return path;
        }

        public List<Point> FindPath(Point start, Point end, int maxTries) {
            Open.Clear();
            Closed.Clear();
            Parents.Clear();
            TotalCost.Clear();

            Open.Add(start);
            for (int tries = 0; tries < maxTries && Open.Count > 0; tries++) {
                Point closest = GetClosestPoint(end);

                Open.Remove(closest);
                Closed.Add(closest);

                if (closest.Equals(end)) { return CreatePath(start, closest); }
                else { CheckNeighbours(end, closest); }
            }
            return null;
        }

        public static List<Point> FindPath(Creature c, int x, int y) { 
            Pathfinder pf = new Pathfinder(c);
            return pf.FindPath(new Point(c.X, c.Y), new Point(x, y), 150);
        }
    }
}
