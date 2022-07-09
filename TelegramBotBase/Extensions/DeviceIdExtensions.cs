//
//  DeviceIdExtensions.cs
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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TelegramBotBase.Base;

namespace TelegramBotBase.Extensions
{
    public static class DeviceIdExtensions
    {
        public static Expression<Func<StateEntry, long>> ProjectDeviceIdExpression => stateEntry => stateEntry.DeviceId;
        public static IQueryable<long> ProjectDeviceId(this IQueryable<StateEntry> entries) => entries.Select(ProjectDeviceIdExpression);

        public static Expression<Func<long, bool>> IsUserIdExpression => deviceId => deviceId > 0;
        public static Expression<Func<long, bool>> IsGroupIdExpression => deviceId => deviceId < 0;

        public static bool IsUserId(long deviceId) => IsUserIdExpression.Compile()(deviceId);
        public static bool IsGroupId(long deviceId) => IsGroupIdExpression.Compile()(deviceId);

        public static Expression<Func<StateEntry, bool>> HasUserExpression => stateEntry => stateEntry.DeviceId > 0;
        public static Expression<Func<StateEntry, bool>> HasGroupExpression => stateEntry => stateEntry.DeviceId < 0;

        public static bool HasUserId(StateEntry stateEntry) => HasUserExpression.Compile()(stateEntry);
        public static bool HasGroupId(StateEntry stateEntry) => HasGroupExpression.Compile()(stateEntry);
    }
}

