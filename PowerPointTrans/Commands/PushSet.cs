using PointTrans.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace PointTrans.Commands
{
    /// <summary>
    /// Pushes a new Set with `group` and `cmds` onto the context stack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="PointTrans.IPointCommand" />
    /// <seealso cref="PointTrans.IPointSet" />
    public class PushSet<T> : IPointCommand, IPointSet
    {
        /// <summary>
        /// Gets the when.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public When When { get; }
        /// <summary>
        /// Gets the take y.
        /// </summary>
        /// <value>
        /// The take y.
        /// </value>
        public int TakeY { get; private set; }
        /// <summary>
        /// Gets the skip x.
        /// </summary>
        /// <value>
        /// The skip x.
        /// </value>
        public int SkipX { get; private set; }
        /// <summary>
        /// Gets the skip y.
        /// </summary>
        /// <value>
        /// The skip y.
        /// </value>
        public int SkipY { get; private set; }
        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public Func<IPointContext, IEnumerable<Collection<string>>, IEnumerable<IGrouping<T, Collection<string>>>> Group { get; private set; }
        /// <summary>
        /// Gets the CMDS.
        /// </summary>
        /// <value>
        /// The CMDS.
        /// </value>
        public Func<IPointContext, object, IPointCommand[]> Cmds { get; private set; }
        List<Collection<string>> _set;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushSet{T}"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="takeY">The take y.</param>
        /// <param name="skipX">The skip x.</param>
        /// <param name="skipY">The skip y.</param>
        /// <param name="cmds">The CMDS.</param>
        /// <exception cref="ArgumentNullException">cmds</exception>
        public PushSet(Func<IPointContext, IEnumerable<Collection<string>>, IEnumerable<IGrouping<T, Collection<string>>>> group, int takeY = 1, int skipX = 0, int skipY = 0, Func<IPointContext, IGrouping<T, Collection<string>>, IPointCommand[]> cmds = null)
        {
            if (cmds == null)
                throw new ArgumentNullException(nameof(cmds));

            When = When.Normal;
            TakeY = takeY;
            SkipX = skipX;
            SkipY = skipY;
            Group = group;
            Cmds = (z, x) => cmds(z, (IGrouping<T, Collection<string>>)x);
        }

        void IPointCommand.Read(BinaryReader r)
        {
            TakeY = r.ReadInt32();
            SkipX = r.ReadInt32();
            SkipY = r.ReadInt32();
            Group = PointSerDes.DecodeFunc<IPointContext, IEnumerable<Collection<string>>, IEnumerable<IGrouping<T, Collection<string>>>>(r);
            Cmds = PointSerDes.DecodeFunc<IPointContext, object, IPointCommand[]>(r);
            _set = new List<Collection<string>>();
        }

        void IPointCommand.Write(BinaryWriter w)
        {
            w.Write(TakeY);
            w.Write(SkipX);
            w.Write(SkipY);
            PointSerDes.EncodeFunc(w, Group);
            PointSerDes.EncodeFunc(w, Cmds);
        }

        void IPointCommand.Execute(IPointContext ctx, ref Action after) => ctx.Sets.Push(this);

        void IPointCommand.Describe(StringWriter w, int pad)
        {
            w.WriteLine($"{new string(' ', pad)}PushSet{(TakeY <= 1 ? null : $"[{TakeY}]")}: {(Group != null ? "[group func]" : null)}");
            if (Group != null)
            {
                var fakeCtx = new PointContext();
                var fakeSet = new[] { new Collection<string> { "Fake" } };
                var fakeObj = fakeSet.GroupBy(y => y[0]).FirstOrDefault();
                var cmds = Cmds(fakeCtx, fakeObj);
                PointSerDes.DescribeCommands(w, pad, cmds);
            }
        }

        void IPointSet.Add(Collection<string> s) => _set.Add(s);

        void IPointSet.Execute(IPointContext ctx)
        {
            //ctx.WriteRowFirstSet(null);
            //var takeY = _set.Take(TakeY).ToArray();
            //if (Group != null)
            //    foreach (var g in Group(ctx, _set.Skip(TakeY + SkipY)))
            //    {
            //        ctx.WriteRowFirst(null);
            //        var frame = ctx.ExecuteCmd(Cmds(ctx, g), out var action);
            //        ctx.CsvY = 0;
            //        foreach (var v in takeY)
            //        {
            //            ctx.CsvY--;
            //            ctx.WriteRow(v, SkipX);
            //        }
            //        ctx.CsvY = 0;
            //        foreach (var v in g)
            //        {
            //            ctx.AdvanceRow();
            //            ctx.WriteRow(v, SkipX);
            //        }
            //        action?.Invoke();
            //        ctx.WriteRowLast(null);
            //        ctx.Frame = frame;
            //    }
            //ctx.WriteRowLastSet(null);
        }
    }
}