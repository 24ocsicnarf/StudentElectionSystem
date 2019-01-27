using AutoMapper;
using StudentElection.PostgreSQL.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.Repositories
{
    public abstract class Repository
    {
        protected readonly IMapper _mapper;

        public Repository()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<PostgreSQLProfile>();
            });

            _mapper = config.CreateMapper();
        }
    }
}
