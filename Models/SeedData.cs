using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using qwerty.Data;
using System;
using System.Linq;

namespace qwerty.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new QwertyContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<QwertyContext>>()))
            {
                // Look for any movies.
                if (context.Status.Any())
                {
                    return;   // DB has been seeded
                }
                context.Status.AddRange(
                    new Status
                    {
                        Id = 1,
                        status = "To-Do"
                    },

                    new Status
                    {
                        Id = 2,
                        status = "Doing"

                    },
                    new Status
                    {
                        Id = 3,
                        status = "Done"
                    },
                       new Status
                       {
                           Id = 4,
                           status = "Reject"
                       }


                );
                if (context.Permission.Any())
                {
                    return;   // DB has been seeded
                }
                context.Permission.AddRange(
                    new Permission
                    {
                        Id = 0,
                        permission = "Owner"
                    },

                    new Permission
                    {
                        Id = 1,
                        permission = "Approve"

                    }
                );
                context.SaveChanges();
            }
        }
    }
}