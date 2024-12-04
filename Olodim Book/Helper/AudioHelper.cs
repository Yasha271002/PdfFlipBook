using NAudio.Wave;

public class AudioHelper
{
    private IWavePlayer _waveOut;
    private AudioFileReader _audioFileReader;
    private string _audioFilePath;
    private float _volume;

    public AudioHelper(string audioFilePath, float volume)
    {
        _audioFilePath = audioFilePath;
        _volume = volume;
        InitializeAudio();
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
        {
            _audioFileReader.Position = 0;
        }
        _waveOut.Play();
    }

    public void Stop()
    {
        _waveOut.Stop();
    }

    public void Pause()
    {
        _waveOut.Pause();
    }

    public void Resume()
    {
        _waveOut.Play();
    }

    public bool IsPlaying => _waveOut.PlaybackState == PlaybackState.Playing;

    public void SetVolume(float volume)
    {
        _volume = volume;
        if (_audioFileReader != null)
        {
            _audioFileReader.Volume = volume;
        }
    }

    public float GetVolume()
    {
        return _volume;
    }

    public void Exit()
    {
        _waveOut.Stop();
        _audioFileReader.Dispose();
        _waveOut.Dispose();
    }
}