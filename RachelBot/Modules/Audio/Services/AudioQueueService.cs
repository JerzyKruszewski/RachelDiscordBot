using Discord.WebSocket;
using RachelBot.Modules.Audio.Objects;
using RachelBot.Services.Storage;
using SharpLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.Audio.Services
{
    public class AudioQueueService
    {
        private static readonly IStorageService _storage = new JsonStorageService();
        private static readonly IList<AudioQueue> _audioQueues;
        private static readonly string _folderPath = "./Resources";
        private static readonly string _queuesFile = $"{_folderPath}/audioQueues.json";

        static AudioQueueService()
        {
            _storage.EnsureDirectoryExist(_folderPath);

            if (_storage.FileExist(_queuesFile))
            {
                _audioQueues = _storage.RestoreObject<List<AudioQueue>>(_queuesFile);
                return;
            }

            _audioQueues = new List<AudioQueue>();
            SaveQueues();
        }

        public static void SaveQueues()
        {
            _storage.StoreObject(_audioQueues, _queuesFile);
        }

        public static AudioQueue GetAudioQueue(SocketGuild guild)
        {
            return GetOrCreateAudioQueue(guild.Id);
        }

        private static AudioQueue GetOrCreateAudioQueue(ulong id)
        {
            AudioQueue queue = _audioQueues.FirstOrDefault(q => q.GuildId == id);

            if (queue is null)
            {
                queue = CreateQueue(id);
            }

            return queue;
        }

        private static AudioQueue CreateQueue(ulong id)
        {
            AudioQueue newQueue = new AudioQueue()
            {
                GuildId = id,
                PlayingTrackIndex = -1,
                Queue = new List<AudioQueueItem>()
            };

            _audioQueues.Add(newQueue);
            SaveQueues();
            return newQueue;
        }

        public static List<AudioQueueItem> GetOrCreateGuildQueue(LavalinkTrack track, AudioQueue audioQueue, ulong channelId)
        {
            AudioQueueItem firstTrack = audioQueue.Queue.ElementAtOrDefault(0);

            if (firstTrack is null)
            {
                audioQueue.Queue = AddNewTrack(track, channelId);
                SaveQueues();
                return audioQueue.Queue;
            }

            audioQueue.Queue.Add(new AudioQueueItem()
            {
                RequestedInChannelId = channelId,
                Track = track
            });

            SaveQueues();
            return audioQueue.Queue;
        }

        private static List<AudioQueueItem> AddNewTrack(LavalinkTrack track, ulong channelId)
        {
            return new List<AudioQueueItem>()
            {
                new AudioQueueItem()
                {
                    RequestedInChannelId = channelId,
                    Track = track
                }
            };
        }
    }
}
