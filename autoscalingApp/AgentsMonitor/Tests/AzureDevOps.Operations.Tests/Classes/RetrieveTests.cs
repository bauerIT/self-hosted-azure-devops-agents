﻿using AzureDevOps.Operations.Classes;
using AzureDevOps.Operations.Tests.Data;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.IO;

namespace AzureDevOps.Operations.Tests.Classes
{
    public class RetrieveTests
    {
        [TestCase(@"..\..\Data\TestData\GetPoolId\pools-success.json", Description = "Finds pool id by name successfully")]
        public void GetPoolIdTest_Pool_Present(string jsonPath)
        {
            var dataRetriever = CreateRetriever(jsonPath);

            var poolId = dataRetriever.GetPoolId(TestsConstants.TestPoolName);

            Assert.IsNotNull(poolId);
            Assert.AreEqual(poolId.Value, TestsConstants.TestPoolId);
        }

        [TestCase(@"..\..\Data\TestData\GetPoolId\pools-fail.json", Description = "There is no pool with required name")]
        [TestCase(TestsConstants.FileNotExistPointer, Description = "Response was with status 200, but empty")]
        public void GetPoolIdTest_Pool_Not_Present(string jsonPath)
        {
            var dataRetriever = CreateRetriever(jsonPath);

            var poolId = dataRetriever.GetPoolId(TestsConstants.TestPoolName);

            Assert.IsNull(poolId);
        }

        [TestCase(@"..\..\Data\TestData\Agents\allAgents.json", Description = "Check json parsing for agents")]
        public void CheckAgentsRetrieval(string jsonPath)
        {
            var dataRetriever = CreateRetriever(jsonPath);

            var allAgents = dataRetriever.GetAllAccessibleAgents(TestsConstants.TestPoolId);
            var onlineAgents = dataRetriever.GetOnlineAgentsCount(TestsConstants.TestPoolId);

            Assert.IsNotNull(allAgents);
            Assert.AreEqual(allAgents, TestsConstants.AllAgentsCount);
            Assert.IsNotNull(onlineAgents);
            Assert.AreEqual(onlineAgents.Value, TestsConstants.OnlineAgentsCount);
        }

        [TestCase(@"..\..\Data\TestData\JobRequests\jobs-0-running.json", 0, Description = "There is 0 jobs running according to test JSON")]
        [TestCase(TestsConstants.Json1JobIsRunning, 1, Description = "There is 1 job running according to test JSON")]
        [TestCase(TestsConstants.FileNotExistPointer, 0, Description = "Response was with status 200, but empty")]
        public void CheckJobsRetrieval(string jsonPath, int runningJobs)
        {
            var dataRetriever = CreateRetriever(jsonPath);

            var jobsRunning = dataRetriever.GetCurrentJobsRunningCount(TestsConstants.TestPoolId);

            Assert.AreEqual(jobsRunning, runningJobs);
        }

        internal static Retrieve CreateRetriever(string jsonPathResponse)
        {
            var mockHttp = new MockHttpMessageHandler();

            var jsonPathCombined = Path.Combine(System.AppContext.BaseDirectory, jsonPathResponse);

            var response = File.Exists(jsonPathCombined) ? File.ReadAllText(jsonPathCombined) : string.Empty;

            mockHttp.When("*").Respond("application/json", response);
            var client = mockHttp.ToHttpClient();
            return new Retrieve(TestsConstants.TestOrganizationName, TestsConstants.TestToken,
                client);
        }
    }
}