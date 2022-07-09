using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotBase.Localizations
{
    public abstract class Localization
    {
        public Dictionary<String, String> Values = new Dictionary<string, string>();

        public string this[String key]
        {
            get
            {
                return Values[key];
            }
        }

        public Localization()
        {
            
            
        }

    }
}

