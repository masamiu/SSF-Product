using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suityou.Framework.Web.DataModel
{
    public class DataDefinition
    {
        public DataInformation DataInformation { get; set; }
    }

    public class DataInformation
    {
        public string? DataID { get; set; }
        public string? DataName { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? UseDownload { get; set; }
        public bool? UseUpload { get; set; }
        public bool? NotUseSearch { get; set; }
        public TableDefinition? MainTable { get; set; }
        public TableDefinition[]? SubTables { get; set; }
    }

    public class TableDefinition
    {
        public string? TableName { get; set; }
        public ColumnDefinition[]? Columns { get; set; }
        public KeyReferenceInformation[]? KeyReference  { get; set; }
        public string[]? Sort { get; set; }
    }

    public class ColumnDefinition
    {
        public string? ColumnName { get; set; }
        public string? ColumnType { get; set; }
        public int? Length { get; set; }
        public string? Accuracy { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
        public ExtenstionAttributes? ExtAttrs { get; set; }
    }

    public class ExtenstionAttributes
    {
        public string? DisplayName { get; set; }
        public bool Hidden { get; set; }
        public bool ListHidden { get; set; }
        public bool IsEditable { get; set; }
        public bool? IsNewTimeStamp { get; set; }
        public bool? IsUpdateTimeStamp { get; set; }
        public ReferenceTableInformation? ReferenceTo { get; set; }
        public SelectValueInfomation? SelectValue { get; set; }
    }

    public class SelectValueInfomation
    {
        public string SelectType { get; set; }
        public string SelectValueField { get; set; }
        public string SelectValue { get; set; }
    }

    public class KeyReferenceInformation
    {
        public string ColumnName { get; set; }
    }

    public class ReferenceTableInformation
    {
        public string TableName { get; set; }
        public string ValueColumnName { get; set; }
        public string CaptionColumnName { get; set; }
    }
}
