using System;
using System.Threading;
using PostSharp.Aspects;

namespace Soundcloud_Playlist_Downloader
{
    [Serializable]
    internal class SafeRetry : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var success = false;
            for (var i = 0; i < 10 && !success; ++i)
            {
                try
                {
                    args.Proceed();
                    success = true;
                }
                catch (Exception)
                {
                    // Logging would be appropriate in a more robust application
                    Thread.Sleep(new Random().Next(10)*1000);
                }
            }

            if (!success)
            {
                throw new Exception("One or more exceptions occurred during the execution of " +
                                    args.Method + "(" + args.Arguments + ")");
            }
        }
    }

    [Serializable]
    internal class SilentFailure : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            try
            {
                args.Proceed();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}