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
    public class PositionRepository : IPositionRepository
    {
        public async Task<IEnumerable<PositionModel>> GetPositionsAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                return tableAdapter.GetData(electionId)
                    .AsQueryable()
                    .ProjectTo<PositionModel>()
                    .AsEnumerable();
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
                    model.ElectionId
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

                var model = new PositionModel();
                Mapper.Map(row, model);

                return model;
            }
        }

        public async Task<PositionModel> GetPositionByTitleAsync(int electionId, string positionTitle)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PositionTableAdapter())
            {
                var row = tableAdapter.GetPositionsByTitle(electionId, positionTitle).SingleOrDefault();

                var model = new PositionModel();
                Mapper.Map(row, model);

                return model;
            }
        }
    }
}
