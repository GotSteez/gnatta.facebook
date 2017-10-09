using System;

namespace Gnatta.Facebook.Logging
{
    public interface ILog
    {
        void Debug(object log);
        void Info(object log);
        void Error(object log, Exception ex = null);
    }
}