using System;
using PostSharp.Aspects;

namespace Soundcloud_Playlist_Downloader
{
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