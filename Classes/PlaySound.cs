using System;
using CSCore;
using System.IO;
using SLIL.Classes;
using CSCore.SoundOut;
using CSCore.Codecs.WAV;

namespace Play_Sound
{
    public class PlaySound : IDisposable
    {
        public bool IsPlaying { get; private set; }
        private readonly WaveFileReader file;
        private readonly LoopStream loopStream;
        private readonly ISoundOut playing;

        public PlaySound(byte[] fileBytes, bool loop)
        {
            try
            {
                file = new WaveFileReader(new MemoryStream(fileBytes));
                playing = new WasapiOut();
                if (loop)
                {
                    loopStream = new LoopStream(file);
                    playing.Initialize(loopStream);
                }
                else playing.Initialize(file);
            }
            catch { Dispose(); }
        }

        public void Play(float volume)
        {
            if (file == null) return;
            try
            {
                file.Position = 0;
                SetVolume(volume);
                playing.Play();
            }
            catch { }
        }

        public int GetRemainTime()
        {
            if (file == null) return 0;
            return (int)(file.Length / (double)file.WaveFormat.BytesPerSecond -
                        file.Position / (double)file.WaveFormat.BytesPerSecond);
        }

        public void PlayFromThe(float volume, long position)
        {
            if (file == null) return;
            try
            {
                file.Position = position;
                SetVolume(volume);
                playing.Play();
            }
            catch { }
        }

        public void PlayWithWait(float volume)
        {
            if (file == null || IsPlaying) return;
            try
            {
                file.Position = 0;
                SetVolume(volume);
                playing.Stopped += Playing_PlaybackStopped;
                playing.Play();
                IsPlaying = true;
            }
            catch { IsPlaying = false; }
        }

        private void Playing_PlaybackStopped(object sender, PlaybackStoppedEventArgs e) => IsPlaying = false;

        public void PlayWithDispose(float volume)
        {
            if (file == null) return;
            try
            {
                file.Position = 0;
                SetVolume(volume);
                playing.Stopped += Stoped;
                playing.Play();
            }
            catch { }
        }

        private void Stoped(object sender, PlaybackStoppedEventArgs e) => Dispose();

        public float GetVolume() => playing.Volume;

        public void LoopPlay(float volume) => Play(volume);

        public void SetVolume(float value)
        {
            if (!ML.WithinOne(value)) return;
            if (playing != null) playing.Volume = value;
        }

        public long Check() => file?.Position ?? 0;

        public void Stop()
        {
            playing?.Stop();
            if (file != null) file.Position = 0;
        }

        public void Dispose()
        {
            file?.Dispose();
            loopStream?.Dispose();
            playing?.Dispose();
        }
    }

    public class LoopStream : IWaveSource
    {
        private readonly IWaveSource SourceStream;
        public bool CanSeek => SourceStream.CanSeek;
        public WaveFormat WaveFormat => SourceStream.WaveFormat;
        public long Position
        {
            get => SourceStream.Position;
            set => SourceStream.Position = value;
        }
        public long Length => SourceStream.Length;

        public LoopStream(IWaveSource sourceStream) => SourceStream = sourceStream;

        public int Read(byte[] buffer, int offset, int count)
        {
            int read = SourceStream.Read(buffer, offset, count);
            if (read < count)
            {
                SourceStream.Position = 0;
                read += SourceStream.Read(buffer, offset + read, count - read);
            }
            return read;
        }

        public void Dispose() => SourceStream.Dispose();
    }
}