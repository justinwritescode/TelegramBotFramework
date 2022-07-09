using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBase.Base;
using TelegramBotBase.Controls.Hybrid;

namespace TelegramBotBase.Form
{
    /// <summary>
    /// Base class for an buttons array
    /// </summary>
    public class ButtonForm : ICloneable
    {
        public virtual List<ButtonRow> Buttons { get; } = new List<ButtonRow>();

        public virtual IReplyMarkup Markup { get; set; }

        public virtual ControlBase DependencyControl { get; set; }

        /// <summary>
        /// Contains the number of rows.
        /// </summary>
        public virtual int Rows
        {
            get
            {
                return Buttons.Count;
            }
        }

        /// <summary>
        /// Contains the highest number of columns in an row.
        /// </summary>
        public virtual int Cols
        {
            get
            {
                return Buttons.Select(a => a.Count).OrderByDescending(a => a).FirstOrDefault();
            }
        }


        public virtual ButtonRow this[int row]
        {
            get
            {
                return Buttons[row];
            }
        }

        public ButtonForm()
        {

        }

        public ButtonForm(ControlBase control)
        {
            this.DependencyControl = control;
        }

        public virtual void AddButtonRow(String Text, string Value, string Url = null)
        {
            Buttons.Add(new List<ButtonBase>() { new ButtonBase(Text, Value, Url) });
        }

        //public void AddButtonRow(ButtonRow row)
        //{
        //    Buttons.Add(row.ToList());
        //}

        public virtual void AddButtonRow(ButtonRow row)
        {
            Buttons.Add(row);
        }

        public virtual void AddButtonRow(params ButtonBase[] row)
        {
            AddButtonRow(row.ToList());
        }

        public virtual void AddButtonRows(IEnumerable<ButtonRow> rows)
        {
            Buttons.AddRange(rows);
        }

        public virtual void InsertButtonRow(int index, IEnumerable<ButtonBase> row)
        {
            Buttons.Insert(index, row.ToList());
        }

        public virtual void InsertButtonRow(int index, ButtonRow row)
        {
            Buttons.Insert(index, row);
        }

        //public void InsertButtonRow(int index, params ButtonBase[] row)
        //{
        //    InsertButtonRow(index, row.ToList());
        //}

        public static T[][] SplitTo<T>(IEnumerable<T> items, int itemsPerRow = 2)
        {
            T[][] splitted = default(T[][]);

            try
            {
                var t = items.Select((a, index) => new { a, index })
                             .GroupBy(a => a.index / itemsPerRow)
                             .Select(a => a.Select(b => b.a).ToArray()).ToArray();

                splitted = t;
            }
            catch
            {

            }

            return splitted;
        }

        public virtual int Count
        {
            get
            {
                if (this.Buttons.Count == 0)
                    return 0;

                return this.Buttons.Select(a => a.ToArray()).ToList().Aggregate((a, b) => a.Union(b).ToArray()).Length;
            }
        }

        /// <summary>
        /// Add buttons splitted in the amount of columns (i.e. 2 per row...)
        /// </summary>
        /// <param name="buttons"></param>
        /// <param name="buttonsPerRow"></param>
        public virtual void AddSplitted(IEnumerable<ButtonBase> buttons, int buttonsPerRow = 2)
        {
            var sp = SplitTo<ButtonBase>(buttons, buttonsPerRow);

            foreach (var bl in sp)
            {
                AddButtonRow(bl);
            }
        }

        /// <summary>
        /// Returns a range of rows from the buttons.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual List<ButtonRow> GetRange(int start, int count)
        {
            return Buttons.Skip(start).Take(count).ToList();
        }


        public virtual List<ButtonBase> ToList()
        {
            return this.Buttons.DefaultIfEmpty(new List<ButtonBase>()).Select(a => a.ToList()).Aggregate((a, b) => a.Union(b).ToList());
        }

        public virtual InlineKeyboardButton[][] ToInlineButtonArray()
        {
            var ikb = this.Buttons.Select(a => a.ToArray().Select(b => b.ToInlineButton(this)).ToArray()).ToArray();

            return ikb;
        }

        public virtual KeyboardButton[][] ToReplyButtonArray()
        {
            var ikb = this.Buttons.Select(a => a.ToArray().Select(b => b.ToKeyboardButton(this)).ToArray()).ToArray();

            return ikb;
        }

        public virtual List<ButtonRow> ToArray()
        {
            return Buttons;
        }

        public virtual int FindRowByButton(ButtonBase button)
        {
            var row = this.Buttons.FirstOrDefault(a => a.ToArray().Count(b => b == button) > 0);
            if (row == null)
                return -1;

            return this.Buttons.IndexOf(row);
        }

        public virtual Tuple<ButtonRow, int> FindRow(String text, bool useText = true)
        {
            var r = this.Buttons.FirstOrDefault(a => a.Matches(text, useText));
            if (r == null)
                return null;

            var i = this.Buttons.IndexOf(r);
            return new Tuple<ButtonRow, int>(r, i);
        }


        /// <summary>
        /// Returns the first Button with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual ButtonBase GetButtonByValue(String value)
        {
            return this.ToList().Where(a => a.Value == value).FirstOrDefault();
        }

        public static implicit operator InlineKeyboardMarkup(ButtonForm form)
        {
            if (form == null)
                return null;

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(form.ToInlineButtonArray());

            return ikm;
        }

        public static implicit operator ReplyKeyboardMarkup(ButtonForm form)
        {
            if (form == null)
                return null;

            ReplyKeyboardMarkup ikm = new ReplyKeyboardMarkup(form.ToReplyButtonArray());

            return ikm;
        }

        /// <summary>
        /// Creates a copy of this form.
        /// </summary>
        /// <returns></returns>
        public virtual ButtonForm Clone()
        {
            var bf = new ButtonForm()
            {
                Markup = this.Markup,
                DependencyControl = this.DependencyControl
            };

            foreach (var b in Buttons)
            {
                var lst = new ButtonRow();
                foreach (var b2 in b)
                {
                    lst.Add(b2);
                }
                bf.Buttons.Add(lst);
            }

            return bf;
        }

        /// <summary>
        /// Creates a copy of this form and filters by the parameter.
        /// </summary>
        /// <returns></returns>
        public virtual ButtonForm FilterDuplicate(String filter, bool ByRow = false)
        {
            var bf = new ButtonForm()
            {
                Markup = this.Markup,
                DependencyControl = this.DependencyControl
            };

            foreach (var b in Buttons)
            {
                var lst = new ButtonRow();
                foreach (var b2 in b)
                {
                    if (b2.Text.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) == -1)
                        continue;

                    //Copy full row, when at least one match has found.
                    if (ByRow)
                    {
                        lst = b;
                        break;
                    }
                    else
                    {
                        lst.Add(b2);
                    }
                }

                if (lst.Count > 0)
                    bf.Buttons.Add(lst);
            }

            return bf;
        }

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Creates a copy of this form and filters by the parameter.
        /// </summary>
        /// <returns></returns>
        public virtual ButtonForm TagDuplicate(List<String> tags, bool ByRow = false)
        {
            var bf = new ButtonForm()
            {
                Markup = this.Markup,
                DependencyControl = this.DependencyControl
            };

            foreach (var b in Buttons)
            {
                var lst = new ButtonRow();
                foreach (var b2 in b)
                {
                    if (!(b2 is TagButtonBase tb))
                        continue;

                    if (!tags.Contains(tb.Tag))
                        continue;

                    //Copy full row, when at least one match has found.
                    if (ByRow)
                    {
                        lst = b;
                        break;
                    }
                    else
                    {
                        lst.Add(b2);
                    }
                }

                if (lst.Count > 0)
                    bf.Buttons.Add(lst);
            }

            return bf;
        }
    }
}
