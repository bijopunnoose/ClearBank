using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ClearBank.DeveloperTest.Tests")]
namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        //Added an example of DI.
        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal void UpdateAccount(MakePaymentRequest request, string dataStoreType, Account account, MakePaymentResult result)
        {
            try
            {
                if (result.Success)
                {
                    account.Balance -= request.Amount;

                    if (dataStoreType == "Backup")
                    {
                        var accountDataStore = new BackupAccountDataStore();
                        accountDataStore.UpdateAccount(account);
                    }
                    else
                    {
                        var accountDataStore = new AccountDataStore();
                        accountDataStore.UpdateAccount(account);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Make Payment not successful. MakePaymentResult did not return success");
            }
        }

        internal MakePaymentResult GetMakePaymentResult(MakePaymentRequest request, Account account)
        {
            var result = new MakePaymentResult();
            if (account == null)
            {
                result.Success = false;
                return result;
            }

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                        return result;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                        || account.Balance < request.Amount)
                    {
                        result.Success = false;
                        return result;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) ||
                        account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                        return result;
                    }
                    break;
            }
            return result;
        }

        //This method should be ideally extracted to a separate class and injected into MakePayment method
        //This would help in mocking this method and better unit testing of MakePayment
        //Commented code in the unit test project line 354-363
        public Account GetAccountBasedOnDataStoreType(MakePaymentRequest request, string dataStoreType)
        {
            if (dataStoreType == "Backup")
            {
                var backupAccountDataStore = new BackupAccountDataStore();
                return backupAccountDataStore.GetAccount(request.DebtorAccountNumber);
            }

            var accountDataStore = new AccountDataStore();
            return accountDataStore.GetAccount(request.DebtorAccountNumber);
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType= _configuration.GetValue<string>("DataStoreType");

            var account = GetAccountBasedOnDataStoreType(request, dataStoreType);

            MakePaymentResult result = GetMakePaymentResult(request, account);

            UpdateAccount(request, dataStoreType, account, result);

            return result;
        }
    }
}
