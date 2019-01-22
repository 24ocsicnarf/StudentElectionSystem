using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentElection.G;

namespace StudentElection.Classes
{
    public class User
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public UserType Type { get; set; }

        public string FullName
        {
            get
            {
                return (LastName.ToUpper() + ", " + FirstName + " " + MiddleName).Trim();
            }
        }
        public string TypeAbbr
        {
            get
            {
                if (Type == UserType.SuperAdmin)
                {
                    return "SA";
                }
                else if (Type == UserType.Admin)
                {
                    return "A";
                }
                else
                {
                    return "V";
                }
            }
        }
    }

    public static class Users
    {
        private static UserTableAdapter _tableAdapter = new UserTableAdapter();

        public static void DeleteData(int id)
        {
            _tableAdapter.Delete(id);
        }
    }
}
