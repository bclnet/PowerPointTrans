using System;
using System.IO;

namespace PointTrans.Commands
{
    /// <summary>
    /// Pops a Frame off the context stack
    /// </summary>
    /// <seealso cref="PointTrans.IPointCommand" />
    public struct PopFrame : IPointCommand
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

        void IPointCommand.Execute(IPointContext ctx, ref Action after) => ctx.Frame = ctx.Frames.Pop();

        void IPointCommand.Describe(StringWriter w, int pad) { w.WriteLine($"{new string(' ', pad)}PopFrame"); }
    }
}