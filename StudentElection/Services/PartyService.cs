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
    public class PartyService
    {
        private IPartyRepository _partyRepository;

        public PartyService()
        {
            _partyRepository = RepositoryFactory.Get<IPartyRepository>();
        }

        public async Task<IEnumerable<PartyModel>> GetPartiesAsync(int electionId)
        {
            return await _partyRepository.GetPartiesAsync(electionId);
        }

        public async Task<PartyModel> GetPartyAsync(int partyId)
        {
            return await _partyRepository.GetPartyAsync(partyId);
        }

        public async Task DeletePartyAsync(PartyModel party)
        {
            await _partyRepository.DeletePartyAsync(party);
        }

        public async Task InsertPartyAsync(PartyModel party)
        {
            await _partyRepository.InsertPartyAsync(party);
        }

        public async Task UpdatePartyAsync(PartyModel party)
        {
            await _partyRepository.UpdatePartyAsync(party);
        }

        public async Task<int> GetPartiesCount(int electionId)
        {
            return await _partyRepository.GetPartiesCountAsync(electionId);
        }
    }
}
