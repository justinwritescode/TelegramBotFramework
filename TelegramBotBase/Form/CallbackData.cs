using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TelegramBotBase.Form
{
    /// <summary>
    /// Base class for serializing buttons and data
    /// </summary>
    public class CallbackData
    {
        [JsonProperty("m")]
        public virtual string Method { get; set; }

        [JsonProperty("v")]
        public virtual string Value { get; set; }


        public CallbackData()
        {

        }

        public CallbackData(String method, string value)
        {
            this.Method = method;
            this.Value = value;
        }

        public static string Create(String method, string value)
        {
            return new CallbackData(method, value).Serialize();
        }

        /// <summary>
        /// Serializes data to json string
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize()
        {
            string s = "";
            try
            {

                s = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
            catch
            {


            }
            return s;
        }

        /// <summary>
        /// Deserializes data from json string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CallbackData Deserialize(String data)
        {
            CallbackData cd = null;
            try
            {
                cd = Newtonsoft.Json.JsonConvert.DeserializeObject<CallbackData>(data);

                return cd;
            }
            catch
            {

            }

            return null;
        }

    }
}
