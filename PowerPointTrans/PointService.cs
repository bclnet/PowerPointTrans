using PointTrans.Commands;
using PointTrans.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PointTrans.Services;


// https://docs.microsoft.com/en-us/office/open-xml/how-to-get-all-the-text-in-all-slides-in-a-presentation
// https://docs.microsoft.com/en-us/office/open-xml/working-with-presentation-slides
// https://stackoverflow.com/questions/26372020/how-to-programmatically-create-a-powerpoint-from-a-list-of-images
// https://www.free-power-point-templates.com/articles/create-powerpoint-ppt-programmatically-using-c/

namespace PointTrans
{
    /// <summary>
    /// PointService
    /// </summary>
    public static class PointService
    {
        /// <summary>
        /// The comment
        /// </summary>
        public static readonly string Comment = "^q|";
        /// <summary>
        /// The stream
        /// </summary>
        public static readonly string Stream = "^q=";
        /// <summary>
        /// The break
        /// </summary>
        public static readonly string Break = "^q!";
        /// <summary>
        /// Gets the pop frame.
        /// </summary>
        /// <value>
        /// The pop frame.
        /// </value>
        public static string PopFrame => $"{Stream}{PointSerDes.Encode(new PopFrame())}";
        /// <summary>
        /// Gets the pop set.
        /// </summary>
        /// <value>
        /// The pop set.
        /// </value>
        public static string PopSet => $"{Stream}{PointSerDes.Encode(new PopSet())}";
        /// <summary>
        /// Encodes the specified describe.
        /// </summary>
        /// <param name="describe">if set to <c>true</c> [describe].</param>
        /// <param name="cmds">The CMDS.</param>
        /// <returns></returns>
        public static string Encode(bool describe, params IPointCommand[] cmds) => $"{(describe ? PointSerDes.Describe(Comment, cmds) : null)}{Stream}{PointSerDes.Encode(cmds)}";
        /// <summary>
        /// Encodes the specified describe.
        /// </summary>
        /// <param name="describe">if set to <c>true</c> [describe].</param>
        /// <param name="cmds">The CMDS.</param>
        /// <returns></returns>
        public static string Encode(bool describe, IEnumerable<IPointCommand> cmds) => Encode(describe, cmds.ToArray());
        /// <summary>
        /// Encodes the specified CMDS.
        /// </summary>
        /// <param name="cmds">The CMDS.</param>
        /// <returns></returns>
        public static string Encode(params IPointCommand[] cmds) => $"{Stream}{PointSerDes.Encode(cmds)}";
        /// <summary>
        /// Encodes the specified CMDS.
        /// </summary>
        /// <param name="cmds">The CMDS.</param>
        /// <returns></returns>
        public static string Encode(IEnumerable<IPointCommand> cmds) => Encode(cmds.ToArray());
        /// <summary>
        /// Decodes the specified packed.
        /// </summary>
        /// <param name="packed">The packed.</param>
        /// <returns></returns>
        public static IPointCommand[] Decode(string packed) => PointSerDes.Decode(packed.Substring(Stream.Length));

        /// <summary>
        /// Transforms the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static (Stream stream, string, string path) Transform((Stream stream, string, string path) value)
        {
            using (var s1 = value.stream)
            {
                s1.Seek(0, SeekOrigin.Begin);
                var s2 = new MemoryStream(Build(s1));
                return (s2, "application/vnd.openxmlformats-officedocument.presentationml", value.path.Replace(".csv", ".pptx"));
            }
        }

        static byte[] Build(Stream s)
        {
            using (var ctx = new PointContext())
            {
                CsvReader.Read(s, x =>
                {
                    if (x == null || x.Count == 0 || x[0].StartsWith(Comment)) return true;
                    else if (x[0].StartsWith(Stream))
                    {
                        var r = ctx.ExecuteCmd(Decode(x[0]), out var action) != null;
                        action?.Invoke();
                        return r;
                    }
                    else if (x[0].StartsWith(Break)) return false;
                    //ctx.AdvanceRow();
                    //if (ctx.Sets.Count == 0) ctx.WriteRow(x);
                    //else ctx.Sets.Peek().Add(x);
                    return true;
                }).Any(x => !x);
                ctx.Flush();
                ctx.Doc.Close();
                return ((MemoryStream)ctx.Stream).ToArray();
            }
        }

        internal static object ParseValue(this string v) =>
            v == null ? null :
                v.Contains("/") && DateTime.TryParse(v, out var vd) ? vd :
                v.Contains(".") && double.TryParse(v, out var vf) ? vf :
                long.TryParse(v, out var vl) ? vl :
                (object)v;

        internal static string SerializeValue(this object v, Type type) =>
            type == null ? v.ToString() : type.IsArray
            ? string.Join("|^", ((Array)v).Cast<object>().Select(x => x?.ToString()))
            : v.ToString();

        internal static object DeserializeValue(this string v, Type type) =>
            type == null ? v : type.IsArray && type.GetElementType() is Type elemType
            ? v.Split(new[] { "|^" }, StringSplitOptions.None).Select(x => Convert.ChangeType(x, elemType)).ToArray()
            : Convert.ChangeType(v, type);

        internal static T CastValue<T>(this object v, T defaultValue = default) => v != null
            ? typeof(T).IsArray && typeof(T).GetElementType() is Type elemType
            ? (T)(object)((Array)v).Cast<object>().Select(x => Convert.ChangeType(v, elemType)).ToArray()
            : (T)Convert.ChangeType(v, typeof(T))
            : defaultValue;
    }
}
