using System;
using System.IO;

namespace PointTrans.Commands
{
    /// <summary>
    /// Opens a Presentation at `.Path`
    /// </summary>
    /// <seealso cref="PointTrans.IPointCommand" />
    public struct PresentationOpen : IPointCommand
    {
        public When When { get; }
        public string Path { get; private set; }

        public PresentationOpen(string path, string password = null)
        {
            When = When.Normal;
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        void IPointCommand.Read(BinaryReader r)
        {
            Path = r.ReadString();
        }

        void IPointCommand.Write(BinaryWriter w)
        {
            w.Write(Path);
        }

        void IPointCommand.Execute(IPointContext ctx, ref Action after) => ctx.PresentationOpen(Path);

        void IPointCommand.Describe(StringWriter w, int pad) { w.WriteLine($"{new string(' ', pad)}PresentationOpen: {Path}"); }
    }
}