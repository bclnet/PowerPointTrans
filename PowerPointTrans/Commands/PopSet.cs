using System;
using System.IO;

namespace PointTrans.Commands
{
    /// <summary>
    /// Pops a Set off the context stack
    /// </summary>
    /// <seealso cref="PointTrans.IPointCommand" />
    public struct PopSet : IPointCommand
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

        void IPointCommand.Execute(IPointContext ctx, ref Action after) => ctx.Sets.Pop().Execute(ctx);

        void IPointCommand.Describe(StringWriter w, int pad) { w.WriteLine($"{new string(' ', pad)}PopSet"); }

        internal static void Flush(IPointContext ctx, int index)
        {
            while (ctx.Sets.Count > index)
                ctx.Sets.Pop().Execute(ctx);
        }

    }
}