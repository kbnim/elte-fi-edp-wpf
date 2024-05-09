using RaceBike.Model.Classes;

namespace RaceBike.Model
{
    public class FuelEventArgs
    {
        public bool IsDeletion { get; private set; }
        public SimplePoint Point { get; private set; }
        public FuelEventArgs(bool isDeletion, SimplePoint point)
        {
            IsDeletion = isDeletion;
            Point = point;
        }
    }
}