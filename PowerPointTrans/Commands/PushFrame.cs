using PointTrans.Utils;
using System;
using System.IO;

namespace PointTrans.Commands
{
    /// <summary>
    /// Pushes a new Frame with `cmds` onto the context stack
    /// </summary>
    /// <seealso cref="PointTrans.IPointCommand" />
    public struct PushFrame : IPointCommand
    {
        /// <summary>
        /// Gets the when.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public When When { get; }
        /// <summary>
        /// Gets the CMDS.
        /// </summary>
        /// <value>
        /// The CMDS.
        /// </value>
        public IPointCommand[] Cmds { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushFrame"/> struct.
        /// </summary>
        /// <param name="cmds">The CMDS.</param>
        /// <exception cref="ArgumentNullException">cmds</exception>
        public PushFrame(params IPointCommand[] cmds)
        {
            When = When.Normal;
            Cmds = cmds ?? throw new ArgumentNullException(nameof(cmds));
        }

        void IPointCommand.Read(BinaryReader r) => Cmds = PointSerDes.DecodeCommands(r);

        void IPointCommand.Write(BinaryWriter w) => PointSerDes.EncodeCommands(w, Cmds);

        void IPointCommand.Execute(IPointContext ctx, ref Action after)
        {
            ctx.Frames.Push(ctx.Frame);
            ctx.ExecuteCmd(Cmds, out after); //action?.Invoke();
        }

        void IPointCommand.Describe(StringWriter w, int pad) { w.WriteLine($"{new string(' ', pad)}PushFrame:"); PointSerDes.DescribeCommands(w, pad, Cmds); }
    }
}