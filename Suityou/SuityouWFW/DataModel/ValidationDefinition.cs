using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.DataModel
{
    public class ValidationDefinition
    {
        public ValidationInformation ValidationInformation { get; set; }
    }

    public class ValidationInformation
    {
        public string? DataID { get; set; }
        public string? DataName { get; set; }
        public ColumnValidationDefinition[]? Columns { get; set; }
    }

    public class ColumnValidationDefinition
    {
        public string? ColumnName { get; set; }
        public Validation[]? Validations { get; set; }
    }

    public class Validation
    {
        public string? Type { get; set; }
        public int? Length { get; set; }
        public string? Values { get; set; }
    }
}
