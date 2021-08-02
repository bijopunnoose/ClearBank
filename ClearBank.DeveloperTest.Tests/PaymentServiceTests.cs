using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceTests
    {
        [Fact]
        public void GetAccountBasedOnDataStoreTypeBackupTest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var accountBackup = ps.GetAccountBasedOnDataStoreType(makePaymentRequest, "Backup");

            //assert
            accountBackup.Should().NotBeNull();
        }

        [Fact]
        public void GetAccountBasedOnDataStoreTypeNonBackupTest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var accountNonBackup = ps.GetAccountBasedOnDataStoreType(makePaymentRequest, "NonBackup");

            //assert
            accountNonBackup.Should().NotBeNull();
        }

        [Fact]
        public void GetMakePaymentResultAccountNullTest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = null;

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(false);
        }

        [Fact]
        public void GetMakePaymentResultBacs()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(true);
        }

        [Fact]
        public void GetMakePaymentResultBacsAccountNonBacs()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(false);
        }

        [Fact]
        public void GetMakePaymentResultFasterPaymentsAccountFasterPaymentsAndAccountBalancegreaterThanRequest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 100
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(true);
        }

        [Fact]
        public void GetMakePaymentResultFasterPaymentsAccountFasterPaymentsAndAccountBalanceLessThanRequest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 100
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 10
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(false);
        }
        [Fact]
        public void GetMakePaymentResultFasterPaymentsAccountNonFasterPaymentsAndAccountBalanceGreaterThanRequest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 100
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(false);
        }

        [Fact]
        public void GetMakePaymentResultChapsAccountChapsAndAccountLive()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(true);
        }

        [Fact]
        public void GetMakePaymentResultChapsAccountNonChapsAndAccountLive()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(false);
        }

        [Fact]
        public void GetMakePaymentResultChapsAccountChapsAndAccountNonLive()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.GetMakePaymentResult(makePaymentRequest, account);

            //assert
            result.Success.Should().Be(false);
        }

        [Fact]
        public void UpdateAccountMakePaymentResultFailure()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10                
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };
            var makePaymentResult = new MakePaymentResult()
            {
                Success = false
            };

            //act
            var ps = new PaymentService(configuration.Object);
            Action act = () => ps.UpdateAccount(makePaymentRequest, "Backup", account, null);

            //assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Make Payment not successful. MakePaymentResult did not return success");
        }

        [Fact]
        public void UpdateAccountMakePaymentResultSuccess()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            //Setuppayment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };
            var makePaymentResult = new MakePaymentResult()
            {
                Success = true
            };

            //act
            var ps = new PaymentService(configuration.Object);
            Action act = () => ps.UpdateAccount(makePaymentRequest, "Backup", account, makePaymentResult);

            //assert
            act.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void MakePaymentRequest()
        {
            //arrange
            //Mock Iconfiguration
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(a => a.Value).Returns("Backup");
            configuration.Setup(a => a.GetSection("DataStoreType")).Returns(configurationSection.Object);
            //Setup payment service
            var makePaymentRequest = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 10
            };

            /*
            //We could mock GetAccountBasedOnDataStoreTypecif GetAccount can be injected into MakePayment method (ie Dependency Inversion Principle)
            Account account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };
            var mPS = new Mock<IPaymentService>();
            mPS.Setup(x => x.GetAccountBasedOnDataStoreType(makePaymentRequest,"Backup")).Returns(account);
            */

            //act
            var ps = new PaymentService(configuration.Object);
            var result = ps.MakePayment(makePaymentRequest);

            //assert
            result.Success.Should().Be(false);
        }
    }
}
