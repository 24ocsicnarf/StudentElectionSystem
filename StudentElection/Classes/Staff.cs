using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentElection.G;

namespace StudentElection.Classes
{
    public class Staff : User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static class Staffs
    {
        private static StaffTableAdapter _tableAdapter = new StaffTableAdapter();
        private static Dictionary<int, Staff> _dictionary = new Dictionary<int, Staff>();
        private static bool _isLoaded = false;

        public static Staff LogIn(string username, string password)
        {
            if (!_isLoaded) LoadData();

            var row = _dictionary.Where(x => x.Value.Username == username && x.Value.Password == password);
            if (row.Count() == 1)
            {
                return row.First().Value;
            }
            else
            {
                return null;
            }
        }

        public static Dictionary<int, Staff> Dictionary
        {
            get
            {
                return _dictionary;
            }
        }

        public static void InsertData(Staff staff)
        {
            _tableAdapter.InsertQuery(staff.LastName, staff.FirstName, staff.MiddleName, (byte)staff.Type, staff.Username, staff.Password);
            staff.ID = (int)_tableAdapter.GetLastID();
            _dictionary.Add(staff.ID, staff);
        }

        public static void UpdateData(Staff staff)
        {
            _tableAdapter.UpdateQuery(staff.LastName, staff.FirstName, staff.MiddleName, (byte)staff.Type, staff.Username, staff.Password, staff.ID);
            _dictionary[staff.ID] = staff;
        }

        public static void DeleteData(int id)
        {
            Users.DeleteData(id);
            _dictionary.Remove(id);
        }

        private static void LoadData()
        {
            var rows = _tableAdapter.GetData();

            foreach (var row in rows)
            {
                var staff = new Staff()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    LastName = Convert.ToString(row["LastName"]),
                    FirstName = Convert.ToString(row["FirstName"]),
                    MiddleName = Convert.ToString(row["MiddleName"]),
                    Type = (UserType)Convert.ToByte(row["Type"]),
            
                    Username = Convert.ToString(row["Username"]),
                    Password = Convert.ToString(row["Password"])
                };

                _dictionary.Add(staff.ID, staff);
            }
            _isLoaded = true;
        }
    }
}
