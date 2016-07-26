using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using UkadTask.Domain;

namespace UkadTask.Repository.Mapping
{
    public class HistoryMap : EntityTypeConfiguration<History>
    {
        public HistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("History");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PageId).HasColumnName("PageId");
            this.Property(t => t.ResponseTime).HasColumnName("ResponseTime");
            this.Property(t => t.Date).HasColumnName("Date");

            // Relationships
            this.HasRequired(t => t.Page)
                .WithMany(t => t.History)
                .HasForeignKey(d => d.PageId);

        }
    }
}
