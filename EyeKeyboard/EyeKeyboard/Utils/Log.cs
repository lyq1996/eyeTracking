using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeKeyboard.Utils
{
    class Log
    {
        public static void Error(object message)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Error(message);
        }
        public static void Error(object message, System.Exception exception)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Error(message, exception);
        }

        public static void Info(object message)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Info(message);
        }

        public static void Info(object message, System.Exception ex)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Info(message, ex);
        }

        public static void Warn(object message)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Warn(message);
        }

        public static void Warn(object message, System.Exception ex)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Warn(message, ex);
        }

        public static void Debug(object message)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Debug(message);
        }

        public static void Debug(object message, System.Exception ex)
        {
            ILog logger = LogManager.GetLogger("EyeKeyboard");
            logger.Debug(message, ex);
        }

      
    }
}