using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class MutableSpeed : AbstractSpeed
    {
        #region Constructors
        public MutableSpeed()
        {
            _speed = 1;
        }
        #endregion

        #region Public methods
        public void SpeedUp()
        {
            if (_speed < 3) ++_speed;
        }

        public void SlowDown()
        {
            if (_speed > 1) --_speed;
        }

        public void Reset()
        {
            _speed = 1;
        }

        public void SetSpeed(AbstractSpeed speed)
        {
            _speed = (int)speed;
        }

        public ImmutableSpeed ToImmutableSpeed()
        {
            return new ImmutableSpeed(_speed);
        }
        #endregion

        #region Operators
        public static explicit operator ImmutableSpeed(MutableSpeed mutableSpeed)
        {
            return mutableSpeed.ToImmutableSpeed();
        }
        #endregion
    }
}
