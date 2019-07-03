using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Silverlight.Common.Reflection
{
    /// <summary>
    /// 可绑定的基类
    /// </summary>
    public class BindableObject : INotifyPropertyChanged
    {
        DataCollection _dataCollection;

        string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        public BindableObject() { }

        public BindableObject(DataCollection collection) {
            _dataCollection = collection;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取值通过健值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if (_dataCollection != null)
            {
                var item = _dataCollection.Get(key);
                return item.Value;
            }
            return null;
        }

        /// <summary>
        /// 返回数字类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public decimal GetDecimalValue(string key)
        {
            var v = GetValue(key);
            decimal value = 0;
            decimal.TryParse(v, out value);
            return value;
        }

        /// <summary>
        /// 返回数字类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DateTime GetDateTimeValue(string key)
        {
            var v = GetValue(key);
            DateTime value;
            DateTime.TryParse(v, out value);
            return value;
        }

        /// <summary>
        /// 设置项的值
        /// </summary>
        /// <param name="key">健</param>
        /// <param name="value">值</param>
        public void SetValue(string key, string value)
        {
            var item = _dataCollection.Get(key);
            if (item != null)
            {
                item.Value = value;
            }
        }

        public void SetDecimalValue(string columnName, decimal value)
        {
            SetValue(columnName, value.ToString());
        }

        public void SetDateTimeValue(string columnName, DateTime value)
        {
            SetValue(columnName, string.IsNullOrWhiteSpace(_dateFormat) ? value.ToString() : value.ToString(_dateFormat));
        }

        /// <summary>
        /// PropertyChanged event
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Event arguments</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, e);
            }
        }
    }

    /// <summary>
    /// 数据集合
    /// </summary>
    public class DataCollection : System.Collections.Generic.IDictionary<string,string>
    {
        /// <summary>
        /// 当前数据所有项
        /// </summary>
        ObservableCollection<DataItem> _items = new ObservableCollection<DataItem>();

        public ObservableCollection<DataItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public void Add(DataItem item)
        {
            Items.Add(item);
        }

        public void Add(string key, string value)
        {
            var item = new DataItem() { Key = key, Value = value };
            Items.Add(item);
        }

        public bool ContainsKey(string key)
        {
            foreach (var item in _items)
            {
                if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public System.Collections.Generic.ICollection<string> Keys
        {
            get {
                var keys = (from p in _items
                            select p.Key).ToList<string>();
                return keys;
            }
        }

        public DataItem Get(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                foreach (var item in _items)
                {
                    if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return item;
                    }
                
                }
            }
            return null;
        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var item = Get(key);
                return _items.Remove(item);
            }
            return false;
        }

        public bool TryGetValue(string key, out string value)
        {
            var item = Get(key);
            value = string.Empty;
            if (item != null)
            {
                value = item.Value;
                return true;
            }
            return false;
        }

        public System.Collections.Generic.ICollection<string> Values
        {
            get
            {
                var vs = (from p in _items
                            select p.Value).ToList<string>();
                return vs;
            }
        }

        public string this[string key]
        {
            get
            {
                var item = Get(key);
                if (item != null) return item.Value;
                return null;
            }
            set
            {
                var item = Get(key);
                if (item != null) item.Value = value;
            }
        }

        public void Add(System.Collections.Generic.KeyValuePair<string, string> item)
        {
            var newitem = new DataItem() { Key = item.Key, Value = item.Value };
            _items.Add(newitem);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(System.Collections.Generic.KeyValuePair<string, string> item)
        {
            var obj = Get(item.Key);
            if (obj != null && obj.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        public void CopyTo(System.Collections.Generic.KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(System.Collections.Generic.KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, string>> GetEnumerator()
        {
            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    /// <summary>
    /// 数据项
    /// </summary>    
    public class DataItem
    {
        /// <summary>
        /// 健值
        /// </summary>        
        public string Key { get; set; }

        /// <summary>
        /// 当前项的值
        /// </summary>        
        public string Value { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>       
        public Type DataType { get; set; }

        public override string ToString()
        {
            return Key + Value + DataType==null?"":DataType.FullName;
        }
    }
}
