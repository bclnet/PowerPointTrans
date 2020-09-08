using System;
using System.IO;

namespace PointTrans.Commands
{
    /// <summary>
    /// Flushes all pending commands
    /// </summary>
    /// <seealso cref="PointTrans.IPointCommand" />
    public struct Flush : IPointCommand
    {
        /// <summary>
        /// Gets the when.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public When When { get; }

        void IPointCommand.Read(BinaryReader r) { }

        void IPointCommand.Write(BinaryWriter w) { }

        void IPointCommand.Execute(IPointContext ctx, ref Action after) => ctx.Flush();

        void IPointCommand.Describe(StringWriter w, int pad) { w.WriteLine($"{new string(' ', pad)}Flush"); }
    }
}