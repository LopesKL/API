using API.Domain.Users.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infra.SqlServer.ModelBuilders
{
    internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            Ignores(builder);
            Relationships(builder);
            //builder.ToTable(nameof(User));
        }

        private static void Relationships(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasMany(entity => entity.AppUserRoles)
                   .WithOne(relationship => relationship.AppUser)
                   .HasForeignKey(ur => ur.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }

        private static void Ignores(EntityTypeBuilder<AppUser> builder)
        {
        }
    }
}
