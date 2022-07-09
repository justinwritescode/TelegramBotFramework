using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotBase.Form
{
    public class DynamicButton : ButtonBase
    {
        public override string Text
        {
            get
            {
                return GetText?.Invoke() ?? m_text;
            }
            set
            {
                m_text = value;
            }
        }

        private string m_text = "";

        private Func<String> GetText;

        public DynamicButton(String Text, string Value, string Url = null)
        {
            this.Text = Text;
            this.Value = Value;
            this.Url = Url;
        }

        public DynamicButton(Func<String> GetText, string Value, string Url = null)
        {
            this.GetText = GetText;
            this.Value = Value;
            this.Url = Url;
        }


    }
}
