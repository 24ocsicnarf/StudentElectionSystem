using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Classes
{
    //public static class Machine
    //{
    //    private static string _tag = null;
    //    private static bool? _finalized = null, _viewed = null;
    //    private static int? _id = null;

    //    public static int ID
    //    {
    //        get
    //        {
    //            if (_id == null)
    //            {
    //                _id = Convert.ToInt32(_machineAdapter.GetData()[0]["ID"]);
    //            }
    //            return _id.Value;
    //        }
    //    }

    //    public static bool AreCandidatesFinalized
    //    {
    //        get
    //        {
    //            if (_finalized == null)
    //            {
    //                _finalized = Convert.ToBoolean(_machineAdapter.GetData()[0]["IsFinalized"]);
    //            }
    //            return _finalized.Value;
    //        }
    //        set
    //        {
    //            value = _finalized.Value;
    //        }
    //    }
    //    public static bool AreResultsViewed
    //    {
    //        get
    //        {
    //            if (_viewed == null)
    //            {
    //                _viewed = Convert.ToBoolean(_machineAdapter.GetData()[0]["IsResultsViewed"]);
    //            }
    //            return _viewed.Value;
    //        }
    //        set
    //        {
    //            value = _viewed.Value;
    //        }
    //    }
    //    public static string Tag
    //    {
    //        get
    //        {
    //            if (_tag == null)
    //            {
    //                _tag = Convert.ToString(_machineAdapter.GetData()[0]["Name"]);
    //            }
    //            return _tag;
    //        }
    //    }

    //    private static MachineTableAdapter _machineAdapter = new MachineTableAdapter();
        
    //    public static void FinalizeCandidates()
    //    {
    //        _machineAdapter.FinalizeCandidates();
    //        _finalized = true;
    //    }

    //    public static void Viewed()
    //    {
    //        _machineAdapter.MakeResultsViewed();
    //        _viewed = true;
    //    }

    //    public static void UpdateTag(string tag)
    //    {
    //        _machineAdapter.UpdateTag(tag);
    //        _tag = tag;
    //    }
    //}
}
