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
                        status = "To-Do",
                        visible = 1
                    },

                    new Status
                    {
                        Id = 2,
                        status = "Doing",
                        visible = 1

                    },
                    new Status
                    {
                        Id = 3,
                        status = "Done",
                        visible = 1
                    },
                       new Status
                       {
                           Id = 4,
                           status = "Reject",
                           visible = 1
                       }


                );
                if (context.Permission.Any())
                {
                    return;   // DB has been seeded
                }
                context.Permission.AddRange(
                    new Permission
                    {
                        Id = 1,
                        permission = "Admin",
                        visible = 1
                    },

                    new Permission
                    {
                        Id = 2,
                        permission = "Approve",
                        visible = 1

                    },
                    new Permission
                    {
                        Id = 3,
                        permission = "Owner",
                        visible = 1
                    }
                );
                if (context.Department.Any())
                {
                    return;   // DB has been seeded
                }
                context.Department.AddRange(
                    new Department
                    {
                        Id = 1,
                        department = "BPBI",
                        visible = 1
                    }


                );
                if (context.Owner.Any())
                {
                    return;   // DB has been seeded
                }
                context.Owner.AddRange(
                    new Owner
                    {
                        Id = 1,
                        own = "admin",
                        visible = 1,
                        pswd = "p@ssw0rd",
                        PermissionId = 1,
                        DepartmentId =1
                    }

                );

                if (context.UserPer.Any())
                {
                    return;   // DB has been seeded
                }
                context.UserPer.AddRange(
                    new UserPer
                    {
                        Id = 1,
                        OwnerId = 1,
                        visible = 1 ,
                        PermissionsId = 1,
                       
                    }
                );

                
                context.SaveChanges();
            }
        }
    }
}