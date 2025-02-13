using API.Domain.Notifications;
using API.Domain.Users.Auth;
using API.Domain.Users.Auth.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infra.SqlServer.ModelBuilders
{
    internal class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            Ignores(builder);
            Relationships(builder);
            Seed(builder);
        }

        private static void Relationships(EntityTypeBuilder<AppRole> builder)
        {
            builder.HasMany(entity => entity.AppUserRoles)
                   .WithOne(relationship => relationship.AppRole)
                   .HasForeignKey(ur => ur.RoleId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }

        private static void Seed(EntityTypeBuilder<AppRole> builder)
        {
            var appRoleFactory = new AppRoleFactory(new NotificationHandler());

            builder.HasData(
                appRoleFactory.DefaultBuilder()
                    .Name(Roles.ROLE_ADMIN_CREATE.Name)
                    .NormalizedName(Roles.ROLE_ADMIN_CREATE.Name.ToUpper())
                    .Id(Roles.ROLE_ADMIN_CREATE.Id)
                    .Raise()
            );
        }

        private static void Ignores(EntityTypeBuilder<AppRole> builder)
        {
        }
    }
}
