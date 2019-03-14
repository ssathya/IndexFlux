using IndexFlux.Utils;
using Moq;
using NUnit.Framework;
using System;

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
		public void TestMethod1()
		{
			// Arrange
			var stringToTest = @"Hello World!\r\nThis is a test Line\n";

			var value = Utilities.ConvertToSSML(stringToTest);

			// Act

			// Assert
			Assert.IsTrue(value.Length != 0);
			Assert.IsFalse(value.Contains(Environment.NewLine));			
		}
	}
}
