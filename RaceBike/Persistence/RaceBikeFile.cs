using RaceBike.Model.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Persistence
{
    public struct RaceBikeFile
    {
        public TimeSpan LatestBestTime { get; private set; }
        public ImmutableSpeed Speed {  get; private set; }
        public Queue<GameField> Entities { get; private set; }

        public RaceBikeFile(TimeSpan time, ImmutableSpeed speed, Queue<GameField> entities)
        {
            LatestBestTime = time;
            Speed = speed;
            Entities = entities;
        }

        public static RaceBikeFile Parse(List<string> lines)
        {
            TimeSpan time;
            ImmutableSpeed speed;
            Queue<GameField> entities = new();

            try
            {
                var info = new DateTimeFormatInfo();
                // time
                time = TimeSpan.Parse(lines[0], new DateTimeFormatInfo());
                // speed
                speed = ImmutableSpeed.Parse(lines[1]);

                // coordinates of entities
                for (int i = 2; i < lines.Count; i++)
                {
                    entities.Enqueue(GameField.Parse(lines[i]));
                }
            }
            catch (Exception ex)
            {
                throw new RaceBikeDataException(ex.Message, ex);
            }

            return new RaceBikeFile(time, speed, entities);
        }
    }
}
