namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserModel
    {
        public UserModel()
        {
            this.AuditLogs = new HashSet<AuditLogModel>();
        }
    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public int Sex { get; set; }
        public UserType Type { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    
        public ICollection<AuditLogModel> AuditLogs { get; set; }

        public string FullName => $"{ this.FirstName } { this.MiddleName } { this.LastName } { this.Suffix }";
    }
}
