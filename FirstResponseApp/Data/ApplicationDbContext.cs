using System;
using System.Collections.Generic;
using System.Text;
using FirstResponseApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstResponseApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>//IdentityDbContext
    {
        public ApplicationDbContext() 
        {
            this.Database.SetCommandTimeout(150000);
           
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<TbAuditHistoryMaster> TbAuditHistoryMaster { get; set; }
        public virtual DbSet<TbChannelMaster> TbChannelMaster { get; set; }
        public virtual DbSet<TbOrganisationMaster> TbOrganisationMaster { get; set; }
        public virtual DbSet<TbTicketDocuments> TbTicketDocuments { get; set; }
        public virtual DbSet<TbTicketMaster> TbTicketMaster { get; set; }
        public virtual DbSet<TbTicketResponse> TbTicketResponse { get; set; }
        public virtual DbSet<TbTicketStatusMaster> TbTicketStatusMaster { get; set; }

        public virtual DbSet<EmailTemplates> EmailTemplates { get; set; }
        public virtual DbSet<SmtpSetting> SmtpSetting { get; set; }

        public virtual DbSet<TbTicketResponseDocuments> TbTicketResponseDocuments { get; set; }
        //   public virtual DbSet<TbUsersMaster> TbUsersMaster { get; set; }
        //     public virtual DbSet<TbWaitingOnMaster> TbWaitingOnMaster { get; set; }

    }
}
