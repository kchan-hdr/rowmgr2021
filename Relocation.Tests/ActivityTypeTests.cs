using com.hdr.rowmgr.Relocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relocation.Tests
{
    /// <summary>
    /// Summary description for ActivityTypeTests
    /// </summary>
    [TestClass]
    public class ActivityTypeTests
    {
        public ActivityTypeTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public async Task Simple_Get_All_Types()
        {
            var c = new RelocationContext();
            var r = new RelocationRepository(c);
            IRelocationCaseOps h = new RelocationCaseOps(c, r);

            var types = await h.GetActivityTypes();
            Assert.IsTrue(types.Any());
            Assert.IsTrue(types.All(t => t.IsActive));

            foreach (var t in types.OrderBy(rt => rt.DisplayOrder))
            {
                Trace.WriteLine($"{t.ActivityTypeCode}, {t.Description}");

                Trace.WriteLine(string.Join(";", t.GetTasks().Select(tx => tx.Caption)));
            }
        }
    }
}
