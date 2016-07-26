using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using UkadTask.Domain;

namespace UkadTask.Repository.Mapping
{
    public class HostMap : EntityTypeConfiguration<Host>
    {
        public HostMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.HostName)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Hosts");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HostName).HasColumnName("Host");
        }
    }
}
