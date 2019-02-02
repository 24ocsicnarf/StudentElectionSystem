using AutoMapper;
using AutoMapper.QueryableExtensions;
using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.Repositories
{
    public class PositionRepository : Repository, IPositionRepository
    {
        public async Task<IEnumerable<PositionModel>> GetPositionsAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                var positions = context.Positions.Where(p => p.ElectionId == electionId)
                    .OrderBy(p => p.Rank);

                return await _mapper.ProjectTo<PositionModel>(positions)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsByYearLevelAsync(int electionId, int yearLevel)
        {
            using (var context = new StudentElectionContext())
            {
                var positions = context.Positions
                    .Where(p => p.ElectionId == electionId
                        && (p.YearLevel == null || p.YearLevel == yearLevel))
                    .OrderBy(p => p.Rank);

                return await _mapper.ProjectTo<PositionModel>(positions)
                    .ToListAsync();
            }
        }


        public async Task<PositionModel> GetPositionAsync(int positionId)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions.SingleOrDefaultAsync(p => p.Id == positionId);
                if (position == null)
                {
                    return null;
                }

                var model = new PositionModel();
                _mapper.Map(position, model);

                return model;
            }
        }

        public async Task<PositionModel> GetPositionByTitleAsync(int electionId, string positionTitle)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions
                    .SingleOrDefaultAsync(p => p.ElectionId == electionId && p.Title.ToLower() == positionTitle.ToLower());
                if (position == null)
                {
                    return null;
                }

                var model = new PositionModel();
                _mapper.Map(position, model);

                return model;
            }
        }

        public async Task<int> CountPositionsAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Positions
                    .Where(p => p.ElectionId == electionId)
                    .CountAsync();
            }
        }
        
        public async Task SwitchRankAsync(PositionModel selectedPosition, PositionModel closestPosition)
        {
            using (var context = new StudentElectionContext())
            {
                var sPosition = await context.Positions.SingleOrDefaultAsync(p => p.Id == selectedPosition.Id);
                sPosition.Rank = closestPosition.Rank;

                var cPosition = await context.Positions.SingleOrDefaultAsync(p => p.Id == closestPosition.Id);
                cPosition.Rank = selectedPosition.Rank;

                await context.SaveChangesAsync();
            }
        }

        public async Task InsertPositionAsync(PositionModel model)
        {
            var position = new Position();
            _mapper.Map(model, position);

            using (var context = new StudentElectionContext())
            {
                context.Positions.Add(position);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePositionAsync(PositionModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions.SingleOrDefaultAsync(p => p.Id == model.Id);
                _mapper.Map(model, position);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeletePositionAsync(PositionModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions.SingleOrDefaultAsync(p => p.Id == model.Id);
                context.Positions.Remove(position);

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsPositionTitleExistingAsync(int electionId, string title, PositionModel editingPosition)
        {
            var position = await GetPositionByTitleAsync(electionId, title);

            if (editingPosition == null)
            {
                return position != null;
            }
            else
            {
                return position != null && position.Id != editingPosition.Id;
            }
        }

        public async Task<int> GetMaxRankAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Positions
                    .Where(p => p.ElectionId == electionId)
                    .MaxAsync(p => p.Rank);
            }
        }
    }
}
