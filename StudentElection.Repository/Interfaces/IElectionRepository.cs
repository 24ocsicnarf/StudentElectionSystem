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
        Task<ElectionModel> GetElectionByServerTagAsync(string tag);
        Task FinalizeCandidatesAsync(int electionId, DateTime dateTime);
        Task CloseElectionAsync(int electionId, DateTime dateTime);
        Task UpdateServerTagAsync(int electionId, string tag);
        Task InsertElectionAsync(ElectionModel model);
        Task UpdateElectionAsync(ElectionModel election);
    }
}
