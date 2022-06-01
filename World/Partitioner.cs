using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {

    // A class to break up a large rectangle into a bunch of partitions, using Binary Space Partitioning
    public class Partitioner {
        private System.Random Random;
        int Width;
        int Height;

        public Partitioner(System.Random rng, int width, int height) {
            Random = rng;
            Width = width;
            Height = height;
        }

        // A recursive call to keep dividing up the given rectangle into two smaller rectangles
        public List<Rectangle> Iterate(int x, int y, int width, int height, int iterations) {

            // If the size of the partition is smaller than the minimum room size, ignore it
            if (width < Constants.RoomMinSize || height < Constants.RoomMinSize) {
                return new List<Rectangle>();
            }

            // Base Case: Return the given rectangle in a list of itself
            if (iterations == 0 || (width < Constants.RoomMinSize * 2 && height < Constants.RoomMinSize * 2)) {
                List<Rectangle> r = new List<Rectangle>();
                r.Add(new Rectangle(x, y, width, height));
                return r;
            }

            if (width > height) {
                // Cut in half horizontally
                int halfWidth = System.Math.Max(Constants.RoomMinSize, width / 2 + Random.Next(-(width / 4), (width / 4)));

                List<Rectangle> left = Iterate(x, y, halfWidth, height, iterations - 1);
                List<Rectangle> right = Iterate(x + halfWidth + 1, y, width - halfWidth - 1, height, iterations - 1);
                left.AddRange(right);
                return left;
            } else {
                // Cut in half vertically
                int halfHeight = System.Math.Max(Constants.RoomMinSize, height / 2 + Random.Next(-(height / 4), (height / 4)));
                List<Rectangle> top = Iterate(x, y, width, halfHeight, iterations - 1);
                List<Rectangle> bot = Iterate(x, y + halfHeight + 1, width, height - halfHeight - 1, iterations - 1);
                top.AddRange(bot);
                return top;
            }
        }

        public List<Rectangle> Partition(int startX, int startY, int iterations) {
            return Iterate(startX, startY, Width, Height, iterations);
        }
    }
}
