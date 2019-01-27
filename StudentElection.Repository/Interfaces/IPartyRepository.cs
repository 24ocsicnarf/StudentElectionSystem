using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface IPartyRepository
    {
        Task<IEnumerable<PartyModel>> GetPartiesAsync(int electionId);
        Task<PartyModel> GetPartyAsync(int partyId);
        Task InsertPartyAsync(PartyModel model);
        Task UpdatePartyAsync(PartyModel model);
        Task DeletePartyAsync(PartyModel model);
        Task<int> GetPartiesCountAsync(int electionId);
    }
}
