using StudentElection.Repository;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Services
{
    public class PositionService
    {
        private readonly IPositionRepository _positionRepository;

        public PositionService()
        {
            _positionRepository = RepositoryFactory.Get<IPositionRepository>();
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsAsync(int electionId)
        {
            return await _positionRepository.GetPositionsAsync(electionId);
        }

        public async Task<PositionModel> GetPositionAsync(int positionId)
        {
            return await _positionRepository.GetPositionAsync(positionId);
        }

        public async Task<int> GetPositionsCountAsync(int electionId)
        {
            return await _positionRepository.GetPositionsCountAsync(electionId);
        }

        public async Task SavePositionAsync(PositionModel position)
        {
            if (position.Id == 0)
            {
                await _positionRepository.InsertPositionAsync(position);
            }
            else
            {
                await _positionRepository.UpdatePositionAsync(position);
            }
        }

        public async Task DeletePositionAsync(PositionModel position)
        {
            await _positionRepository.DeletePositionAsync(position);
        }
        
        public async Task MoveRankAsync(PositionModel selectedPosition, PositionModel closestPosition)
        {
            await _positionRepository.MoveRankAsync(selectedPosition, closestPosition);
        }
    }
}
