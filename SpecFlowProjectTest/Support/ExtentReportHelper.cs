using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;

namespace SpecFlowProjectTest.Support
{
    public static class ExtentReportHelper
    {
        public static ExtentReports Extent;
        public static ExtentTest Test;
        private static string _reportPath;

        static ExtentReportHelper()
        {
            var reportsDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Reports");
            System.IO.Directory.CreateDirectory(reportsDir);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _reportPath = System.IO.Path.Combine(reportsDir, $"ExtentReport_{timestamp}.html");

            var sparkReporter = new ExtentSparkReporter(_reportPath);
            Extent = new ExtentReports();
            Extent.AttachReporter(sparkReporter);
        }

        public static void FlushReport()
        {
            Extent.Flush();
        }

        public static string ReportPath => _reportPath;
    }
}