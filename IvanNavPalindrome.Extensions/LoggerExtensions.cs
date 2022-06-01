using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace IvanNavPalindrome.Extensions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Параметры логирования
        /// </summary>
        public class LogDataParams<T>
        {
            /// <summary>
            /// Exception
            /// </summary>
            public Exception? Exception { get; set; }

            /// <summary>
            /// Message
            /// </summary>
            public string? Message { get; set; }

            /// <summary>
            /// Params
            /// </summary>
            public T? Params { get; set; }
        }

        /// <summary>
        /// Logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logDataParams">Параметры</param>
        /// <param name="caller"></param>
        public static void LogData<T>(this ILogger<T> logger, LogLevel level,
            Action<LogDataParams<object>>? logDataParams = null,
            [CallerMemberName] string caller = "")
        {
            logger.LogDataInner(level, logDataParams, caller);
        }

        /// <summary>
        /// Logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logDataParams">Параметры</param>
        /// <param name="caller"></param>
        private static void LogDataInner<T, TAction>(this ILogger<T> logger, LogLevel level,
            Action<LogDataParams<TAction>>? logDataParams = null,
            [CallerMemberName] string caller = "")
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            try
            {
                var paramsData = new LogDataParams<TAction>();
                logDataParams?.Invoke(paramsData);


                var msg = GetLogMessage(typeof(T), paramsData, caller);
                logger.Log(level, paramsData.Exception, msg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Logging error");
                throw new Exception("Logging error", e);
            }
        }

        private static dynamic GetMachineInfo()
        {
            return new
            {
                Environment.MachineName
            };
        }

        private static dynamic? GetActivityInfo()
        {
            var activity = Activity.Current;
            if (activity == null) return null;

            var activityInfo = new
            {
                activity.Id,
                TraceId = activity.TraceId.ToHexString()
            };

            return activityInfo;
        }

        private static dynamic? GetExceptionInfo(Exception? e)
        {
            if (e == null) return null;

            return new
            {
                e.Message,
                Class = e.GetType().FullName,
                e.StackTrace,
            };
        }

        private static string GetLogMessage<TData>(Type type, LogDataParams<TData> paramsData, string caller)
        {
            var logParams = new
            {
                Activity = GetActivityInfo(),
                Machine = GetMachineInfo(),

                paramsData.Message,

                Logger = new
                {
                    type.Name,
                    type.Namespace
                },
                Caller = caller,

                paramsData.Params,

                Exception = GetExceptionInfo(paramsData.Exception),
                ExceptionInner = GetExceptionInfo(paramsData.Exception?.InnerException)
            };

            var msg = JsonSerializer.Serialize(logParams, GetJsonSerializerOptions());

            return msg;
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var serializerOptions = new JsonSerializerOptions
            {
                // Camel case
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // 
                PropertyNameCaseInsensitive = true,
                // Unicode
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            serializerOptions.Converters.Add(new JsonStringEnumConverter());

            return serializerOptions;
        }
    }
}
