using LNHSVoting.LNHSVotingDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNHSVoting.Classes
{
    public class Position
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public static class Positions
    {
        private static PositionTableAdapter _tableAdapter = new PositionTableAdapter();
        private static Dictionary<int, Position> _dictionary = new Dictionary<int, Position>();
        private static bool _isLoaded = false;

        public static Dictionary<int, Position> Dictionary
        {
            get
            {
                if (!_isLoaded) LoadData();

                return _dictionary;
            }
        }

        public static void InsertData(Position position)
        {
            _tableAdapter.Insert(position.Title, position.Order);
            position.ID = (int)_tableAdapter.GetLastID();
            _dictionary.Add(position.ID, position);
        }

        public static void UpdateData(Position position)
        {
            _tableAdapter.UpdateQuery(position.Title, position.Order, position.ID);
            Candidates.UpdatePosition(position);

            _dictionary[position.ID] = position;
        }

        public static void DeleteData(int id)
        {
            _tableAdapter.DeleteQuery(id);
            _dictionary.Remove(id);

            var deletedIds = Candidates.Dictionary.Where(x => x.Value.Position.ID == id).Select(x => x.Key).ToList();
            foreach (var key in deletedIds)
                Candidates.DeleteData(key);
        }

        private static void LoadData()
        {
            var rows = _tableAdapter.GetData();

            foreach (var row in rows)
            {
                var position = new Position()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Title = Convert.ToString(row["Title"]),
                    Order = Convert.ToInt32(row["Order"])
                };
                _dictionary.Add(position.ID, position);
            }
            _isLoaded = true;
        }
    }
}