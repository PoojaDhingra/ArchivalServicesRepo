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
            string testResult = ws.HelloWorld1();
            Assert.AreEqual("result confirmed", testResult);
        }

        [TestMethod]
        public void BreakRoleInheritance()
        {
            WebService2 ws = new WebService2();
            ws.BreakRoleInheritance("Dokumenter", "http://spivpc:9004/sites/DCM/_api/", "asharma", "span@123", "SPIVPC"); ;
        }
    }
}
