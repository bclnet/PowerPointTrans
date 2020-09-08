using System.Collections.ObjectModel;

namespace PointTrans
{
    /// <summary>
    /// IPointSet
    /// </summary>
    public interface IPointSet
    {
        /// <summary>
        /// Adds the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        void Add(Collection<string> s);
        /// <summary>
        /// Executes the specified CTX.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        void Execute(IPointContext ctx);
    }
}
