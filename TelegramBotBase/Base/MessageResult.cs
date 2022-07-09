using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotBase.Sessions;

namespace TelegramBotBase.Base
{
    public class MessageResult : ResultBase
    {

        public Telegram.Bot.Types.Update UpdateData { get; set; }

        /// <summary>
        /// Returns the Device/ChatId
        /// </summary>
        public override long DeviceId
        {
            get
            {
                return this.UpdateData?.Message?.Chat?.Id
                    ?? this.UpdateData?.EditedMessage?.Chat.Id
                    ?? this.UpdateData?.CallbackQuery.Message?.Chat.Id
                    ?? Device?.DeviceId
                    ?? 0;
            }
        }

        public virtual DeviceSession Device
        {
            get;
            set;
        }

        /// <summary>
        /// The message id
        /// </summary>
        public override int MessageId
        {
            get
            {
                return this.UpdateData?.Message?.MessageId
                    ?? this.Message?.MessageId
                    ?? this.UpdateData?.CallbackQuery?.Message?.MessageId
                    ?? 0;
            }
        }

        public virtual string Command
        {
            get
            {
                return this.UpdateData?.Message?.Text ?? "";
            }
        }

        public virtual string MessageText
        {
            get
            {
                return this.UpdateData?.Message?.Text ?? "";
            }
        }

        public virtual Telegram.Bot.Types.Enums.MessageType MessageType
        {
            get
            {
                return Message?.Type ?? Telegram.Bot.Types.Enums.MessageType.Unknown;
            }
        }

        public virtual Message Message
        {
            get
            {
                return this.UpdateData?.Message
                    ?? this.UpdateData?.EditedMessage
                    ?? this.UpdateData?.ChannelPost
                    ?? this.UpdateData?.EditedChannelPost
                    ?? this.UpdateData?.CallbackQuery?.Message;
            }
        }

        /// <summary>
        /// Is this an action ? (i.e. button click)
        /// </summary>
        public virtual bool IsAction
        {
            get
            {
                return (this.UpdateData.CallbackQuery != null);
            }
        }

        /// <summary>
        /// Is this a command ? Starts with a slash '/' and a command
        /// </summary>
        public virtual bool IsBotCommand
        {
            get
            {
                return (this.MessageText.StartsWith("/"));
            }
        }

        /// <summary>
        /// Returns a List of all parameters which has been sent with the command itself (i.e. /start 123 456 789 => 123,456,789)
        /// </summary>
        public virtual List<String> BotCommandParameters
        {
            get
            {
                if (!IsBotCommand)
                    return new List<string>();

                //Split by empty space and skip first entry (command itself), return as list
                return this.MessageText.Split(' ').Skip(1).ToList();
            }
        }

        /// <summary>
        /// Returns just the command (i.e. /start 1 2 3 => /start)
        /// </summary>
        public virtual string BotCommand
        {
            get
            {
                if (!IsBotCommand)
                    return null;

                return this.MessageText.Split(' ')[0];
            }
        }

        /// <summary>
        /// Returns if this message will be used on the first form or not.
        /// </summary>
        public virtual bool IsFirstHandler { get; set; } = true;

        public virtual bool IsHandled { get; set; } = false;

        public virtual string RawData
        {
            get
            {
                return this.UpdateData?.CallbackQuery?.Data;
            }
        }

        public virtual T GetData<T>()
            where T : class
        {
            T cd = null;
            try
            {
                cd = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(this.RawData);

                return cd;
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// Confirm incomming action (i.e. Button click)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task ConfirmAction(String message = "", bool showAlert = false, string urlToOpen = null)
        {
            await this.Device.ConfirmAction(this.UpdateData.CallbackQuery.Id, message, showAlert, urlToOpen);
        }

        public override async Task DeleteMessage()
        {
            try
            {
                await base.DeleteMessage(this.MessageId);
            }
            catch
            {

            }
        }

        internal MessageResult()
        {

        }

        public MessageResult(Telegram.Bot.Types.Update update)
        {
            this.UpdateData = update;

        }

    }
}
