using AutoMapper;
using StudentElection.MSAccess.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.MSAccess.Repositories
{
    public abstract class Repository
    {
        protected readonly IMapper _mapper;

        public Repository()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MSAccessProfile>();
            });

            _mapper = config.CreateMapper();
        }
    }
}
