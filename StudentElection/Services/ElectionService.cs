using Project.Library;
using StudentElection.Repository;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Services
{
    public class ElectionService
    {
        private readonly IElectionRepository _electionRepository;

        public ElectionService()
        {
            _electionRepository = RepositoryFactory.Get<IElectionRepository>();
        }
        
        public async Task<ElectionModel> GetCurrentElectionAsync()
        {
            return await _electionRepository.GetCurrentElectionAsync();
        }

        public async Task FinalizeCandidatesAsync(int electionId)
        {
            var dateTime = DateTime.Now;

            await _electionRepository.FinalizeCandidatesAsync(electionId, dateTime);
        }

        public async Task CloseElectionAsync(int electionId)
        {
            var dateTime = DateTime.Now;

            await _electionRepository.CloseElectionAsync(electionId, dateTime);
        }

        public async Task OpenElectionAsync(ElectionModel model)
        {
            await _electionRepository.InsertElectionAsync(model);
        }

        public async Task UpdateTagAsync(int electionId, string tag)
        {
            await _electionRepository.UpdateTagAsync(electionId, tag);
        }

        public async Task UpdateElectionAsync(ElectionModel election)
        {
            await _electionRepository.UpdateElectionAsync(election);
        }
    }
}
