using PointTrans.Utils;
using System;
using System.IO;

namespace PointTrans.Commands
{
    /// <summary>
    /// Executes `.Action()`
    /// </summary>
    /// <seealso cref="PointTrans.IPointCommand" />
    public class Command : IPointCommand
    {
        /// <summary>
        /// Gets the when.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public When When { get; private set; }
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public Action<IPointContext> Action { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public Command(Action action)
            : this(When.Normal, v => action()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public Command(Action<IPointContext> action)
            : this(When.Normal, action) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="when">The when.</param>
        /// <param name="action">The action.</param>
        public Command(When when, Action action)
            : this(when, v => action()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="when">The when.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">action</exception>
        public Command(When when, Action<IPointContext> action)
        {
            When = when;
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        void IPointCommand.Read(BinaryReader r)
        {
            When = (When)r.ReadByte();
            Action = PointSerDes.DecodeAction<IPointContext>(r);
        }

        void IPointCommand.Write(BinaryWriter w)
        {
            w.Write((byte)When);
            PointSerDes.EncodeAction(w, Action);
        }

        void IPointCommand.Execute(IPointContext ctx, ref Action after) => Action(ctx);

        void IPointCommand.Describe(StringWriter w, int pad) { w.WriteLine($"{new string(' ', pad)}Command{(When == When.Normal ? null : $"[{When}]")}: [action]"); }
    }
}