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
        Task<IEnumerable<PositionModel>> GetPositionsByYearLevelAsync(int electionId, int yearLevel);
        Task<PositionModel> GetPositionAsync(int positionId);
        Task<PositionModel> GetPositionByTitleAsync(int electionId, string positionTitle);
        Task<int> CountPositionsAsync(int electionId);
        Task<int> GetMaxRankAsync(int electionId);
        Task<bool> IsPositionTitleExistingAsync(int electionId, string positionTitle, PositionModel editingPosition);
        Task SwitchRankAsync(PositionModel selectedPosition, PositionModel closestPosition);
        Task InsertPositionAsync(PositionModel model);
        Task UpdatePositionAsync(PositionModel model);
        Task DeletePositionAsync(PositionModel model);
    }
}
