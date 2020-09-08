﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace PointTrans.Services
{
    /// <summary>
    /// Processes the input CSV
    /// </summary>
    public static class CsvReader
    {
        static Collection<string> ParseLineIntoEntries(string value, char delimiter, Func<string> readLine)
        {
            var list = new Collection<string>();
            var lineArray = value.ToCharArray();
            var inQuote = false;
            var b = new StringBuilder();
            for (var i = 0; i < value.Length; i++)
            {
                if (!inQuote && b.Length == 0)
                {
                    if (char.IsWhiteSpace(lineArray[i])) continue;
                    if (lineArray[i] == '"') { inQuote = true; continue; }
                }
                if (inQuote && lineArray[i] == '"')
                {
                    if (i + 1 < value.Length && lineArray[i + 1] == '"') i++; // double quote error
                    else { if (i + 1 < value.Length && lineArray[i + 1] != delimiter) return null; inQuote = false; continue; } // broken quote error
                }
                if (inQuote || lineArray[i] != delimiter)
                {
                    b.Append(lineArray[i]);
                    if (inQuote && i + 1 == value.Length) { b.Append("\r\n"); i = -1; value = readLine(); lineArray = value.ToCharArray(); } // line spill
                }
                else { list.Add(b.ToString().Trim()); b.Length = 0; }
            }
            list.Add(b.ToString().Trim());
            return list;
        }

        /// <summary>
        /// Executes the specified reader.
        /// </summary>
        /// <param name="reader">The reader instance.</param>
        /// <param name="action">The logic to execute.</param>
        /// <exception cref="System.ArgumentNullException">If the reader instance is null</exception>
        public static IEnumerable<T> Read<T>(Stream stream, Func<Collection<string>, T> action, int startRow = 0, CsvReaderSettings settings = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var delimiter = settings?.Delimiter[0] ?? ',';
            var reader = new StreamReader(stream);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (startRow > 0) { startRow--; continue; }
                var entries = !string.IsNullOrWhiteSpace(line) ? ParseLineIntoEntries(line, delimiter, () => reader.ReadLine()) : null;
                yield return action(entries);
            }
        }
    }
}