using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Classes
{
    public static class DB
    {
        public static StudentElectionDataSet DataSet = new StudentElectionDataSet();

        public static void LoadData()
        {
            var userAdapter = new UserTableAdapter();
            var userData = userAdapter.GetData();
            foreach (var row in userData)
            {
                var newRow = DataSet.User.NewUserRow();

                newRow.ID = row.ID;
                newRow.LastName = row.LastName;
                newRow.FirstName = row.FirstName;
                newRow.MiddleName = row.IsMiddleNameNull() ? "" : row.MiddleName;
                newRow.Type = row.Type;
                
                DataSet.User.AddUserRow(newRow);
            }

            var machineAdapter = new MachineTableAdapter();
            var machineData = machineAdapter.GetData();
            foreach (var row in machineData)
            {
                var newRow = DataSet.Machine.NewMachineRow();

                newRow.ID = row.ID;
                newRow.IsFinalized = row.IsFinalized;
                newRow.IsResultsViewed = row.IsResultsViewed;
                newRow.Name = row.IsNameNull() ? "" : row.Name;

                DataSet.Machine.AddMachineRow(newRow);
            }

            var staffAdapter = new StaffTableAdapter();
            var staffData = staffAdapter.GetData();
            foreach (var row in staffData)
            {
                var newRow = DataSet.Staff.NewStaffRow();

                newRow.UserRow = DataSet.User.FindByID(row.UserID);
                newRow.Username = row.Username;
                newRow.Password = row.Password;
                
                DataSet.Staff.AddStaffRow(newRow);
            }

            var voterAdapter = new VoterTableAdapter();
            var voterData = voterAdapter.GetData();
            foreach (var row in voterData)
            {
                var newRow = DataSet.Voter.NewVoterRow();

                newRow.ID = row.ID;
                newRow.GradeLevel = row.GradeLevel;
                newRow.StrandSection = row.StrandSection;
                newRow.IsMale = row.IsMale;
                newRow.Birthday = row.Birthday;
                newRow.VoterID = row.VoterID;
                newRow.IsForeign = row.IsForeign;
                newRow.MachineRow = DataSet.Machine.FindByID(row.MachineID);

                DataSet.Voter.AddVoterRow(newRow);
            }
        }
    }
}
