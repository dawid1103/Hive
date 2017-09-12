using System;
using System.Drawing;

namespace HiveSymulator
{
    public enum BeeState
    {
        Idle,
        FlyingToFlower,
        GatheringNectar,
        ReturningToHive,
        MakingHoney,
        Retired
    }

    [Serializable]
    public class Bee
    {
        private const double HONEY_CONSUMED = 0.5;
        private const int MOVE_RATE = 3;
        private const double MINIMUM_FLOWER_NECTAR = 1.5;
        private const int CAREER_SPAN = 1000;

        public int Age { get; private set; }
        public bool InsideHive { get; private set; }
        public double NectarCollected { get; private set; }
        public BeeState CurrentState { get; private set; }

        private Point _location;
        public Point Location { get { return _location; } }

        private int _ID;
        private Flower destinationFlower;

        private Hive _hive;
        private World _world;

        [NonSerialized]
        public BeeMessage MessageSender;

        public Bee(int id, Point location, World world, Hive hive)
        {
            _ID = id;
            Age = 0;
            _location = location;
            InsideHive = true;
            destinationFlower = null;
            NectarCollected = 0;
            CurrentState = BeeState.Idle;
            _hive = hive;
            _world = world;
        }


        public void Go(Random random)
        {
            Age++;
            BeeState oldState = CurrentState;
            switch (CurrentState)
            {
                case BeeState.Idle:
                    if (Age > CAREER_SPAN)
                    {
                        CurrentState = BeeState.Retired;
                    }
                    else if (_world.Flowers.Count > 0 && _hive.ConsumeHoney(HONEY_CONSUMED))
                    {
                        Flower flower = _world.Flowers[random.Next(_world.Flowers.Count)];
                        if (flower.Nectar >= MINIMUM_FLOWER_NECTAR && flower.Alive)
                        {
                            destinationFlower = flower;
                            CurrentState = BeeState.FlyingToFlower;
                        }
                    }

                    break;
                case BeeState.FlyingToFlower:
                    if (!_world.Flowers.Contains(destinationFlower))
                    {
                        CurrentState = BeeState.ReturningToHive;
                    }
                    else if (InsideHive)
                    {
                        if (MoveTowardsLocation(_hive.GetLocation(HiveLocations.Exit)))
                        {
                            InsideHive = false;
                            _location = _hive.GetLocation(HiveLocations.Entrance);
                        }
                    }
                    else
                    {
                        if (MoveTowardsLocation(destinationFlower.Location))
                        {
                            CurrentState = BeeState.GatheringNectar;
                        }
                    }
                    break;
                case BeeState.GatheringNectar:
                    double nectar = destinationFlower.HarvestNectar();
                    if (nectar > 0)
                        NectarCollected += nectar;
                    else
                        CurrentState = BeeState.ReturningToHive;
                    break;
                case BeeState.ReturningToHive:
                    if (!InsideHive)
                    {
                        if (MoveTowardsLocation(_hive.GetLocation(HiveLocations.Entrance)))
                        {
                            InsideHive = true;
                            _location = _hive.GetLocation(HiveLocations.Exit);
                        }
                    }
                    else
                    {
                        if (MoveTowardsLocation(_hive.GetLocation(HiveLocations.Factory)))
                        {
                            CurrentState = BeeState.MakingHoney;
                        }
                    }

                    break;
                case BeeState.MakingHoney:
                    if (NectarCollected < 0.5)
                    {
                        NectarCollected = 0;
                        CurrentState = BeeState.Idle;
                    }
                    else
                    {
                        double honeyPortion = 0.5;
                        if (_hive.AddHoney(honeyPortion))
                        {
                            NectarCollected -= honeyPortion;
                        }
                        else
                        {
                            NectarCollected = 0;
                        }
                    }
                    break;
                case BeeState.Retired:
                    break;
                default:
                    break;
            }

            if (oldState != CurrentState && MessageSender != null)
            {
                string stringState;
                switch (CurrentState)
                {
                    case BeeState.FlyingToFlower:
                        stringState = "Leci do kwiatów";
                        break;
                    case BeeState.GatheringNectar:
                        stringState = "Zbiera nektar";
                        break;
                    case BeeState.ReturningToHive:
                        stringState = "Wraca do ula";
                        break;
                    case BeeState.MakingHoney:
                        stringState = "Wytwarze miód";
                        break;
                    case BeeState.Retired:
                        stringState = "Na emeryturze";
                        break;
                    default:
                        stringState = "Bezrobotna";
                        break;
                }
                MessageSender(_ID, stringState);
            }
        }

        private bool MoveTowardsLocation(Point destination)
        {
            if (Math.Abs(destination.X - _location.X) <= MOVE_RATE && Math.Abs(destination.Y - _location.Y) <= MOVE_RATE)
                return true;

            if (destination.X > _location.X)
                _location.X += MOVE_RATE;
            else if (destination.X < _location.X)
                _location.X -= MOVE_RATE;

            if (destination.Y > _location.Y)
                _location.Y += MOVE_RATE;
            else if (destination.Y < _location.Y)
                _location.Y -= MOVE_RATE;

            return false;
        }
    }
}
