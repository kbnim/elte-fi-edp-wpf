using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public abstract class AbstractSpeed
    {
        #region Protected fields
        protected int _speed;
        #endregion

        #region Overridden methods
        public override string ToString()
        {
            return _speed switch
            {
                1 => "Slow",
                2 => "Medium",
                3 => "Fast",
                _ => "Invalid speed value",
            };
        }
        #endregion

        #region Operators
        public static explicit operator int(AbstractSpeed speed)
        {
            return speed._speed;
        }
        #endregion
    }
}
