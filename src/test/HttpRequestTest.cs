using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlexWebserver;

namespace Test
{
    [TestClass]
    public class HttpRequestTest
    {
        [TestMethod]
        public void CreateTest()
        {
            var files = new String[]
            {
                "TestHttpRequests/GetEbayByChrome.txt",
                "TestHttpRequests/GetHeiseByChrome.txt",
                "TestHttpRequests/GetEbayByFirefox.txt",
                "TestHttpRequests/GetHeiseByFirefox.txt",
                "TestHttpRequests/GetEbayByIE.txt",
                "TestHttpRequests/GetHeiseByIE.txt",
                "TestHttpRequests/GetWebApplication1ByFirefox.txt"
            };

            HttpRequest request = null;

            foreach (var file in files)
            {
                var content = Util.GetContentFromFile(file);
                request = HttpRequest.CreateByMessageString(content);
            }
        }
    }
}
