namespace Gnatta.Facebook.Logging
{
    // TODO
    public static class LogFactory
    {
        public static ILog Build(bool verbose = true)
        {
            return new ConsoleLogger(verbose);
        }
    }
}
