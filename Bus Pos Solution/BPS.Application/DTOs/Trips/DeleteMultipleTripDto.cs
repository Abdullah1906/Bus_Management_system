using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Trips
{
    public class DeleteMultipleTripDto
    {
        public List<long> Ids { get; set; } = new();
    }
}
