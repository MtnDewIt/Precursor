namespace Precursor.Tests.Reports
{
    public static class  ReportHelper
    {
        public enum ReportErrorLevel
        {
            None = 0,
            Intermediate,
            All,
        }

        public static ReportErrorLevel ParseErrorLevel(int currentCount, int totalCount)
        {
            if (currentCount == 0)
                return ReportErrorLevel.None;

            if (currentCount > 0 && currentCount < totalCount)
                return ReportErrorLevel.Intermediate;

            if (currentCount == totalCount)
                return ReportErrorLevel.All;

            return ReportErrorLevel.All;
        }
    }
}
