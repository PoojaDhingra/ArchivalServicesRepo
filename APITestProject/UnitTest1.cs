using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService;

namespace APITestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            WebService2 ws = new WebService2();
            ws.HelloWorld1();           
        }
     
    }
}
