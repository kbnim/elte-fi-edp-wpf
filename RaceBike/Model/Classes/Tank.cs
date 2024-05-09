using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class Tank
    {
        #region Properties
        public int MaxCapacity { get; private set; }
        public int CurrentLevel { get; private set; }
        #endregion

        #region Constructors
        public Tank(int maxCapacity = 1000)
        {
            if (maxCapacity <= 0) throw new EmptyOrNegativeTankCapacityException();
            MaxCapacity = maxCapacity;
            CurrentLevel = maxCapacity;
        }
        #endregion

        #region Public methods
        public void IncreaseChargeLevel(Fuel gas)
        {
            if ((CurrentLevel + gas.Volume) > MaxCapacity)
            {
                CurrentLevel = MaxCapacity;
            }
            else
            {
                CurrentLevel += gas.Volume;
            }
        }

        public void DecreaseChargeLevel(int speed)
        {
            if (CurrentLevel - speed < 0)
            {
                CurrentLevel = 0;
            }
            else
            {
                CurrentLevel -= speed;
            }
        }

        public bool IsEmpty() { return CurrentLevel == 0; }

        public void Reset()
        {
            CurrentLevel = MaxCapacity;
        }
        #endregion

        #region Exceptions
        public class EmptyOrNegativeTankCapacityException : Exception { }
        #endregion
    }
}
