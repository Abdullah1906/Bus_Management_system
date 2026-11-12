using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public long TotalRecords { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling((double)TotalRecords / PageSize)
                : 0;
    }
}
