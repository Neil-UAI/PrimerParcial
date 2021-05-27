using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSimulation
{
    class Pheromone : GameObject
    {
        public static void SpawnOn(World world, PointF pos, double intensity = 100)
        {
            if (intensity > 100) { intensity = 100; }

            Pheromone[] neighbours = world.GameObjectsNear(pos)
                .Select(each => each as Pheromone)
                .Where(p => p != null)
                .ToArray();

            if (neighbours.Length == 0)
            {
                // No objects there
                Pheromone clone = new Pheromone(intensity);
                clone.Position = pos;
                world.Add(clone);
            }
            else
            {
                foreach (Pheromone p in neighbours)
                {
                    if (p.Intensity < intensity)
                    {
                        p.Intensity = intensity;
                    }
                }
            }
        }

        private double intensity;

        public Pheromone(double intensity = 100)
        {
            this.intensity = intensity;
            UpdateColor();
        }

        public double Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public override void UpdateOn(World world)
        {
            if (intensity <= 1)
            {
                world.Remove(this);
            }
            else
            {
                SpreadOn(world);

                intensity *= 0.9f;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            Color = Color.FromArgb((int)Math.Floor(intensity / 100 * 255), 255, 255, 0);
        }

        private void SpreadOn(World world)
        {
            float radius = 2;
            for (float x = Position.X - radius; x <= Position.X + radius; x++)
            {
                for (float y = Position.Y - radius; y <= Position.Y + radius; y++)
                {
                    if (x == Position.X && y == Position.Y) continue;
                    double squaredDist = Math.Pow(x - Position.X, 2) + Math.Pow(y - Position.Y, 2);
                    if (squaredDist <= radius)
                    {
                        double diffuse = 0.75;
                        SpawnOn(world, new PointF(x, y), intensity * diffuse);
                    }
                }
            }
        }
    }
}
