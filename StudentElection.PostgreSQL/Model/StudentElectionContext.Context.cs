﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentElection.PostgreSQL.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StudentElectionContext : DbContext
    {
        public StudentElectionContext()
            : base("name=StudentElectionContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Ballot> Ballots { get; set; }
        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<Election> Elections { get; set; }
        public virtual DbSet<Party> Parties { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }
        public virtual DbSet<Voter> Voters { get; set; }
    }
}
