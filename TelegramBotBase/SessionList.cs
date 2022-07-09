//
//  SessionList.cs
//
//  Author:
//       Justin Chase <dev@thebackroom.app>
//
//  Copyright (c) 2022 Justin Chase
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using TelegramBotBase.Args;
using TelegramBotBase.Attributes;
using TelegramBotBase.Base;
using TelegramBotBase.Form;
using TelegramBotBase.Interfaces;
using TelegramBotBase.Sessions;

namespace TelegramBotBase
{
    public class SessionList : IDictionary<long, DeviceSession>
    {
        private IQueryable<StateEntry> _states;
        private IStateMachine _stateMachine;
        private MessageClient _messageClient;
        private IDictionary<long, DeviceSession> _cache;
        private IStateContainer _stateContainer;

        public SessionList(IStateMachine stateMachine, MessageClient messageClient)
        {
            _stateMachine = stateMachine;
            _stateContainer = stateMachine.LoadFormStates();
            _states = _stateContainer.States;
            _messageClient = messageClient;
            _cache = new Dictionary<long, DeviceSession>();
        }

        protected virtual async Task<DeviceSession> HydrateSessionAsync(StateEntry entry)
        {
            Type t = Type.GetType(entry.QualifiedName);
            if (t == null || !t.IsSubclassOf(typeof(FormBase)))
            {
                return default;
            }

            //Key already existing
            if (_cache.ContainsKey(entry.DeviceId))
                return _cache[entry.DeviceId];

            var form = t.GetConstructor(new Type[] { })?.Invoke(new object[] { }) as FormBase;

            //No default constructor, fallback
            if (form == null)
            {
                if (!_stateMachine.FallbackStateForm.IsSubclassOf(typeof(FormBase)))
                    return default;

                form = _stateMachine.FallbackStateForm.GetConstructor(new Type[] { })?.Invoke(new object[] { }) as FormBase;

                //Fallback failed, due missing default constructor
                if (form == null)
                    return default;
            }


            if (entry.Values != null && entry.Values.Count > 0)
            {
                var properties = entry.Values.Where(a => a.Key.StartsWith("$"));
                var fields = form.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).Where(a => a.GetCustomAttributes(typeof(SaveState), true).Length != 0).ToList();

                foreach (var p in properties)
                {
                    var f = fields.FirstOrDefault(a => a.Name == p.Key.Substring(1));
                    if (f == null)
                        continue;

                    try
                    {
                        if (f.PropertyType.IsEnum)
                        {
                            var ent = Enum.Parse(f.PropertyType, p.Value.ToString());

                            f.SetValue(form, ent);

                            continue;
                        }


                        f.SetValue(form, p.Value);
                    }
                    catch (ArgumentException)
                    {
                        Tools.Conversion.CustomConversionChecks(form, p, f);
                    }
                    catch
                    {

                    }
                }
            }

            form.Client = _messageClient;
            var device = new DeviceSession(entry.DeviceId, form);

            device.ChatTitle = entry.ChatTitle;

            //this.SessionList.Add(entry.DeviceId, device);

            //Is Subclass of IStateForm
            if (form is IStateForm iform)
            {
                var ls = new LoadStateEventArgs();
                ls.Values = entry.Values;
                iform.LoadState(ls);
            }

            try
            {
                await form.OnInit(new InitEventArgs());

                await form.OnOpened(new EventArgs());
            }
            catch
            {
                //Skip on exception
                //this.SessionList.Remove(s.DeviceId);
            }

            return device;
        }

        public virtual IDictionary<long, DeviceSession> HydratedSessions => _cache;

        public DeviceSession this[long key]
        {
            get => _cache[key] = HydrateSessionAsync(_states.FirstOrDefault(state => state.DeviceId == key)).Result;
            set => _cache[key] = value;
        }

        public ICollection<long> Keys => KeysQueryable.ToList();

        public ICollection<DeviceSession> Values => ValuesQueryable.ToList();

        public IQueryable<long> KeysQueryable => _stateContainer.DeviceIds;

        public IQueryable<DeviceSession> ValuesQueryable => KeysQueryable.Select(id => this[id]);

        public int Count => _states.Count();

        public bool IsReadOnly => false;

        public void Add(long key, DeviceSession value) => this[key] = value;

        public void Add(KeyValuePair<long, DeviceSession> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            _cache.Clear();
            _stateContainer = new StateContainer();
            _states = new StateEntry[0].AsQueryable();
        }

        public bool Contains(KeyValuePair<long, DeviceSession> item) => ContainsKey(item.Key);

        public bool ContainsKey(long key) => Keys.Contains(key);

        public void CopyTo(KeyValuePair<long, DeviceSession>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<long, DeviceSession>> GetEnumerator()
            => Keys.Select<long, KeyValuePair<long, DeviceSession>>(key => new(key, this[key])).GetEnumerator();

        public bool Remove(long key)
        {
            _cache.Remove(key);
            _states = _states.Except(new[] { _states.FirstOrDefault(state => state.DeviceId == key) });
            _stateContainer.States = _stateContainer.States.Except(new[] { _stateContainer.States.FirstOrDefault(state => state.DeviceId == key) });
            return true;
        }

        public bool Remove(KeyValuePair<long, DeviceSession> item) => Remove(item.Key);

        public bool TryGetValue(long key, out DeviceSession value)
            => (value = ContainsKey(key) ? this[key] : default) != default;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

