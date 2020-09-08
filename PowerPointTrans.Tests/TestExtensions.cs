using System;

namespace PointTrans
{
    public static class TestExtensions
    {
        public static string ToLocalString(this string value) => value.Replace(@"
", Environment.NewLine);
    }
}
