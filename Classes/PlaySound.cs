using System;
using System.IO;
using NAudio.Wave;

namespace Play_Sound
{
    public class PlaySound : IDisposable
    {
        public bool IsPlaying { get; set; }
        private readonly WaveFileReader file;
        private readonly WaveOutEvent playing;
        private readonly LoopStream loopStream;

        public PlaySound(byte[] fileBytes, bool loop)
        {
            try
            {
                file = new WaveFileReader(new MemoryStream(fileBytes));
                loopStream = new LoopStream(file);
                playing = new WaveOutEvent();
                if (!loop) playing.Init(file);
                else playing.Init(loopStream);
            }
            catch
            {
                Dispose();
            }
        }

        public void Play(float volume)
        {
            if (file == null) return;
            try
            {
                file.Position = 0;
                playing.Volume = volume;
                playing.Play();
            }
            catch { }
        }

        public int GetRemeinTime()
        {
            if (file == null) return 0;
            return (int)(file.TotalTime.TotalSeconds - file.CurrentTime.TotalSeconds);
        }

        public void PlayFromThe(float volume, long position)
        {
            if (file == null) return;
            try
            {
                file.Position = position;
                playing.Volume = volume;
                playing.Play();
            }
            catch { }
        }

        public void PlayWithWait(float volume)
        {
            if (file == null) return;
            if (IsPlaying) return;
            try
            {
                file.Position = 0;
                playing.Volume = volume;
                playing.PlaybackStopped += Playing_PlaybackStopped;
                playing.Play();
                IsPlaying = true;
                return;
            }
            catch
            {
                IsPlaying = false;
                return;
            }
        }

        private void Playing_PlaybackStopped(object sender, StoppedEventArgs e) => IsPlaying = false;

        public void PlayWithDispose(float volume)
        {
            if (file == null) return;
            try
            {
                file.Position = 0;
                playing.Volume = volume;
                playing.PlaybackStopped += new EventHandler<StoppedEventArgs>(Stoped);
                playing.Play();
            }
            catch { }
        }

        private void Stoped(object sender, EventArgs e) => Dispose();

        public void LoopPlay(float volume)
        {
            if (file == null) return;
            file.Position = 0;
            playing.Volume = volume;
            playing.Play();
        }

        public void SetVolume(float value) => playing.Volume = value;

        public long Check() => playing.GetPosition();

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

    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        public override long Length => sourceStream.Length;

        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                        break;
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}