using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class ImmutableSpeed : AbstractSpeed
    {
        #region Constructors
        public ImmutableSpeed(int speed = 1)
        {
            _speed = speed switch
            {
                1 or 2 or 3 => speed,
                _ => throw new ArgumentException("Argument can only be 1, 2 or 3.", nameof(speed)),
            };
        }
        #endregion

        #region Public methods
        public static ImmutableSpeed Parse(string s)
        {
            return s switch
            {
                "Slow" or "Slow\n" => new ImmutableSpeed(1),
                "Medium" or "Medium\n" => new ImmutableSpeed(2),
                "Fast" or "Fast\n" => new ImmutableSpeed(3),
                _ => throw new FormatException("Invalid formatting of type 'ImmutableSpeed'"),
            };
        }
        #endregion
    }
}
