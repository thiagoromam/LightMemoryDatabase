using System;
using System.Threading.Tasks;

namespace LightMemoryDatabase.Api
{
    public static class TransactionExtensions
    {
        public static async Task ExecuteInTransaction(Action action)
        {
            var context = DependencyRegistry.Resolve<IContextService>();
            IContextTransaction transaction = null;
            try
            {
                transaction = await context.OpenTransaction();
                action();
            }
            catch
            {
                if (transaction != null) 
                    transaction.Rollback().Wait();

                throw;
            }
        }
    }
}
