#if IOS
using AVFoundation;
#endif
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

        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
        };
    }

    private async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        //foreach (var barcode in e.Results)
        //    Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        var first = e.Results?.FirstOrDefault();
        if (first is null) return;
        await PlayAudioAsync();
        Dispatcher.Dispatch(() =>
        {
            // Update BarcodeGeneratorView
            barcodeGenerator.ClearValue(BarcodeGeneratorView.ValueProperty);
            barcodeGenerator.Format = first.Format;
            barcodeGenerator.Value = first.Value;
                    
                    
            // Update Label
            ResultLabel.Text = $"Barcodes: {first.Format} -> {first.Value}";


            if (first.Value.StartsWith("http"))
            {
                
            }

            //var parsed =  ResultParser.parseResult(new Result(first.Value, first.Raw,
            //    first.PointsOfInterest.Select(p => new ResultPoint(p.X, p.Y)).ToArray(),
            //    BarcodeFormat.QR_CODE));
                    
        });
    }

    public async Task PlayAudioAsync()
    {
#if IOS
    AVFoundation.AVAudioSession.SharedInstance().SetCategory(AVFoundation.AVAudioSessionCategory.Playback);
#endif
        using var audioPlayer = AudioManager
            .Current
            .CreatePlayer(await FileSystem.OpenAppPackageFileAsync("short-beep-tone.mp3"));

        audioPlayer.Play();
    }

    void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        barcodeView.CameraLocation = barcodeView.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
    }

    void TorchButton_Clicked(object sender, EventArgs e)
    {
        barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
    }
}