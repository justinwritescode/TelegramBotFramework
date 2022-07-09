using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotBase.Form
{
    /// <summary>
    /// Base class for button handling
    /// </summary>
    public class TagButtonBase : ButtonBase
    {
        public string Tag { get; set; }

        public TagButtonBase()
        {

        }

        public TagButtonBase(String Text, string Value, string Tag)
        {
            this.Text = Text;
            this.Value = Value;
            this.Tag = Tag;
        }


        /// <summary>
        /// Returns an inline Button
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public override InlineKeyboardButton ToInlineButton(ButtonForm form)
        {
            string id = (form.DependencyControl != null ? form.DependencyControl.ControlID + "_" : "");

            return InlineKeyboardButton.WithCallbackData(this.Text, id + this.Value);

        }


        /// <summary>
        /// Returns a KeyBoardButton
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public override KeyboardButton ToKeyboardButton(ButtonForm form)
        {
            return new KeyboardButton(this.Text);
        }

    }
}
