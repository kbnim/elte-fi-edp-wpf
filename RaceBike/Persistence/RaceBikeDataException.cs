using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Persistence
{
    [Serializable]
    public class RaceBikeDataException : Exception
    {
        #region Constructors
        public RaceBikeDataException() { }

        public RaceBikeDataException(string? message) : base(message) { }

        public RaceBikeDataException(string? message, Exception? innerException) : base(message, innerException) { }

        protected RaceBikeDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
