using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelegramBotBase.Extensions;
using static TelegramBotBase.Extensions.DeviceIdExtensions;
using TelegramBotBase.Interfaces;

namespace TelegramBotBase.Base
{
    public partial class StateContainer : IStateContainer
    {
        public virtual IQueryable<StateEntry> States { get; set; } = new List<StateEntry>().AsQueryable();

        public virtual IQueryable<long> ChatIds => States.Where(HasUserExpression).Select(ProjectDeviceIdExpression);

        public virtual IQueryable<long> GroupIds => States.Where(HasGroupExpression).Select(ProjectDeviceIdExpression);

        public virtual IQueryable<long> DeviceIds => States.ProjectDeviceId();

        public StateContainer() : this(Array.Empty<StateEntry>()) { }
        public StateContainer(ICollection<StateEntry> states) : this(states.AsQueryable()) { }
        public StateContainer(IQueryable<StateEntry> states) => States = states;
    }
}
