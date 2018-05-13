using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzdaraServer.Chinook.Tests.Helpers
{
    public class AssertHelper
    {
        public static void JsonEqual(JToken expected, JToken actual)
        {
            if (!JToken.DeepEquals(expected, actual))
            {
                string failures = string.Format(" Expected:<{0}> Actual:<{1}>", expected, actual);
                Assert.Fail("AssertHelper.JsonEqual failed. " +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, failures)
                );
            }
        }

        public static void XmlEqual(XNode expected, XNode actual)
        {
            if(!XNode.DeepEquals(expected,actual))
            {
                string failures = string.Format(" Expected:<{0}> Actual:<{1}>", expected, actual);
                Assert.Fail("AssertHelper.XmlEqual failed. " +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, failures)
                );
            }
        }
    }
}
