using AutoMapper;
using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.AutoMapper
{
    public class PostgreSQLProfile : Profile
    {
        public PostgreSQLProfile()
        {
            MapUsers();
            MapElections();
            MapParties();
            MapPositions();
            MapCandidates();
            MapVoters();
            MapBallots();
            MapVotes();
        }

        public void MapUsers()
        {
            CreateMap<User, UserModel>()
                .ForMember(m => m.Type, o => o.MapFrom(s => (UserType)s.Type));

            CreateMap<UserModel, User>()
                .ForMember(m => m.Type, o => o.MapFrom(s => (int)s.Type));
        }

        public void MapElections()
        {
            CreateMap<Election, ElectionModel>()
                .ForMember(m => m.Parties, o => o.Ignore())
                .ForMember(m => m.Positions, o => o.Ignore())
                .ForMember(m => m.Voters, o => o.Ignore());

            CreateMap<ElectionModel, Election>()
                .ForMember(m => m.Parties, o => o.Ignore())
                .ForMember(m => m.Positions, o => o.Ignore())
                .ForMember(m => m.Voters, o => o.Ignore());
        }

        public void MapParties()
        {
            CreateMap<Party, PartyModel>()
                .ForMember(m => m.Candidates, o => o.Ignore());
                //.ForMember(m => m.Election, o => o.ExplicitExpansion());

            CreateMap<PartyModel, Party>()
                .ForMember(m => m.Candidates, o => o.Ignore());
        }

        public void MapPositions()
        {
            CreateMap<Position, PositionModel>()
                .ForMember(m => m.Candidates, o => o.Ignore());
                //.ForMember(m => m.Election, o => o.ExplicitExpansion());

            CreateMap<PositionModel, Position>()
                .ForMember(m => m.Candidates, o => o.Ignore());
        }

        public void MapCandidates()
        {
            CreateMap<Candidate, CandidateModel>()
                .ForMember(m => m.Votes, o => o.Ignore())
                //.ForMember(m => m.Party, o => o.MapFrom(s => s.Party))
                //.ForMember(m => m.Position, o => o.MapFrom)
                .ForMember(m => m.Sex, o => o.MapFrom(s => (Sex)s.Sex));

            CreateMap<CandidateModel, Candidate>()
                .ForMember(m => m.Votes, o => o.Ignore())
                .ForMember(m => m.Sex, o => o.MapFrom(s => (int)s.Sex));
        }

        public void MapVoters()
        {
            CreateMap<Voter, VoterModel>()
                //.ForMember(m => m.Ballots, o => o.Ignore())
                //.ForMember(m => m.Election, o => o.ExplicitExpansion())
                .ForMember(m => m.Sex, o => o.MapFrom(s => (Sex)s.Sex));

            CreateMap<VoterModel, Voter>()
                .ForMember(m => m.Ballots, o => o.Ignore())
                .ForMember(m => m.Sex, o => o.MapFrom(s => (int)s.Sex));
        }

        private void MapBallots()
        {
            CreateMap<Ballot, BallotModel>()
                .ForMember(m => m.Votes, o => o.Ignore());
                //.ForMember(m => m.Voter, o => o.ExplicitExpansion());

            CreateMap<BallotModel, Ballot>()
                .ForMember(m => m.Votes, o => o.Ignore());
        }

        private void MapVotes()
        {
            CreateMap<Vote, VoteModel>();
                //.ForMember(m => m.Ballot, o => o.ExplicitExpansion())
                //.ForMember(m => m.Candidate, o => o.ExplicitExpansion());

            CreateMap<VoteModel, Vote>();
        }
    }
}
