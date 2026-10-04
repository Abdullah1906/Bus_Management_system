using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<string>> GetPermissionsByRoleAsync(
            string roleName);
    }
}
