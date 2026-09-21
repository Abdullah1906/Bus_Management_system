using BPS.Application.DTOs.Common;
using BPS.Application.DTOs.Reports;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository
            _reportRepository;


        public ReportService(
            IReportRepository reportRepository)
        {
            _reportRepository =
                reportRepository;
        }


        public async Task<IEnumerable<ReportDto>> GetAsync(
            ReportFilterDto filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(
                    nameof(filter));
            }


            if (
                filter.FromDate.HasValue &&
                filter.ToDate.HasValue &&
                filter.FromDate.Value.Date >
                filter.ToDate.Value.Date)
            {
                throw new ArgumentException(
                    "From date cannot be greater than To date.");
            }


            if (
                filter.PlaceId.HasValue &&
                filter.PlaceId.Value <= 0)
            {
                throw new ArgumentException(
                    "Invalid place.");
            }


            return await _reportRepository
                .GetAsync(filter);
        }




        public async Task<PagedResult<ReportDto>> GetPagedAsync( ReportFilterDto filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(
                    nameof(filter));
            }


            if (filter.Page < 1)
            {
                filter.Page = 1;
            }



            if (filter.PageSize < 1)
            {
                filter.PageSize = 10;
            }

            if (filter.PageSize > 100)
            {
                filter.PageSize = 100;
            }



            if (
                filter.FromDate.HasValue &&
                filter.ToDate.HasValue &&
                filter.FromDate.Value.Date >
                filter.ToDate.Value.Date)
            {
                throw new ArgumentException(
                    "From date cannot be greater than To date.");
            }

            if (
                filter.PlaceId.HasValue &&
                filter.PlaceId.Value <= 0)
            {
                throw new ArgumentException(
                    "Invalid place.");
            }

            return await _reportRepository
                .GetPagedAsync(filter);
        }


    }
}
