using AzdaraServer.Chinook.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzdaraServer.Chinook.Tests
{
    
    [TestClass]
    public class ODataIntegrationTest
    {
        
        JObject _configTest;
        
        public ODataIntegrationTest()
        {
            _configTest = ChecklistHelper.GetJson("__ConfigTest.json");
        }

        private string checkingFile(string nameSegment)
        {
            return (string)_configTest[nameSegment]["checkFilePath"];
        }
        private string checkingUrl(string nameSegment)
        {
            return (string)_configTest[nameSegment]["url"];
        }

        private void StandartAnswerCheck(HttpResponseMessage response)
        {
            // Check that response was successful or throw exception
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task StandartCheckJsonFile(string segmentOfConfig)
        {
            // Arrange
            JObject expected = ChecklistHelper.GetJson(checkingFile(segmentOfConfig));
            // Act
            RouteHelper testHelper = RouteHelper.Create();
            // Send a request asynchronously and continue when complete
            HttpResponseMessage response = await testHelper._client.GetAsync(checkingUrl(segmentOfConfig));
            // Assert
            StandartAnswerCheck(response);
            string result = response.Content.ReadAsStringAsync().Result;
            JObject actual = JObject.Parse(result);
            AssertHelper.JsonEqual(expected, actual);
        }

        private async Task StandartCheckXmlFile(string segmentOfConfig)
        {
            // Arrange
            XElement expected = ChecklistHelper.GetXml(checkingFile(segmentOfConfig));
            // Act
            RouteHelper testHelper = RouteHelper.Create();

            // Send a request asynchronously and continue when complete
            HttpResponseMessage response = await testHelper._client.GetAsync(checkingUrl(segmentOfConfig));

            // Assert
            StandartAnswerCheck(response);
            string result = response.Content.ReadAsStringAsync().Result;
            XElement actual = XElement.Parse(result);
            AssertHelper.XmlEqual(expected, actual);
        }

        [TestMethod]
        public async Task _01_ReadTheServiceRoot()
        {
            await StandartCheckJsonFile("root");
        }

        [TestMethod]
        public async Task _02_ReturnTheServiceMetadata()
        {
            await StandartCheckXmlFile("metadata");
        }

        [TestMethod]
        public async Task _03_ReadEntity()
        {
            await StandartCheckJsonFile("ReadEntity");
        }

        [TestMethod]
        public async Task _04_GetSingleEntityById()
        {
            await StandartCheckJsonFile("GetSingleEntityById");
        }

        // replace with ~/entityset/key/property on ~/entityset(key)?$select=property
        [TestMethod]
        public async Task _05_GetPrimitiveProperty()
        {
            await StandartCheckJsonFile("GetPrimitiveProperty");
        }

        [TestMethod]
        public async Task _06_SelectAllProperties()
        {
            await StandartCheckJsonFile("SelectAllProperties");
        }

        [TestMethod]
        public async Task _07_FullMetadata()
        {
            await StandartCheckJsonFile("FullMetadata");
        }

        [TestMethod]
        public async Task _08_Filter_SelectFields_Count()
        {
            await StandartCheckJsonFile("Filter_SelectFields_Count");
        }

        [TestMethod]
        public async Task _09_SortBy_And_Paging()
        {
            await StandartCheckJsonFile("SortBy_And_Paging");
        }

        [TestMethod]
        public async Task _10_ExpandByForeignKey()
        {
            await StandartCheckJsonFile("ExpandByForeignKey");
        }

        [TestMethod]
        public async Task _11_ExpandReverse()
        {
            await StandartCheckJsonFile("ExpandReverse");
        }
        
    }
}
