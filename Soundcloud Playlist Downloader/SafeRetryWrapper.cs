using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Soundcloud_Playlist_Downloader
{
    [Serializable]
    class SafeRetry : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            bool success = false;
            for (int i = 0; i < 10 && !success; ++i)
            {
                try
                {
                    args.Proceed();
                    success = true;
                }
                catch (Exception)
                {
                    // Logging would be appropriate in a more robust application
                    Thread.Sleep((new Random()).Next(10) * 1000);
                }
            }

            if (!success) {
                throw new Exception("One or more exceptions occurred during the execution of " +
                    args.Method + "(" + args.Arguments + ")");
            }
        }
    }

    [Serializable]
    class SilentFailure : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            try
            {
                args.Proceed();
            }
            catch (Exception) { }
        }
    }
}
