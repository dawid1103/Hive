using System;
using System.Drawing;

namespace HiveSymulator
{
    [Serializable]
    public class Flower
    {
        private const int LIFE_SPAN_MIN = 15000;
        private const int LIFE_SPAN_MAX = 30000;
        private const double INITIAL_NECTAR = 1.5;
        private const double MAX_NECTAR = 5;
        private const double NECTAR_ADDED_PER_TURN = 0.01;
        private const double NECTAR_GATHERED_PER_TURN = 0.3;

        public Point Location { get; private set; }
        public int Age { get; private set; }
        public bool Alive { get; private set; }
        public double Nectar { get; private set; }
        public double NectarHarvested { get; set; }
        private int _lifespan;

        public Flower(Point location, Random random)
        {
            Location = location;
            Age = 0;
            Alive = true;
            Nectar = INITIAL_NECTAR;
            NectarHarvested = 0;
            _lifespan = random.Next(LIFE_SPAN_MIN, LIFE_SPAN_MAX + 1);
        }

        /// <summary>
        /// Pobieramy nektar z kwiata 
        /// </summary>
        public double HarvestNectar()
        {
            if (NECTAR_GATHERED_PER_TURN <= Nectar)
            {
                NectarHarvested += NECTAR_GATHERED_PER_TURN;
                Nectar -= NECTAR_GATHERED_PER_TURN;
                return NECTAR_GATHERED_PER_TURN;
            }
            else
                return 0;

        }

        public void Go()
        {
            if (Age < LIFE_SPAN_MAX && Alive)
            {
                this.Age++;
                Nectar += NECTAR_ADDED_PER_TURN;

                if (Nectar > MAX_NECTAR)
                    this.Nectar = MAX_NECTAR;
            }
            else
                Alive = false;
        }
    }
}
