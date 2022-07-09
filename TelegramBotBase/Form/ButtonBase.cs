﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotBase.Form
{
    [DebuggerDisplay("{Text}, {Value}")]
    /// <summary>
    /// Base class for button handling
    /// </summary>
    public class ButtonBase
    {
        public virtual string Text { get; set; }

        public virtual string Value { get; set; }

        public virtual string Url { get; set; }

        public ButtonBase()
        {

        }

        public ButtonBase(String Text, string Value, string Url = null)
        {
            this.Text = Text;
            this.Value = Value;
            this.Url = Url;
        }


        /// <summary>
        /// Returns an inline Button
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual InlineKeyboardButton ToInlineButton(ButtonForm form)
        {
            string id = (form.DependencyControl != null ? form.DependencyControl.ControlID + "_" : "");
            if (this.Url == null)
            {
                return InlineKeyboardButton.WithCallbackData(this.Text, id + this.Value);
            }

            var ikb = new InlineKeyboardButton(this.Text);

            //ikb.Text = this.Text;
            ikb.Url = this.Url;

            return ikb;

        }


        /// <summary>
        /// Returns a KeyBoardButton
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual KeyboardButton ToKeyboardButton(ButtonForm form)
        {
            return new KeyboardButton(this.Text);
        }

    }
}
