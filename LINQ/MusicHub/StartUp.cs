using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

using MusicHub.Data.Models;

namespace MusicHub
{
    using System;

    using Data;

    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here

            //string albumResult = ExportAlbumsInfo(context, 9);

            //Console.WriteLine(albumResult);

            string songResult = ExportSongsAboveDuration(context, 4).Trim();

            Console.WriteLine(songResult);

        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albumsInfo = context
                             .Albums
                             .ToArray()
                             .Where(a => a.ProducerId == producerId)
                             .OrderByDescending(a => a.Price)
                             .Select(a => new
                             {
                                 AlbumName = a.Name,
                                 ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                 ProducerName = a.Producer.Name,
                                 Songs = a.Songs
                                          .ToArray()
                                          .Select(s => new
                                          {
                                              SongName = s.Name,
                                              Price = s.Price.ToString("f2"),
                                              Writer = s.Writer.Name
                                          })
                                          .OrderByDescending(s => s.SongName)
                                          .ThenBy(s => s.Writer)
                                          .ToArray(),
                                 TotalAlbumPrice = a.Price.ToString("f2")
                             })
                             .ToArray();

            foreach (var album in albumsInfo)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}")
                  .AppendLine($"-ReleaseDate: {album.ReleaseDate}")
                  .AppendLine($"-ProducerName: {album.ProducerName}")
                  .AppendLine($"-Songs:");

                int i = 1;
                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{i++}")
                      .AppendLine($"---SongName: {song.SongName}")
                      .AppendLine($"---Price: {song.Price}")
                      .AppendLine($"---Writer: {song.Writer}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {

            StringBuilder sb = new StringBuilder();

            var songInfo = context
                                    .Songs
                                    .ToArray()
                                    .Where(s => s.Duration.TotalSeconds > duration)
                                    .Select(s => new
                                    {
                                        SongName = s.Name,
                                        Writer = s.Writer.Name,
                                        PerformerName = s.SongPerformers
                                                     .ToArray()
                                                     .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                                     .FirstOrDefault(),
                                        AlbumProducer = s.Album.Producer.Name,
                                        Duration = s.Duration.ToString("c", CultureInfo.InvariantCulture)
                                    })
                                    .OrderBy(s => s.SongName)
                                    .ThenBy(s => s.Writer)
                                    .ThenBy(s => s.PerformerName)
                                    .ToArray();


            int i = 1;
            foreach (var song in songInfo)
            {
                sb.AppendLine($"-Song #{i++}")
                  .AppendLine($"---SongName: {song.SongName}")
                  .AppendLine($"---Writer: {song.Writer}")
                  .AppendLine($"---Performer: {song.PerformerName}")
                  .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                  .AppendLine($"---Duration: {song.Duration}");
            }


            return sb.ToString().TrimEnd();

        }
    }
}
