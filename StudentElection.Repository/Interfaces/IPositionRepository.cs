using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface IPositionRepository
    {
        Task<IEnumerable<PositionModel>> GetPositionsAsync(int electionId);
        Task<int> GetPositionsCountAsync(int electionId);
        Task<PositionModel> GetPositionAsync(int positionId);
        Task InsertPositionAsync(PositionModel model);
        Task UpdatePositionAsync(PositionModel model);
        Task DeletePositionAsync(PositionModel model);
        Task MoveRankAsync(PositionModel selectedPosition, PositionModel closestPositionModel);
    }
}
