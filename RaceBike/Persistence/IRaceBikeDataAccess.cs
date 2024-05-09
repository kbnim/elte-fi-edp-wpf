using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model;

namespace RaceBike.Persistence
{
    public interface IRaceBikeDataAccess
    {
        #region Methods
        Task<RaceBikeFile> LoadAsync(string path);
        Task SaveAsync(string path, RaceBikeFile fileContents);
        #endregion
    }
}
