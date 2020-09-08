using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PointTrans.Services
{
    /// <summary>
    /// CsvWriterSettings
    /// </summary>
    public class CsvWriterSettings
    {
        /// <summary>
        /// WriteFilterMode
        /// </summary>
        public enum WriteFilterMode
        {
            /// <summary>
            /// ExceptionsInFields
            /// </summary>
            ExceptionsInFields,
            /// <summary>
            /// InclusionsInFields
            /// </summary>
            InclusionsInFields,
        }

        /// <summary>
        /// WriteOptions
        /// </summary>
        public enum WriteOptions
        {
            /// <summary>
            /// HasHeaderRow
            /// </summary>
            HasHeaderRow = 0x1,
            /// <summary>
            /// IncludeFields
            /// </summary>
            IncludeFields = 0x2,
            /// <summary>
            /// EncodeValues
            /// </summary>
            EncodeValues = 0x4,
        }

        /// <summary>
        /// FieldCollection
        /// </summary>
        public class FieldCollection : KeyedCollection<string, CsvWriterField>
        {
            /// <summary>
            /// Tries the get value.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="field">The field.</param>
            /// <returns></returns>
            public bool TryGetValue(string name, out CsvWriterField field)
            {
                if (!Contains(name))
                {
                    field = null;
                    return false;
                }
                field = base[name];
                return true;
            }

            /// <summary>
            /// When implemented in a derived class, extracts the key from the specified element.
            /// </summary>
            /// <param name="item">The element from which to extract the key.</param>
            /// <returns>
            /// The key for the specified element.
            /// </returns>
            protected override string GetKeyForItem(CsvWriterField item) => item.Name;
        }

        string _delimiter;

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>The delimiter.</value>
        public string Delimiter
        {
            get
            {
                if (_delimiter == null)
                    try
                    {
                        _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                        if (_delimiter.Length != 1)
                            _delimiter = ",";
                    }
                    catch (Exception) { _delimiter = ","; }
                return _delimiter;
            }
            set => _delimiter = value;
        }

        /// <summary>
        /// DefaultContext
        /// </summary>
        public static readonly CsvWriterSettings DefaultSettings = new CsvWriterSettings { };

        /// <summary>
        /// Gets or sets the <see cref="System.Boolean"/> with the specified option.
        /// </summary>
        public bool this[WriteOptions option]
        {
            get => (EmitOptions & option) == option;
            set => EmitOptions = value ? EmitOptions | option : EmitOptions & ~option;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has header row.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has header row; otherwise, <c>false</c>.
        /// </value>
        public bool HasHeaderRow
        {
            get => this[WriteOptions.HasHeaderRow];
            set => this[WriteOptions.HasHeaderRow] = value;
        }

        /// <summary>
        /// Gets or sets the filter mode.
        /// </summary>
        /// <value>
        /// The filter mode.
        /// </value>
        public WriteFilterMode FilterMode { get; set; } = WriteFilterMode.ExceptionsInFields;

        /// <summary>
        /// Gets the fields.
        /// </summary>
        public FieldCollection Fields { get; } = new FieldCollection();

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public Func<Type, IEnumerable<CsvWriterColumn>> GetColumns { get; set; }

        /// <summary>
        /// Gets the type of the item infos by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="includeFields">if set to <c>true</c> [include fields].</param>
        /// <returns></returns>
        public static List<CsvWriterColumn> GetColumnsByType(Type type, bool includeFields = false)
        {
            var items = type.GetProperties()
                .Select(x => new CsvWriterColumn(x.Name, x.GetValue, x.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName))
                .ToList();
            if (includeFields)
                items.AddRange(type.GetFields()
                    .Select(x => new CsvWriterColumn(x.Name, x.GetValue, x.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName)));
            return items;
        }

        /// <summary>
        /// Gets or sets the emit options.
        /// </summary>
        /// <value>
        /// The emit options.
        /// </value>
        public WriteOptions EmitOptions { get; set; } = WriteOptions.IncludeFields | WriteOptions.HasHeaderRow | WriteOptions.EncodeValues;

        /// <summary>
        /// Gets or sets the flush at.
        /// </summary>
        /// <value>
        /// The flush at.
        /// </value>
        public int FlushAt { get; set; } = 500;

        /// <summary>
        /// Gets or sets the on flush.
        /// </summary>
        /// <value>
        /// The on flush.
        /// </value>
        public Func<IEnumerable<object>, IEnumerable<object>> BeforeFlush { get; set; }
    }
}