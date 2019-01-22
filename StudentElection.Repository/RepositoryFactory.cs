using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository
{
    public static class RepositoryFactory
    {
        public static T Get<T>() where T : class
        {
            string resolvedTypeName = ConfigurationManager.AppSettings[typeof(T).ToString()];
            var resolvedType = Type.GetType(resolvedTypeName);
            var instance = Activator.CreateInstance(resolvedType) as T;

            return instance;
        }
    }
}
