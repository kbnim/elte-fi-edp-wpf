using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class Fuel
    {
        #region Properties
        public int Volume { get; private set; }
        public SimplePoint Location { get; private set; }
        public SimplePoint MidPoint => CalculateMidPoint();
        public bool IsOutsideOfWindow => Location.Y >= _height;
        #endregion

        #region Private fields
        private readonly int _height;
        private readonly int _side;
        private static readonly Random _random = new();
        #endregion

        #region Constructors
        public Fuel(int width, int height, int side, int volume = 100)
        {
            if (volume <= 0) throw new EmptyOrNegativeVolumeException();
            Volume = volume;
            _height = height;
            _side = side;

            int x = _random.Next(width - side);
            Location = new SimplePoint(x, (-1) * side / 2);
        }
        #endregion

        public void MoveDownwards(AbstractSpeed speed)
        {
            Location.Y += 7 * (int)speed;
        }

        #region Private methods
        private SimplePoint CalculateMidPoint()
        {
            int midX = Location.X + _side / 2;
            int midY = Location.Y + _side / 2;
            return new SimplePoint(midX, midY);
        }
        #endregion

        #region Exceptions
        public class EmptyOrNegativeVolumeException : Exception { }
        #endregion
    }
}
