using BPS.Application.DTOs.Reports;
using BPS.Application.Interfaces;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BPS.Infrastructure.Services
{
    public sealed class ReportPdfGenerator : IReportPdfGenerator
    {
        public async Task<byte[]> GenerateAsync(
            IReadOnlyCollection<ReportDto> reports,
            ReportFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var html = BuildHtml(reports, filter);

            using var playwright = await Playwright.CreateAsync();

            await using var browser =
                await playwright.Chromium.LaunchAsync(
                    new BrowserTypeLaunchOptions
                    {
                        Headless = true
                    });

            var page = await browser.NewPageAsync();

            await page.SetContentAsync(
                html,
                new PageSetContentOptions
                {
                    WaitUntil = WaitUntilState.Load
                });

            cancellationToken.ThrowIfCancellationRequested();


            return await page.PdfAsync(
                new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true,
                    PreferCSSPageSize = true,
                    DisplayHeaderFooter = true,

                    Margin = new Margin
                    {
                        Top = "15mm",
                        Bottom = "15mm",
                        Left = "12mm",
                        Right = "12mm"
                    },

                    HeaderTemplate = """
                    <div style="
                        font-family: Arial, sans-serif;
                        font-size: 8px;
                        width: 100%;
                        text-align: center;
                        color: #64748b;">
                        BPS | Trip Report
                    </div>
                    """,

                    FooterTemplate = """
                    <div style="
                        font-family: Arial, sans-serif;
                        font-size: 8px;
                        width: 100%;
                        padding: 0 12mm;
                        text-align: center;
                        color: #64748b;">
                        Page <span class="pageNumber"></span>
                        of <span class="totalPages"></span>
                    </div>
                    """
                            });

        }

        private static string Encode(string? value) =>
            WebUtility.HtmlEncode(value ?? string.Empty);

        private static string Money(decimal value) =>
            value.ToString("#,##0.00", CultureInfo.InvariantCulture);
        private static string Number(decimal value) =>
            value.ToString("#,##0", CultureInfo.InvariantCulture);

        private static string BuildHtml(
            IReadOnlyCollection<ReportDto> reports,
            ReportFilterDto filter)
        {
            var rows = new StringBuilder();

            decimal totalAmount = 0;
            decimal totalTips = 0;

            var to = reports.Count > 0
                ? reports.Max(r => r.ReportDate)
                    .ToString("dd MMM yyyy", CultureInfo.InvariantCulture)
                : "No records";

            var from = reports.Count > 0
                ? reports.Min(r => r.ReportDate)
                    .ToString("dd MMM yyyy", CultureInfo.InvariantCulture)
                : "No records";

            foreach (var report in reports)
            {
                totalAmount += report.Total;

                if (report.TipStatus)
                    totalTips += report.TipAmount;

                rows.Append($"""
                <tr>
                    <td class="date">{report.ReportDate:dd MMM yyyy}</td>
                    <td class="place">{Encode(report.PlaceName)}</td>
                    <td class="amount">{Money(report.Price)}</td>
                    <td class="amount">
                        {(report.TipStatus ? Number(report.TipAmount) : "—")}
                    </td>
                </tr>
                """);
            }

             if (reports.Count == 0)
                {
                  rows.Append("""
                        <tr>
                            <td colspan="4" class="empty">
                                No report records found.
                            </td>
                        </tr>
                   """);
             }

           return $$"""
            <!DOCTYPE html>
            <html lang="bn">
            <head>
                <meta charset="utf-8">

                <style>
                    @page {
                        size: A4;
                        margin: 20mm 12mm 20mm 12mm;
                    }

                    * {
                        box-sizing: border-box;
                    }

                    body {
                        font-family: "Noto Sans Bengali",
                                     "Nirmala UI", Arial, sans-serif;
                        font-size: 10pt;
                        color: #243247;
                        margin: 0;
                        line-height: 1.5;
                    }

                    .report-header {
                        text-align: center;
                        padding-bottom: 16px;
                        border-bottom: 2px solid #1e3a5f;
                    }

                    h1 {
                        font-size: 23pt;
                        color: #17365d;
                        margin: 0 0 5px;
                        font-weight: 700;
                    }

                    .subtitle {
                        font-size: 10pt;
                        color: #64748b;
                        margin: 0;
                    }

                    .report-date {
                        margin-top: 10px;
                        font-size: 9pt;
                        color: #475569;
                    }

                    .meta {
                        text-align: center;
                        margin: 16px 0;
                        padding: 10px;
                        background: #f1f5f9;
                        border: 1px solid #dbe3ed;
                        border-radius: 5px;
                    }

                    .meta-title {
                        font-size: 9pt;
                        color: #64748b;
                        margin-bottom: 3px;
                    }

                    .meta-value {
                        font-size: 10pt;
                        font-weight: 600;
                        color: #1e293b;
                    }

                    .summary {
                        width: 100%;
                        margin: 16px 0 20px;
                        border-collapse: separate;
                        border-spacing: 10px 0;
                    }

                    .summary td {
                        width: 50%;
                        text-align: center;
                        padding: 13px 8px;
                        background: #f8fafc;
                        border: 1px solid #dbe3ed;
                        border-radius: 5px;
                    }

                    .summary-label {
                        display: block;
                        color: #64748b;
                        font-size: 9pt;
                        margin-bottom: 5px;
                    }

                    .summary-value {
                        display: block;
                        color: #17365d;
                        font-size: 15pt;
                        font-weight: 700;
                    }

                    .section-title {
                        font-size: 12pt;
                        color: #17365d;
                        font-weight: 700;
                        margin: 20px 0 9px;
                        text-align: center;
                    }

                    table.data {
                        width: 100%;
                        border-collapse: collapse;
                        table-layout: fixed;
                    }

                    table.data thead {
                        display: table-header-group;
                    }

                    table.data tr {
                        break-inside: avoid;
                        page-break-inside: avoid;
                    }

                    table.data th {
                        background: #17365d;
                        color: #ffffff;
                        font-size: 9pt;
                        font-weight: 600;
                        padding: 10px 8px;
                        border: 1px solid #17365d;
                        text-align: center;
                    }

                    table.data td {
                        padding: 9px 8px;
                        border: 1px solid #dbe3ed;
                        font-size: 9pt;
                        vertical-align: middle;
                    }

                    table.data tbody tr:nth-child(even) {
                        background: #f1f5f9;
                    }

                    table.data tbody tr:nth-child(odd) {
                        background: #ffffff;
                    }

                    .date {
                        width: 25%;
                        text-align: center;
                        white-space: nowrap;
                    }

                    .place {
                        width: 35%;
                        text-align: center;
                        overflow-wrap: anywhere;
                    }

                    .amount {
                        width: 20%;
                        text-align: center;
                        white-space: nowrap;
                        font-variant-numeric: tabular-nums;
                    }

                    .empty {
                        text-align: center;
                        padding: 22px !important;
                        color: #64748b;
                    }

                    .report-note {
                        margin-top: 14px;
                        text-align: center;
                        color: #64748b;
                        font-size: 8pt;
                    }
                </style>
            </head>

            <body>
                <header class="report-header">
                    <h1>ট্রিপ রিপোর্ট</h1>
                    <p class="subtitle">ভ্রমণ সংক্রান্ত বিস্তারিত রিপোর্ট</p>
            
                    <p class="report-date">
                        রিপোর্ট তৈরির তারিখ:
                        {{DateTime.Now.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}}
                    </p>
                </header>

                <div class="meta">
                    <div class="meta-title">রিপোর্টের সময়সীমা</div>
                    <div class="meta-value">
                        {{Encode(from)}} — {{Encode(to)}}
                    </div>
                    <div class="meta-title" style="margin-top: 5px;">
                        মোট রেকর্ড: {{reports.Count}}
                    </div>
                </div>

                <h2 class="section-title">বিস্তারিত ট্রিপ তালিকা</h2>

                <table class="data">
                    <thead>
                        <tr>
                            <th style="width:25%">তারিখ</th>
                            <th style="width:35%">স্থান</th>
                            <th style="width:20%">মূল্য</th>
                            <th style="width:20%">ট্রিপ</th>
                        </tr>
                    </thead>

                    <tbody>
                        {{rows}}
                    </tbody>
                </table>

                 <table class="summary">
                        <tr>
                            <td>
                                <span class="summary-label">মোট ট্রিপ (Total Tips)</span>
                                <span class="summary-value">
                                    {{Number(totalTips)}}
                                </span>
                            </td>
                            <td>
                                <span class="summary-label">
                                    মোট পরিমাণ (Total Amount)
                                </span>
                                <span class="summary-value">
                                    {{Money(totalAmount)}}
                                </span>
                            </td>
                        </tr>
                    </table>


                <p class="report-note">
                    BPS — System Generated Report
                </p>
            </body>
            </html>
            """;
        }

    }
}
