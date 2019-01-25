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
    public class PositionRepository : IPositionRepository
    {
        public async Task<IEnumerable<PositionModel>> GetPositionsAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Positions
                    .Where(p => p.ElectionId == electionId)
                    .OrderBy(p => p.Rank)
                    .ProjectTo<PositionModel>().ToListAsync();
            }
        }

        public async Task InsertPositionAsync(PositionModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var position = new Position();
                Mapper.Map(model, position);

                context.Positions.Add(position);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePositionAsync(PositionModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions.SingleOrDefaultAsync(p => p.Id == model.Id);
                Mapper.Map(model, position);

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

        public async Task MoveRankAsync(PositionModel selectedPosition, PositionModel closestPosition)
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

        public async Task<int> GetPositionsCountAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Positions
                    .Where(p => p.ElectionId == electionId)
                    .CountAsync();
            }
        }

        public async Task<PositionModel> GetPositionAsync(int positionId)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions
                    .SingleOrDefaultAsync(p => p.Id == positionId);

                var model = new PositionModel();
                Mapper.Map(position, model);

                return model;
            }
        }

        public async Task<PositionModel> GetPositionByTitleAsync(int electionId, string positionTitle)
        {
            using (var context = new StudentElectionContext())
            {
                var position = await context.Positions
                    .SingleOrDefaultAsync(p => p.ElectionId == electionId && p.Title.ToLower() == positionTitle.ToLower());

                var model = new PositionModel();
                Mapper.Map(position, model);

                return model;
            }
        }

        public Task<IEnumerable<PositionModel>> GetPositionsByYearLevelAsync(int electionId, int yearLevel)
        {
            throw new NotImplementedException();
        }
    }
}
