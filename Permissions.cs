using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Commentator
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission AddCommentsAccess = new Permission("AddCommentAccess", "Post Comments");
        public static readonly Permission EditCommentsAccess = new Permission("EditCommentAccess", "Edit Comments");
        public static readonly Permission DeleteCommentsAccess = new Permission("DeleteCommentAccess", "Delete Comments");
        public static readonly Permission ViewCommentsAccess = new Permission("ViewCommentAccess", "View Comments");

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[]
                    {
                        AddCommentsAccess,
                        ViewCommentsAccess
                    }
                },
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[]
                    {
                        AddCommentsAccess,
                        EditCommentsAccess,
                        DeleteCommentsAccess,
                        ViewCommentsAccess
                    }
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[]
                    {
                        AddCommentsAccess,
                        EditCommentsAccess,
                        DeleteCommentsAccess,
                        ViewCommentsAccess
                    }
                }
            };
        }

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[] {
                AddCommentsAccess,
                EditCommentsAccess,
                DeleteCommentsAccess,
                ViewCommentsAccess
            }
            .AsEnumerable());
        }
    }
}
