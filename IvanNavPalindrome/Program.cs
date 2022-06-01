using IvanNavPalindrome.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IvanNavPalindrome;

public class Program
{
    private static ILogger<Program>? _logger;

    public static void Main()
    {
        var serviceCollection = new ServiceCollection();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        serviceCollection.AddSingleton(loggerFactory);

        // Create service provider
        var serviceProvider = serviceCollection.BuildServiceProvider();

        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<Program>();
    }

    /// <summary>
    /// Check is str palindrome
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool IsPalindrome(string str)
    {
        try
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(nameof(str));
            }

            str = str.ToLower();

            var length = str.Length;
            for (var i = 0; i < length / 2; i++)
            {
                if (str[i] != str[length - i - 1])
                {
                    return false;
                }
            }

            return true;

        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var str1 = str;
            _logger?.LogData(LogLevel.Error, o =>
            {
                o.Message = ex.Message;
                o.Exception = ex;
                o.Params = new { str1 };
            });

            throw;
        }
    }
}
