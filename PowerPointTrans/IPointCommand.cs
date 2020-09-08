using PointTrans.Commands;
using System;
using System.IO;

namespace PointTrans
{
    /// <summary>
    /// IPointCommand
    /// </summary>
    public interface IPointCommand
    {
        /// <summary>
        /// Gets the when.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        When When { get; }
        /// <summary>
        /// Reads the specified r.
        /// </summary>
        /// <param name="r">The r.</param>
        void Read(BinaryReader r);
        /// <summary>
        /// Writes the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        void Write(BinaryWriter w);
        /// <summary>
        /// Executes the specified CTX.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="after">The after.</param>
        void Execute(IPointContext ctx, ref Action after);
        /// <summary>
        /// Describes the specified w.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="pad">The pad.</param>
        void Describe(StringWriter w, int pad);
    }
}
