using System;
using System.Collections.Generic;
using System.Drawing;

namespace HiveSymulator
{
    [Serializable]
    public class Hive
    {
        private const int INITIAL_BEES = 12;
        private const double INITIAL_HONEY = 6.4;
        private const double MAXIMUM_HONEY = 39.0;
        private const double NECTAR_HONEY_RATIO = 0.5;
        private const double MINIMUM_HONEY_FOR_CREATING_BEES = 4.0;
        private const int MAXIMUM_BEES = 10000;

        public double Honey { get; private set; }
        private Dictionary<HiveLocations, Point> _locations { get; set; }
        private int _beeCount = 0;

        private World _world;

        [NonSerialized]
        public BeeMessage MessageSender;

        public Hive(World world, BeeMessage messageSender)
        {
            MessageSender = messageSender;
            Honey = INITIAL_HONEY;
            InitializeLocations();
            Random random = new Random();
            _world = world;
            for (int i = 1; i <= INITIAL_BEES; i++)
                AddBee(random);
        }

        public Point GetLocation(HiveLocations location)
        {
            if (_locations.ContainsKey(location))
                return _locations[location];
            else
                throw new ArgumentException("Nieznana lokalizacja" + location);
        }

        private void InitializeLocations()
        {
            _locations = new Dictionary<HiveLocations, Point>();
            _locations[HiveLocations.Entrance] = new Point(590, 80);
            _locations[HiveLocations.Nursery] = new Point(85, 180);
            _locations[HiveLocations.Factory] = new Point(160, 85);
            _locations[HiveLocations.Exit] = new Point(185, 225);
        }

        public bool AddHoney(double nectar)
        {
            double honeyToAdd = nectar * NECTAR_HONEY_RATIO;

            if (honeyToAdd + Honey > MAXIMUM_HONEY)
            {
                return false;
            }
            else
            {
                Honey += honeyToAdd;
                return true;
            }
        }

        public bool ConsumeHoney(double amount)
        {
            if (amount > Honey)
            {
                return false;
            }
            else
            {
                Honey -= amount;
                return true;
            }
        }

        private void AddBee(Random random)
        {
            _beeCount++;
            int r1 = random.Next(100) - 50;
            int r2 = random.Next(100) - 50;
            Point startPoint = new Point(_locations[HiveLocations.Nursery].X + r1, _locations[HiveLocations.Nursery].Y + r2);
            Bee newBee = new Bee(_beeCount, startPoint, _world, this);
            newBee.MessageSender += MessageSender;
            this._world.AddBee(newBee);
        }

        public void Go(Random random)
        {
            if (Honey > MINIMUM_HONEY_FOR_CREATING_BEES && _world.Bees.Count < MAXIMUM_BEES && random.Next(10) == 1)
            {
                AddBee(random);
            }
        }

    }

    public enum HiveLocations
    {
        Entrance,
        Nursery,
        Factory,
        Exit
    }
}
