using IndexFlux.Utils;
using Moq;
using NUnit.Framework;
using System;
using static Google.Cloud.Dialogflow.V2.Intent.Types.Message.Types;

namespace IndexFluxTests.Utils
{
	[TestFixture]
	public class UtilitiesTests
	{
		private MockRepository mockRepository;



		[SetUp]
		public void SetUp()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


		}

		[TearDown]
		public void TearDown()
		{
			this.mockRepository.VerifyAll();
		}

		

		[Test]
		public void ConvertToSsmlTest()
		{
			// Arrange
			var stringToTest = @"Hello World!\r\nThis is a test Line\n";

			var value = Utilities.ConvertToSSML(stringToTest);

			// Act

			// Assert
			Assert.IsTrue(value.Length != 0);
			Assert.IsFalse(value.Contains(Environment.NewLine));			
		}
		[Test]
		public void BuildTextToSpeechTest()
		{
			//var stringToTest = @"Now my question is, whether it is possible to create a repository on two different hosts and then keep them in sync? For example, suppose once per week I update the repository on my NAS to match the one on Bitbucket.\n Then, in case something happens with Bitbucket, I will still have the full repository with full history of development on my local NAS storage.\n";
			var stringToTest = "Hello World!\n How are your?\n";
			SimpleResponse value = Utilities.BuildTextToSpeech(stringToTest);

			//Assert
			Assert.IsTrue(value.Ssml.Contains("<speak>"));
			Assert.IsTrue(value.Ssml.Contains("</speak>"));
			Assert.IsTrue(value.Ssml.Contains("<break"));

		}
	}
}
