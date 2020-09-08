﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using PointTrans.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("PointTrans.Tests")]

namespace PointTrans
{
    /// <summary>
    /// IExcelContext
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IPointContext : IDisposable
    {
        /// <summary>
        /// Gets or sets where the cursor X starts per row.
        /// </summary>
        /// <value>
        /// The x start.
        /// </value>
        int XStart { get; set; }
        /// <summary>
        /// Gets or sets the cursor X coordinate.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        int X { get; set; }
        /// <summary>
        /// Gets or sets the cursor Y coordinate.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        int Y { get; set; }
        /// <summary>
        /// Gets or sets the amount the cursor X advances.
        /// </summary>
        /// <value>
        /// The delta x.
        /// </value>
        int DeltaX { get; set; }
        /// <summary>
        /// Gets or sets the amount the cursor Y advances.
        /// </summary>
        /// <value>
        /// The delta y.
        /// </value>
        int DeltaY { get; set; }
        /// <summary>
        /// Gets or sets the cursor CsvX coordinate, advances with X.
        /// </summary>
        /// <value>
        /// The CSV x.
        /// </value>
        int CsvX { get; set; }
        /// <summary>
        /// Gets or sets the cursor CsvY coordinate, advances with Y.
        /// </summary>
        /// <value>
        /// The CSV y.
        /// </value>
        int CsvY { get; set; }
        /// <summary>
        /// Gets or sets the next direction.
        /// </summary>
        /// <value>
        /// The next direction.
        /// </value>
        //NextDirection NextDirection { get; set; }
        /// <summary>
        /// Gets the stack of commands per row.
        /// </summary>
        /// <value>
        /// The command rows.
        /// </value>
        Stack<CommandRow> CmdRows { get; }
        /// <summary>
        /// Gets the stack of commands per column.
        /// </summary>
        /// <value>
        /// The command cols.
        /// </value>
        Stack<CommandCol> CmdCols { get; }
        /// <summary>
        /// Gets the stack of sets.
        /// </summary>
        /// <value>
        /// The sets.
        /// </value>
        Stack<IPointSet> Sets { get; }
        /// <summary>
        /// Gets the stack of frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        Stack<object> Frames { get; }
        /// <summary>
        /// Gets the current frame.
        /// </summary>
        /// <value>
        /// The frame.
        /// </value>
        object Frame { get; set; }
        /// <summary>
        /// Flushes all pending commands.
        /// </summary>
        void Flush();
        /// <summary>
        /// Gets the specified range.
        /// </summary>
        /// <param name="cells">The cells.</param>
        /// <returns></returns>
        //ExcelRangeBase Get(string cells);
        /// <summary>
        /// Advances the cursor based on NextDirection.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="nextDirection">The next direction.</param>
        /// <returns></returns>
        //ExcelRangeBase Next(ExcelRangeBase range, NextDirection? nextDirection = null);
        /// <summary>
        /// Advances the cursor to the next row.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        //ExcelColumn Next(ExcelColumn column);
        /// <summary>
        /// Advances the cursor to the next row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        //ExcelRow Next(ExcelRow row);
    }
    
    // https://docs.microsoft.com/en-us/office/open-xml/how-to-create-a-presentation-document-by-providing-a-file-name
    internal class PointContext : IPointContext
    {
        public PointContext()
        {
            Stream = new MemoryStream();
            Doc = PresentationDocument.Create(Stream, PresentationDocumentType.Presentation, true);
            P = Doc.AddPresentationPart();
            P.Presentation = new Presentation();
        }
        public void Dispose() => Doc.Dispose();

        public int XStart { get; set; } = 1;
        public int X { get; set; } = 1;
        public int Y { get; set; } = 1;
        public int DeltaX { get; set; } = 1;
        public int DeltaY { get; set; } = 1;
        public int CsvX { get; set; } = 1;
        public int CsvY { get; set; } = 1;
        //public NextDirection NextDirection { get; set; } = NextDirection.Column;
        public Stack<CommandRow> CmdRows { get; } = new Stack<CommandRow>();
        public Stack<CommandCol> CmdCols { get; } = new Stack<CommandCol>();
        public Stack<IPointSet> Sets { get; } = new Stack<IPointSet>();
        public Stack<object> Frames { get; } = new Stack<object>();
        public Stream Stream;
        public PresentationDocument Doc;
        public PresentationPart P;

        //public ExcelWorksheet EnsureWorksheet() => WS ?? (WS = WB.Worksheets.Add($"Sheet {WB.Worksheets.Count + 1}"));

        //public ExcelRangeBase Get(string cells) => WS.Cells[this.DecodeAddress(cells)];

        //public ExcelRangeBase Next(ExcelRangeBase range, NextDirection? nextDirection = null) => (nextDirection ?? NextDirection) == NextDirection.Column ? range.Offset(0, DeltaX) : range.Offset(DeltaY, 0);
        //public ExcelColumn Next(ExcelColumn col) => throw new NotImplementedException();
        //public ExcelRow Next(ExcelRow row) => throw new NotImplementedException();

        public void Flush()
        {
            //if (Sets.Count == 0) this.WriteRowLast(null);
            Frames.Clear();
            CommandRow.Flush(this, 0);
            CommandCol.Flush(this, 0);
            PopSet.Flush(this, 0);
        }

        public object Frame
        {
            get => (CmdRows.Count, CmdCols.Count, Sets.Count);
            set
            {
                var (rows, cols, sets) = ((int rows, int cols, int sets))value;
                CommandRow.Flush(this, rows);
                CommandCol.Flush(this, cols);
                PopSet.Flush(this, sets);
            }
        }
    }
}