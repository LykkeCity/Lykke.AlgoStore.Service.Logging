// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.Logging.Client.AutorestClient
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for AlgoStoreLoggingAPI.
    /// </summary>
    public static partial class AlgoStoreLoggingAPIExtensions
    {
            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IsAliveResponse IsAlive(this IAlgoStoreLoggingAPI operations)
            {
                return operations.IsAliveAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Checks service is alive
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IsAliveResponse> IsAliveAsync(this IAlgoStoreLoggingAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.IsAliveWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='userLog'>
            /// </param>
            public static void WriteLog(this IAlgoStoreLoggingAPI operations, UserLogRequest userLog = default(UserLogRequest))
            {
                operations.WriteLogAsync(userLog).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='userLog'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task WriteLogAsync(this IAlgoStoreLoggingAPI operations, UserLogRequest userLog = default(UserLogRequest), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.WriteLogWithHttpMessagesAsync(userLog, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='instanceId'>
            /// </param>
            /// <param name='message'>
            /// </param>
            public static void WriteMessage(this IAlgoStoreLoggingAPI operations, string instanceId = default(string), string message = default(string))
            {
                operations.WriteMessageAsync(instanceId, message).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='instanceId'>
            /// </param>
            /// <param name='message'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task WriteMessageAsync(this IAlgoStoreLoggingAPI operations, string instanceId = default(string), string message = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.WriteMessageWithHttpMessagesAsync(instanceId, message, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

    }
}
