using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface ITokenGenerator
    {
        string GeneratePasswordResetToken();

        string HashToken(string token);
    }
}
