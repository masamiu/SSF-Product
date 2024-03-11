using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suityou.Framework.Web.Common;

namespace Suityou.Framework.Web.DataModel
{
    public class PageForm
    {
        private readonly List<FormItem> _formItems = new List<FormItem>();
        public IEnumerable<FormItem> FormItems => _formItems;

        #region publicメソッド
        #region アイテム追加
        public void AddItem(FormItem Item)
        {
            _formItems.Add(Item);
        }
        #endregion
        #endregion
    }

    public record FormItem
    {
        public string Name { get; set; } = string.Empty;
        public object? Value { get; set; }
        public string? StringValue
        {
            get => Value.ToString();
            set => Value = value;
        }
        public bool BoolValue
        {
            get => CommonUtil.ConvertToBoolFromStringValue(Value.ToString());
            set => Value = CommonUtil.ConvertToStringFromBoolValue(value, 1);
        }
        public DateTime? DateTimeValue
        {
            get => !Value.ToString().Equals(string.Empty) ? DateTime.Parse(Value.ToString()) : null;
            set => Value = value.ToString();
        }
    }
}
