using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class Bike
    {
        #region Private fields
        private readonly Tank _tank;
        private readonly MutableSpeed _speed;
        private readonly int _width;
        private readonly int _side;
        private SimplePoint ReachBoxLT => LeftTopPoint();
        private SimplePoint ReachBoxRB => RightBottomPoint();
        #endregion

        #region Properties
        public SimplePoint Location { get; private set; }
        public ImmutableSpeed Speed => (ImmutableSpeed)_speed;
        public int MaxCapacity => _tank.MaxCapacity;
        public int TankLevel => _tank.CurrentLevel;
        public bool IsOutOfGas => _tank.IsEmpty();
        #endregion

        #region Constructors
        public Bike(int width, int height, int side)
        {
            if (width <= 0 || height <= 0) throw new ArgumentException("Parameters of level cannot be equal to or less than zero.");
            int x = (width - side) / 2;
            int y = height * 7 / 8; 

            Location = new SimplePoint(x, y);
            _width = width - side;
            _side = side;
            _speed = new MutableSpeed();
            _tank  = new Tank();
        }
        #endregion

        #region Public methods
        public void IncreaseTankLevel(Fuel fuel)
        {
            _tank.IncreaseChargeLevel(fuel);
        }

        public void DecreaseTankLevel()
        {
            switch ((int)_speed)
            {
                case 1: _tank.DecreaseChargeLevel((int)_speed); break;
                case 2: _tank.DecreaseChargeLevel((int)_speed); break;
                case 3: _tank.DecreaseChargeLevel((int)_speed); break;
            }
        }

        public void SpeedUp() { _speed.SpeedUp(); }

        public void SlowDown() { _speed.SlowDown();  }

        public void Reset()
        {
            _tank.Reset();
            _speed.Reset();
            Location.X = _width / 2;
        }

        public void SetSpeed(AbstractSpeed speed)
        {
            _speed.SetSpeed(speed);
        }

        public void MoveLeft()
        {
            if (Location.X - 10 >= 0) Location.X -= 10;
        }

        public void MoveRight()
        {
            if (Location.X + 10 < _width) Location.X += 10;
        }

        public bool IsFuelNear(Fuel fuel)
        {
            return WithinHorizontalReach(fuel.MidPoint.X) && WithinVerticalReach(fuel.MidPoint.Y);
        }
        #endregion

        #region Private methods
        private SimplePoint LeftTopPoint()
        {
            float x = Location.X - ((float)_side / 4);
            float y = Location.Y - ((float)_side / 4);
            return new SimplePoint((int)x, (int)y);
        }

        private SimplePoint RightBottomPoint()
        {
            float x = Location.X + ((float)_side * 5 / 4);
            float y = Location.Y + _side;
            return new SimplePoint((int)x, (int)y);
        }

        private bool WithinHorizontalReach(float x)
        {
            return ReachBoxLT.X <= x && x <= ReachBoxRB.X;
        }

        private bool WithinVerticalReach(float y)
        {
            return ReachBoxLT.Y <= y && y <= ReachBoxRB.Y;
        }
        #endregion
    }
}
