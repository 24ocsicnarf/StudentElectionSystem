using AutoMapper;
using AutoMapper.QueryableExtensions;
using StudentElection.MSAccess.StudentElectionDataSetTableAdapters;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static StudentElection.MSAccess.StudentElectionDataSet;

namespace StudentElection.MSAccess.Repositories
{
    public class PositionRepository : Repository, IPositionRepository
    {
        public async Task<IEnumerable<PositionModel>> GetPositionsAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                var positions = tableAdapter.GetData(electionId)
                    .AsQueryable();

                return _mapper.ProjectTo<PositionModel>(positions);
            }
        }

        public async Task InsertPositionAsync(PositionModel model)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                tableAdapter.Insert(
                    model.Title,
                    model.WinnersCount,
                    model.Rank,
                    model.ElectionId,
                    model.YearLevel
                );
            }
        }

        public async Task UpdatePositionAsync(PositionModel model)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                tableAdapter.Update(
                    model.Title,
                    model.WinnersCount,
                    model.Rank,
                    model.ElectionId,
                    model.YearLevel,
                    model.Id
                );
            }
        }

        public async Task DeletePositionAsync(PositionModel model)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                tableAdapter.Delete(model.Id);
            }
        }

        public async Task MoveRankAsync(PositionModel selectedPosition, PositionModel closestPosition)
        {
            await Task.CompletedTask;

            using (var manager = new TableAdapterManager())
            {
                manager.PositionTableAdapter = new PositionTableAdapter();

                manager.PositionTableAdapter.UpdateRankQuery(
                    selectedPosition.Rank,
                    closestPosition.Id
                );

                manager.PositionTableAdapter.UpdateRankQuery(
                    closestPosition.Rank,
                    selectedPosition.Id
                );
            }
        }

        public async Task<int> GetPositionsCountAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                return tableAdapter.CountPositionsQuery(electionId) ?? 0;
            }
        }

        public async Task<PositionModel> GetPositionAsync(int positionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                var row = tableAdapter.GetPositions(positionId).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new PositionModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task<PositionModel> GetPositionByTitleAsync(int electionId, string positionTitle)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                var row = tableAdapter.GetPositionsByTitle(electionId, positionTitle).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new PositionModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsByYearLevelAsync(int electionId, int yearLevel)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                var positions = tableAdapter.GetPositionsByYearLevel(electionId, yearLevel)
                    .AsQueryable();

                return _mapper.ProjectTo<PositionModel>(positions);
            }
        }
    }
}
