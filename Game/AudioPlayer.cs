using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace MonoRogue {
    public class AudioPlayer {
        private List<SongHandler> Soundtrack;
        private int SongIndex;

        // Some vars for fading in and out between songs
        private SongHandler SubSong;
        private SongHandler NextSong;
        private static long MusicFadeTime = System.TimeSpan.FromSeconds(0.3).Ticks;
        private long FadeOut;
        private long FadeIn;

        public AudioPlayer(ContentManager content) {
            MediaPlayer.Volume = Constants.MusicVolume;
            SongHandler.LoadSongs(content);

            Soundtrack = new List<SongHandler>();
            Soundtrack.Add(SongHandler.GameGame);
            Soundtrack.Add(SongHandler.GaseousTethanus);
            Soundtrack.Add(SongHandler.HighInTheMountains);
            Soundtrack.Add(SongHandler.FinalBattle);
        }

        public void Update(System.TimeSpan elapsed) {
            if (SubSong != null) {
                SubSong.Update(elapsed);
                if (SubSong.Finished) {
                    SubSong.Restart();
                }
            } else {
                SongHandler song = Soundtrack[SongIndex];
                song.Update(elapsed);
                if (song.Finished) {
                    if (SongIndex == Soundtrack.Count - 1) { SongIndex = 0; }
                    else { SongIndex++; }
                    
                    song = Soundtrack[SongIndex];
                    song.Start();
                }
            }

            if (FadeOut > 0) {
                long ticks = elapsed.Ticks;
                FadeOut -= ticks;
                MediaPlayer.Volume -= ((float)ticks / MusicFadeTime) * Constants.MusicVolume;
                if (FadeOut <= 0) {
                    SubSong = NextSong;
                    NextSong = null;

                    if (SubSong != null) { 
                        SubSong.Start(); 
                    } else {
                        Soundtrack[SongIndex].Start();
                    }

                    FadeIn = MusicFadeTime;
                }
            } else if (FadeIn > 0) {
                long ticks = elapsed.Ticks;
                FadeIn -= ticks;
                MediaPlayer.Volume += ((float)ticks / MusicFadeTime) * Constants.MusicVolume;
            }
        }

        public void ChangeSong(SongHandler song) {
            // If null, swap to main soundtrack
            if (SubSong == song) { return; }
            NextSong = song;
            FadeOut = MusicFadeTime;
        }

        public void SetSong(SongHandler song) {
            SubSong = song;
            NextSong = null;
            FadeOut = 0;
            FadeIn = 0;

            if (SubSong != null) { 
                SubSong.Start(); 
            } else {
                Soundtrack[SongIndex].Start();
            }
        }
    }

    public class SongHandler {
        private Song Song;
        private System.TimeSpan CurrentTime;
        private System.TimeSpan TotalTime;
        public bool Finished;

        private SongHandler(Song song) {
            Song = song;
            CurrentTime = System.TimeSpan.Zero;
            TotalTime = song.Duration;
        }

        public void Start() {
            MediaPlayer.Play(Song, CurrentTime);
            Finished = false;
            System.Console.WriteLine(TotalTime);
        }
        public void Restart() {
            CurrentTime = System.TimeSpan.Zero;
            MediaPlayer.Play(Song, CurrentTime);
            Finished = false;
        }
        public void Stop() {
            CurrentTime = System.TimeSpan.Zero;
        }

        public void Update(System.TimeSpan elapsed) {
            CurrentTime += elapsed;
            System.Console.WriteLine(TotalTime - CurrentTime);
            if (CurrentTime >= TotalTime) {
                Finished = true;
                Stop();
            }
        }

        // Main soundtrack songs
        public static SongHandler GameGame;
        public static SongHandler GaseousTethanus;
        public static SongHandler HighInTheMountains;
        public static SongHandler FinalBattle;

        // Subscreen songs
        public static SongHandler StartSong;    // LavaCity
        public static SongHandler MenuSong;     // ShipInterior
        public static SongHandler VictorySong;  // RunningWild
        public static SongHandler DeathSong;    // TheEnd

        public static void LoadSongs(ContentManager content) {
            GameGame = new SongHandler(content.Load<Song>("Audio/GameGame"));
            GaseousTethanus = new SongHandler(content.Load<Song>("Audio/GaseousTethanus"));
            HighInTheMountains = new SongHandler(content.Load<Song>("Audio/HighInTheMountains"));
            FinalBattle = new SongHandler(content.Load<Song>("Audio/FinalBattle"));

            StartSong = new SongHandler(content.Load<Song>("Audio/LavaCity"));
            MenuSong = new SongHandler(content.Load<Song>("Audio/ShipInterior"));
            VictorySong = new SongHandler(content.Load<Song>("Audio/RunningWild"));
            DeathSong = new SongHandler(content.Load<Song>("Audio/TheEnd"));
        }
    }
}
