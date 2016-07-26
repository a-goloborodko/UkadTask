using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using UkadTask.Domain;

namespace UkadTask.Repository.Mapping
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Url)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("Pages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HostId).HasColumnName("HostId");
            this.Property(t => t.Url).HasColumnName("Url");
            this.Property(t => t.MinResponseTime).HasColumnName("MinResponseTime");
            this.Property(t => t.MaxResponseTime).HasColumnName("MaxResponseTime");

            // Relationships
            this.HasRequired(t => t.Host)
                .WithMany(t => t.Pages)
                .HasForeignKey(d => d.HostId);

        }
    }
}
