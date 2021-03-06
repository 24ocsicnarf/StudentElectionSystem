﻿using StudentElection.Repository;
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

        public async Task<int> CountPositionsAsync(int electionId)
        {
            return await _positionRepository.CountPositionsAsync(electionId);
        }

        public async Task<PositionModel> GetPositionByTitleAsync(int electionId, string positionTitle)
        {
            return await _positionRepository.GetPositionByTitleAsync(electionId, positionTitle);
        }

        public async Task SavePositionAsync(PositionModel position)
        {
            if (position.Id == 0)
            {
                int maxRank = await _positionRepository.GetMaxRankAsync(position.ElectionId);
                position.Rank = maxRank + 1;

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
            await _positionRepository.SwitchRankAsync(selectedPosition, closestPosition);
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsByYearLevelAsync(int electionId, int yearLevel)
        {
            return await _positionRepository.GetPositionsByYearLevelAsync(electionId, yearLevel);
        }

        public async Task<bool> IsPositionTitleExistingAsync(int electionId, string title, PositionModel editingPosition)
        {
            return await _positionRepository.IsPositionTitleExistingAsync(electionId, title, editingPosition);
        }
    }
}
