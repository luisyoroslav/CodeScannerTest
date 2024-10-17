#if IOS
using AVFoundation;
#endif
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using ZXing;
using ZXing.Client.Result;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using BarcodeFormat = ZXing.BarcodeFormat;

namespace CodeScannerTest;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BarcodeView.Options = new BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            //TryHarder = true
            //AutoRotate = true,
            //TryInverted = true,
            //TryHarder = true,
        };
    }
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private string? _lastValue = null;

    private async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {

        await _semaphore.WaitAsync();
        //foreach (var barcode in e.Results)
        //    Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");
        try
        {
            var first = e.Results?.FirstOrDefault();
            if (first is null) return;

            BarcodeView.IsDetecting = false;
            var newValue = $"{first.Format}|{first.Value}";

            if (newValue == _lastValue) return;
            _lastValue = newValue;
            var tasks = new List<Task>
            {
                PlayAudioAsync(),
                Dispatcher.DispatchAsync(() =>
                {
                    Vibration.Default.Vibrate(TimeSpan.FromSeconds(5));
                    //HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                    // Update BarcodeGeneratorView
                    BarcodeGenerator.ClearValue(BarcodeGeneratorView.ValueProperty);
                    //barcodeGenerator.Format = first.Format;
                    BarcodeGenerator.Value = first.Value;
                    //first.PointsOfInterest[0]
                    // Update Label
                    ResultLabel.Text = $"Barcodes: {first.Format} -> {first.Value} \r\n";
                    ResultLabel.Text += $"Points: {string.Join(";",first.PointsOfInterest.Select(s => $"({s.X},{s.Y})"))} \r\n";
                    
                    Vibration.Default.Cancel();
                    //var parsed =  ResultParser.parseResult(new Result(first.Value, first.Raw,
                    //    first.PointsOfInterest.Select(p => new ResultPoint(p.X, p.Y)).ToArray(),
                    //    BarcodeFormat.QR_CODE));
                })
            };
            await Task.WhenAll(tasks);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
        finally
        {
            _semaphore.Release();
            BarcodeView.IsDetecting = true;
        }
        
        
    }

    public async Task PlayAudioAsync()
    {
#if IOS
    //AVFoundation.AVAudioSession.SharedInstance().SetCategory(AVFoundation.AVAudioSessionCategory.Playback);
#endif
        
        using var audioPlayer = AudioManager
            .Current
            .CreatePlayer( await FileSystem.OpenAppPackageFileAsync("beep-01.mp3"), new AudioPlayerOptions
            {
#if IOS || MACCATALYST
                CategoryOptions = AVFoundation.AVAudioSessionCategoryOptions.MixWithOthers
#endif
            });
        //audioPlayer.Volume = 1;
        //var tcs = new TaskCompletionSource<bool>();
        //audioPlayer.PlaybackEnded += (sender, args) =>
        //{
        //    tcs.SetResult(true);
        //    audioPlayer.Dispose();
        //};
        await audioPlayer.PlayAsync();

        //await tcs.Task.ConfigureAwait(false);
    }

    void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        BarcodeView.CameraLocation = BarcodeView.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
    }

    void TorchButton_Clicked(object sender, EventArgs e)
    {
        BarcodeView.IsTorchOn = !BarcodeView.IsTorchOn;
    }

    private void BarcodeView_OnFrameReady(object? sender, CameraFrameBufferEventArgs e)
    {
        Debug.WriteLine("BarcodeView_OnFrameReady");
    }
}