using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;

namespace Customers
{
    /// <summary>
    /// A custom sync handler for dealing with conflicts between local SQLite tables and remote Azure SQL tables.
    /// The main purpose in this case is gracefully deal with the fact that the remote demo service does not support INSERT, UPDATE, and DELETE operations. 
    /// The net effect is that the app will appear to be able to insert/delete records, when in reality the remote data will not be modified.
    /// If you intend to use this app as a starting point for an app of your own, make sure to modify the behavior here (or not use a custom sync handler at all)
    /// and be sure to implement the proper INSERT, UPDATE, and DELETE operations in your remote service.
    /// </summary>
    public class CustomSyncHandler : IMobileServiceSyncHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Customers.CustomSyncHandler"/> class.
        /// A custom sync handler for dealing with conflicts between local SQLite tables and remote Azure SQL tables.
        /// The main purpose in this case is to gracefully deal with the fact that the remote demo service does not INSERT, UPDATE, and DELETE operations. 
        /// </summary>
        public CustomSyncHandler () { }

        #region IMobileServiceSyncHandler implementation

        public Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            return Task.FromResult(0);
        }

        public async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            JObject result = null;

            try
            {
                result = await operation.ExecuteAsync();
            }
            catch (Exception ex)
            {
                var mobileServiceInvalidOperationException = ex as MobileServiceInvalidOperationException;

                if (mobileServiceInvalidOperationException != null)
                {
                    if (operation.Kind == MobileServiceTableOperationKind.Delete && mobileServiceInvalidOperationException.Response.StatusCode == HttpStatusCode.MethodNotAllowed)
                    {
                        // This condition is occurring because the remote service is not configured to process DELETE operations, because this is a demo and we don't want to actually delete anything from the remote table.

                        // By ducking this exception and returning the result, we'll sync the local table to the remote table's state.
                    }

                    if (operation.Kind == MobileServiceTableOperationKind.Insert && mobileServiceInvalidOperationException.Response.StatusCode == HttpStatusCode.MethodNotAllowed)
                    {
                        // This condition is occurring because the remote service is not configured to process INSERT operations, because this is a demo and we don't want to actually insert anything to the remote table.

                        // By ducking this exception and returning the result, we'll sync the local table to the remote table's state.
                    }

                    if (operation.Kind == MobileServiceTableOperationKind.Update && mobileServiceInvalidOperationException.Response.StatusCode == HttpStatusCode.MethodNotAllowed)
                    {
                        // This condition is occurring because the remote service is not configured to process UPDATE operations, because this is a demo and we don't want to actually update anything in the remote table.

                        // By ducking this exception and returning the result, we'll sync the local table to the remote table's state.
                    }
                }
            }

            return result;
        }

        #endregion
    }
}

