using OnlineMuhasebeServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMuhasebeServer.Domain.CompanyEntities
{
    public sealed class UniformChartOfAccount : Entities
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public char Type { get; set; }// A: Asset, L: Liability, E: Equity, R: Revenue, C: Cost, I: Income
        public string CompanyId { get; set; }
    }
}
