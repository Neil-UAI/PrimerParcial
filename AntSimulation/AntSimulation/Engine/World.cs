using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSimulation
{
    class World
    {
        private Random rnd = new Random();

        private const int width = 125;
        private const int height = 125;
        private Size size = new Size(width, height);

        private HashSet<GameObject> objects = new HashSet<GameObject>();
        private const int cellSize = 25;
        private List<GameObject>[,] gameObjects = new List<GameObject>[width / cellSize, height / cellSize];

        public IEnumerable<GameObject> GameObjects { get { return objects.ToArray(); } }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public PointF Center { get { return new PointF(width / 2, height / 2); } }

        public World()
        {
            this.InitPartitions();
        }

        public bool IsInside(PointF p)
        {
            return p.X >= 0 && p.X < width
                && p.Y >= 0 && p.Y < height;
        }

        public PointF RandomPoint()
        {
            return new PointF(rnd.Next(width), rnd.Next(height));
        }

        public float Random()
        {
            return (float)rnd.NextDouble();
        }

        public float Random(float min, float max)
        {
            return (float)rnd.NextDouble() * (max - min) + min;
        }

        private void InitPartitions()
        {
            for (int i = 0; i < gameObjects.GetLength(0); i++)
            {
                for (int j = 0; j < gameObjects.GetLength(1); j++)
                {
                    this.gameObjects[i, j] = new List<GameObject>();
                }
            }
        }

        private List<GameObject> GetPartitionAt(PointF pos)
        {
            return gameObjects[Mod(pos.X, width) / cellSize, Mod(pos.Y, height) / cellSize];
        }

        public void Add(GameObject obj)
        {
            objects.Add(obj);

            if (!(obj is Ant))
            {
                var partition = GetPartitionAt(obj.Position);
                partition.Add(obj);
            }
        }

        public void Remove(GameObject obj)
        {
            objects.Remove(obj);
            if (!(obj is Ant))
            {
                var partition = GetPartitionAt(obj.Position);
                if (partition != null)
                {
                    partition.Remove(obj);
                }
            }
        }

        public void Update()
        {
            foreach (GameObject obj in GameObjects)
            {
                obj.InternalUpdateOn(this);
                obj.Position = Mod(obj.Position, size);
            }
        }

        public void DrawOn(Graphics graphics)
        {
            graphics.FillRectangle(Brushes.White, 0, 0, width, height);
            foreach (GameObject obj in GameObjects)
            {
                graphics.FillRectangle(new Pen(obj.Color).Brush, obj.Bounds);
            }
        }

        public double Dist(PointF a, PointF b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public double Dist(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        // http://stackoverflow.com/a/10065670/4357302
        private static int Mod(float a, float n)
        {
            float result = a % n;
            if ((a < 0 && n > 0) || (a > 0 && n < 0))
                result += n;
            return (int)result;
        }
        private static PointF Mod(PointF p, SizeF s)
        {
            return new PointF(Mod(p.X, s.Width), Mod(p.Y, s.Height));
        }

        public IEnumerable<GameObject> GameObjectsNear(PointF pos, float dist = 1)
        {
            List<GameObject> result = new List<GameObject>();

            List<GameObject> partition = this.GetPartitionAt(pos);
            result.AddRange(partition);

            float x = pos.X / cellSize;
            float y = pos.Y / cellSize;

            if (pos.X / cellSize > 0 && pos.Y / cellSize > 0)
                result.AddRange(this.GetPartitionAt(new PointF(x - 1, y - 1)));

            if (pos.X / cellSize < gameObjects.GetLength(0) - 1 && pos.Y / cellSize < gameObjects.GetLength(1) - 1)
                result.AddRange(this.GetPartitionAt(new PointF(x + 1, y + 1)));

            if (pos.X / cellSize > 0 && pos.Y / cellSize < gameObjects.GetLength(1) - 1)
                result.AddRange(this.GetPartitionAt(new PointF(x - 1, y + 1)));

            if (pos.X / cellSize < gameObjects.GetLength(0) - 1 && pos.Y / cellSize > 0)
                result.AddRange(this.GetPartitionAt(new PointF(x + 1, y - 1)));

            if (pos.X / cellSize > 0)
                result.AddRange(this.GetPartitionAt(new PointF(x - 1, y)));

            if (pos.Y / cellSize > 0)
                result.AddRange(this.GetPartitionAt(new PointF(x, y - 1)));

            if (pos.X / cellSize < gameObjects.GetLength(0) - 1)
                result.AddRange(this.GetPartitionAt(new PointF(x + 1, y)));

            if (pos.Y / cellSize < gameObjects.GetLength(1) - 1)
                result.AddRange(this.GetPartitionAt(new PointF(x, y + 1)));

            for (int i = 0; i < result.Count; i++)
            {
                GameObject go = result[i];
                if (Dist(go.Position, pos) > dist)
                {
                    result.Remove(go);
                }
            }

            return result;
        }

    }
}
