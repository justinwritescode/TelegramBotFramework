using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBotBase.Base
{
    /// <summary>
    /// Returns a class to manage attachments within messages.
    /// </summary>
    public class DataResult : ResultBase
    {

        //public Telegram.Bot.Args.MessageEventArgs RawMessageData { get; set; }

        public virtual UpdateResult UpdateData { get; set; }


        public virtual Contact Contact => Message.Contact;

        public virtual Location Location => Message.Location;

        public virtual Document Document => Message.Document;

        public virtual Audio Audio => Message.Audio;

        public virtual Video Video => Message.Video;

        public virtual PhotoSize[] Photos
        {
            get
            {
                return this.Message.Photo;
            }
        }


        public virtual Telegram.Bot.Types.Enums.MessageType Type
        {
            get
            {
                return this.Message?.Type ?? Telegram.Bot.Types.Enums.MessageType.Unknown;
            }
        }

        public override Message Message => UpdateData?.Message;

        /// <summary>
        /// Returns the FileId of the first reachable element.
        /// </summary>
        public virtual string FileId
        {
            get
            => (Document?.FileId ??
                        Audio?.FileId ??
                        Video?.FileId ??
                        Photos.FirstOrDefault()?.FileId);
        }

        public DataResult(UpdateResult update)
        {
            UpdateData = update;
        }


        public virtual async Task<InputOnlineFile> DownloadDocument()
        {
            var buffer = new byte[this.Document.FileSize.Value];
            var encryptedContent = new System.IO.MemoryStream(buffer);
            var file = await this.Client.TelegramClient.GetInfoAndDownloadFileAsync(this.Document.FileId, encryptedContent);

            return new InputOnlineFile(encryptedContent, this.Document.FileName);
        }


        /// <summary>
        /// Downloads a file and saves it to the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual async Task DownloadDocument(String path)
        {
            var file = await this.Client.TelegramClient.GetFileAsync(this.Document.FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await this.Client.TelegramClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// Downloads the document and returns an byte array.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte[]> DownloadRawDocument()
        {
            MemoryStream ms = new MemoryStream();
            await this.Client.TelegramClient.GetInfoAndDownloadFileAsync(this.Document.FileId, ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Downloads  a document and returns it as string. (txt,csv,etc) Default encoding ist UTF8.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<String> DownloadRawTextDocument()
        {
            return await DownloadRawTextDocument(Encoding.UTF8);
        }

        /// <summary>
        /// Downloads  a document and returns it as string. (txt,csv,etc)
        /// </summary>
        /// <returns></returns>
        public virtual async Task<String> DownloadRawTextDocument(Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            await this.Client.TelegramClient.GetInfoAndDownloadFileAsync(this.Document.FileId, ms);

            ms.Position = 0;

            var sr = new StreamReader(ms, encoding);

            return sr.ReadToEnd();
        }

        public virtual async Task<InputOnlineFile> DownloadVideo()
        {
            var buffer = new byte[this.Video.FileSize.Value];
            var encryptedContent = new System.IO.MemoryStream(buffer);
            var file = await this.Client.TelegramClient.GetInfoAndDownloadFileAsync(this.Video.FileId, encryptedContent);

            return new InputOnlineFile(encryptedContent, "");
        }

        public virtual async Task DownloadVideo(String path)
        {
            var file = await this.Client.TelegramClient.GetFileAsync(this.Video.FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await this.Client.TelegramClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        public virtual async Task<InputOnlineFile> DownloadAudio()
        {
            var buffer = new byte[this.Audio.FileSize.Value];
            var encryptedContent = new System.IO.MemoryStream(buffer);
            var file = await this.Client.TelegramClient.GetInfoAndDownloadFileAsync(this.Audio.FileId, encryptedContent);

            return new InputOnlineFile(encryptedContent, "");
        }

        public virtual async Task DownloadAudio(String path)
        {
            var file = await this.Client.TelegramClient.GetFileAsync(this.Audio.FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await this.Client.TelegramClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        public virtual async Task<InputOnlineFile> DownloadPhoto(int index)
        {
            var photo = this.Photos[index];
            var buffer = new byte[photo.FileSize.Value];
            var encryptedContent = new System.IO.MemoryStream(buffer);
            var file = await this.Client.TelegramClient.GetInfoAndDownloadFileAsync(photo.FileId, encryptedContent);

            return new InputOnlineFile(encryptedContent, "");
        }

        public virtual async Task DownloadPhoto(int index, string path)
        {
            var photo = this.Photos[index];
            var file = await this.Client.TelegramClient.GetFileAsync(photo.FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await this.Client.TelegramClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

    }
}
