#region

using Plugin.Maui.Audio;

#endregion

namespace CodeScannerTest
{
    public static class PlayerExt
    {
        #region Methods

        public static async Task PlayAsync(this IAudioPlayer player)
        {
            //var tcs = new TaskCompletionSource<bool>();

            void LocalFunc(object? sender, EventArgs args)
            {
                player.PlaybackEnded -= LocalFunc;
                //player.Dispose();
            }

            //player.PlaybackEnded += LocalFunc;
            player.Play();

            while (player.IsPlaying)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }

            //return tcs.Task;
            //return Task.FromResult(true);
        }

        #endregion
    }
}