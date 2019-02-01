using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface IElectionRepository
    {
        Task<ElectionModel> GetCurrentElectionAsync();
        Task FinalizeCandidatesAsync(int electionId, DateTime dateTime);
        Task CloseElectionAsync(int electionId, DateTime dateTime);
        Task UpdateTagAsync(int electionId, string tag);
        Task InsertElectionAsync(ElectionModel model);
        Task UpdateElectionAsync(ElectionModel election);
    }
}
