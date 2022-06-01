using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class DungeonGeneration {
        private System.Random Random;
        private int Width;
        private int Height;
        private int[,] Tiles;

        public DungeonGeneration(System.Random rng, int width, int height, int[,] tiles) {
            Random = rng;
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        public int[,] Generate(int iterations) {
            // First set everything to be a wall
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Tiles[x, y] = 1;
                }
            }

            // Divide up the world into partitions, generate a room in each partition
            List<Rectangle> rooms = new List<Rectangle>();
            Partitioner partitioner = new Partitioner(Random, Width - 2, Height - 2);
            List<Rectangle> partitions = partitioner.Partition(1, 1, iterations);
            foreach (Rectangle p in partitions) {
                Rectangle room = PlaceRoom(p);
                rooms.Add(room);
            }

            // Find all possible hallways, use Kruskal's algorithm to construct a minimum spanning tree
            List<Point> origins = GetRoomOrigins(rooms);
            List<Edge> edges = GetHallwayCandidates(origins);
            List<Edge> trimmed = TrimHallwayCandidates(edges);
            List<Edge> hallways = FinalizeHallways(origins, trimmed);
            ConstructHallways(hallways);

            return Tiles;
        }

        // Given a rectangular partition of space, place a room in it somewhere and return the room rectangle
        private Rectangle PlaceRoom(Rectangle partition) {
            int width = Random.Next(Constants.RoomMinSize, partition.Width);
            int height = Random.Next(Constants.RoomMinSize, partition.Height);
            int x = Random.Next(partition.X, partition.X + partition.Width - width);
            int y = Random.Next(partition.Y, partition.Y + partition.Height - height);
            for (int dx = 0; dx < width; dx++) {
                for (int dy = 0; dy < height; dy++) {
                    Tiles[dx + x, dy + y] = 0;
                }
            }
            return new Rectangle(x, y, width, height);
        }

        // Get the center tile of each room
        private List<Point> GetRoomOrigins(List<Rectangle> rooms) {
            List<Point> origins = new List<Point>();
            foreach (Rectangle room in rooms) {
                int x = room.X + room.Width / 2;
                int y = room.Y + room.Height / 2;
                origins.Add(new Point(x, y));
            }
            return origins;
        }

        // Get all possible hallways by drawing a line from each room origin to each other origin
        private List<Edge> GetHallwayCandidates(List<Point> origins) {
            List<Edge> edges = new List<Edge>();
            List<Point> origins2 = new List<Point>(origins);

            foreach (Point s in origins) {
                origins2.Remove(s);
                foreach (Point d in origins2) {
                    List<Point> hallway = GetHallwayLine(s, d);
                    edges.Add(new Edge(s, d, hallway));
                }
            }
            return edges;
        }

        // Go over each edge, remove points that are already floors (overlap with rooms)
        private List<Edge> TrimHallwayCandidates(List<Edge> edges) {
            List<Edge> trimmed = new List<Edge>();
            foreach (Edge e in edges) {
                List<Point> newPath = new List<Point>();
                foreach (Point p in e.Path) {
                    if (Tiles[p.X, p.Y] == 1) { newPath.Add(p); }
                }
                trimmed.Add(new Edge(e.A, e.B, newPath));
            }
            return trimmed;
        }

        // Go over all hallways, use kruskals algorithm to find the edges that create a minimum spanning tree
        private List<Edge> FinalizeHallways(List<Point> origins, List<Edge> edges) {
            // Sort the list by minimum length of edges
            List<Edge> sorted = edges.OrderBy(e => e.Path.Count).ToList<Edge>();

            DFS dfs = new DFS(origins);

            // Kruskal's Algorithm implementation
            int i = 0;
            List<Edge> hallways = new List<Edge>();
            while (hallways.Count < origins.Count - 1) {
                Edge candidate = sorted[i];

                if (!dfs.ContainsCycleWith(candidate)) { 
                    hallways.Add(candidate);                     
                    dfs.AddHallway(candidate);
                }
                i++;
            }
            return hallways;
        }

        // Once they are finalized, we go and mark each hallway tile as a floor
        private void ConstructHallways(List<Edge> hallways) {
            foreach (Edge e in hallways) {
                foreach (Point p in e.Path) {
                    Tiles[p.X, p.Y] = 0;
                }
            }
        }

        // Return a line of points between a source and destination without including diagonals
        private List<Point> GetHallwayLine(Point source, Point dest) {
            List<Point> points = new List<Point>();

            int sx = source.X < dest.X ? 1 : -1;
            int sy = source.Y < dest.Y ? 1 : -1;
            int dx = System.Math.Abs(dest.X - source.X);
            int dy = System.Math.Abs(dest.Y - source.Y);
            int err = dx - dy;

            while (true) {
                points.Add(source);
                if (source == dest) { break; }

                int e2 = err * 2;
                if (e2 > -dy) {
                    err -= dy;
                    source.X += sx;
                } else if (e2 < dx) {
                    err += dx;
                    source.Y += sy;
                }
            }
            return points;
        }
    }

    struct Edge {
        public Point A;
        public Point B;
        public List<Point> Path;
        public Edge(Point a, Point b, List<Point> path) {
            A = a;
            B = b;
            Path = path;
        }
    }
}
