using System;

namespace PointTrans.Services
{
    /// <summary>
    /// CsvWriterField
    /// </summary>
    public class CsvWriterField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriterField"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CsvWriterField(string name) => Name = name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is ignore.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore; otherwise, <c>false</c>.
        /// </value>
        public bool Ignore { get; set; }
        /// <summary>
        /// Gets or sets the custom field formatter.
        /// </summary>
        /// <value>
        /// The custom field formatter.
        /// </value>
        public Func<CsvWriterField, object, object, string> CustomFieldFormatter { get; set; }
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public string DefaultValue { get; set; }
        /// <summary>
        /// Gets or sets the args.
        /// </summary>
        /// <value>
        /// The args.
        /// </value>
        public dynamic Args { get; set; }
    }
}