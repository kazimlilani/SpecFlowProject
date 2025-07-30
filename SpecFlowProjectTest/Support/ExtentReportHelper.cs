using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;

namespace SpecFlowProjectTest.Support
{
    public static class ExtentReportHelper
    {
        public static ExtentReports Extent;
        public static ExtentTest Test;

        static ExtentReportHelper()
        {
            var sparkReporter = new ExtentSparkReporter(@"D:\Automation\csharp\Demonstrating-DHCW-UI-Test-Approach-master\ExtentReport.html");
            Extent = new ExtentReports();
            Extent.AttachReporter(sparkReporter);
        }

        public static void FlushReport()
        {
            Extent.Flush();
        }
    }
}