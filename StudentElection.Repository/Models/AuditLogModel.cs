namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuditLogModel
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int Action { get; set; }
        public System.DateTime OccuredAt { get; set; }
        public string RowData { get; set; }
        public string ChangedFields { get; set; }
        public int UserId { get; set; }
    
        public UserModel User { get; set; }
    }
}
