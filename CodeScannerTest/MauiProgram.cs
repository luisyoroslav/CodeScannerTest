#region

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Plugin.Maui.Audio;
using ZXing.Net.Maui.Controls;

#endregion

namespace CodeScannerTest
{
    public static class MauiProgram
    {
        #region Methods

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder
                .UseBarcodeReader()
                .AddAudio(playbackOptions =>
                    {
#if IOS || MACCATALYST
                        playbackOptions.Category = AVFoundation.AVAudioSessionCategory.Playback;
#endif
                    },
                    recordingOptions =>
                    {
#if IOS || MACCATALYST
                        recordingOptions.Category = AVFoundation.AVAudioSessionCategory.Record;
                        recordingOptions.Mode = AVFoundation.AVAudioSessionMode.Default;
                        recordingOptions.CategoryOptions = AVFoundation.AVAudioSessionCategoryOptions.MixWithOthers;
#endif
                    });
            return builder.Build();
        }

        #endregion
    }
}