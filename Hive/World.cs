using System;
using System.Collections.Generic;
using System.Drawing;

namespace HiveSymulator
{
    [Serializable]
    public class World
    {
        private const double NECTAR_HARVESTED_PER_NEW_FLOWER = 50.0;
        private const int FIELD_MINX = 15;
        private const int FIELD_MINY = 177;
        private const int FIELD_MAXX = 690;
        private const int FIELD_MAXY = 290;

        public List<Flower> Flowers { get; private set; }
        public List<Bee> Bees { get; private set; }
        public Hive Hive { get; private set; }

        public World() { }
        public World(BeeMessage messageSender)
        {
            this.Bees = new List<Bee>();
            this.Flowers = new List<Flower>();
            this.Hive = new Hive(this, messageSender);
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                AddFlower(random);
            }
        }

        public void AddBee(Bee bee)
        {
            Bees.Add(bee);
        }

        public void Go(Random random)
        {
            Hive.Go(random);

            for (int i = Bees.Count - 1; i >= 0; i--)
            {
                Bee bee = Bees[i];
                bee.Go(random);
                if (bee.CurrentState == BeeState.Retired)
                {
                    Bees.Remove(bee);
                }
            }

            double totalNectarHarvested = 0;
            for (int i = Flowers.Count - 1; i >= 0; i--)
            {
                Flower flower = Flowers[i];
                flower.Go();
                totalNectarHarvested += flower.NectarHarvested;
                if (!flower.Alive)
                {
                    Flowers.Remove(flower);
                }
            }

            if (totalNectarHarvested > NECTAR_HARVESTED_PER_NEW_FLOWER)
            {
                foreach (Flower flower in Flowers)
                {
                    flower.NectarHarvested = 0;
                }
                AddFlower(random);
            }
        }

        private void AddFlower(Random random)
        {
            Point location = new Point(random.Next(FIELD_MINX, FIELD_MAXX), random.Next(FIELD_MINY, FIELD_MAXY));
            Flower newFlower = new Flower(location, random);
            Flowers.Add(newFlower);
        }
    }
}

