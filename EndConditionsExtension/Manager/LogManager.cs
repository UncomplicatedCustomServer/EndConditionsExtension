/*
 * This file is a part of the UncomplicatedCustomRoles project.
 *
 * Copyright (c) 2023-present FoxWorn3365 (Federico Cosma) <me@fcosma.it>
 *
 * This file is licensed under the GNU Affero General Public License v3.0.
 * You should have received a copy of the AGPL license along with this file.
 * If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using LabApi.Features.Console;

namespace EndConditionsExtension.Manager;

internal class LogManager
{
    private static bool DebugEnabled => Plugin.Singleton.Config.Debug;

    public static void Debug(string message)
    {
        if (!DebugEnabled)
            return;
        Logger.Debug(message);
    }

    public static void SmInfo(string message, string label = "Info")
    {
        Logger.Raw($"[{label}] [{Plugin.Singleton.Name}] {message}", ConsoleColor.Gray);
    }

    public static void Info(string message, ConsoleColor color = ConsoleColor.Cyan)
    {
        Logger.Raw($"[INFO] [{Plugin.Singleton.Name}] {message}", color);
    }

    public static void Warn(string message)
    {
        Logger.Warn(message);
    }

    public static void Error(string message)
    {
        Logger.Error(message);
    }
}