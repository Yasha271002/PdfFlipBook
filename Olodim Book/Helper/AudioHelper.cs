using System;
using System.Timers;
using System.Windows.Threading;
using NAudio.Wave;

public class AudioHelper
{
    private IWavePlayer _waveOut;
    private AudioFileReader _audioFileReader;
    private string _audioFilePath;
    private float _volume;

    private DispatcherTimer _timer;

    public AudioHelper(string audioFilePath, float volume)
    {
        _audioFilePath = audioFilePath;
        _volume = volume;
        InitializeAudio();
        _timer = new DispatcherTimer();
    }

    private void InitializeTimer()
    {
        _timer.Tick += TimerOnTick;
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Start();
    }

    private void InitializeAudio()
    {
        _audioFileReader = new AudioFileReader(_audioFilePath);
        _audioFileReader.Volume = _volume;
        _waveOut = new WaveOutEvent();
        _waveOut.Init(_audioFileReader);
    }

    public void Play()
    {
        if (_audioFileReader.Position > 0)
            _audioFileReader.Position = 0;

        _waveOut.Play();
    }

    public void Stop()
    {
        _waveOut.Stop();
    }

    public void InfinityPlay()
    {
        if (_audioFileReader.Position > 0)
            _audioFileReader.Position = 0;
        _waveOut.Play();
        InitializeTimer();
    }

    public bool IsPlaying => _waveOut.PlaybackState == PlaybackState.Playing;

    public void Exit()
    {
        _timer.Tick -= TimerOnTick;
        _timer?.Stop();
        _waveOut.Stop();
        _audioFileReader.Dispose();
        _waveOut.Dispose();
    }


    private void TimerOnTick(object sender, EventArgs e)
    {
        if (_waveOut.PlaybackState != PlaybackState.Stopped) return;
        InfinityPlay();
    }
}