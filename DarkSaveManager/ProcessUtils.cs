﻿using System.Diagnostics;

namespace DarkSaveManager;

internal static partial class Utils
{
    /*
    @NET5(ProcessUtils):
    These wrappers that set UseShellExecute to true are just here for compatibility with .NET Core 3 and
    above:
    In Framework, UseShellExecute defaults to true, but in Core 3 and above, it defaults to false (it's
    something to do with cross-platform concerns). We just want to keep it true to keep behavior the same
    and I think sometimes we want it true because there are behavioral differences and some things only
    work with it true or false. I can't remember the details at the moment but yeah.
    */

    /// <inheritdoc cref="Process.Start(string)"/>
    internal static void ProcessStart_UseShellExecute(string fileName)
    {
        using (Process.Start(new ProcessStartInfo { FileName = fileName, UseShellExecute = true }))
        {
        }
    }

    /// <summary>
    /// Starts the process resource that is specified by the parameter containing process start information (for example, the file name of the process to start) and associates the resource with a new <see cref="T:System.Diagnostics.Process" /> component.
    /// <para>
    /// *Use this for Framework and Core compatibility: Core has UseShellExecute off by default (but we want it on).
    /// </para>
    /// </summary>
    /// <param name="startInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
    /// <param name="overrideUseShellExecuteToOn">Force UseShellExecute to be <see langword="true"/></param>
    /// <exception cref="T:System.InvalidOperationException">No file name was specified in the <paramref name="startInfo" /> parameter's <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property.
    /// -or-
    /// The <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property of the <paramref name="startInfo" /> parameter is <see langword="true" /> and the <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardInput" />, <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardOutput" />, or <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardError" /> property is also <see langword="true" />.
    /// -or-
    /// The <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property of the <paramref name="startInfo" /> parameter is <see langword="true" /> and the <see cref="P:System.Diagnostics.ProcessStartInfo.UserName" /> property is not <see langword="null" /> or empty or the <see cref="P:System.Diagnostics.ProcessStartInfo.Password" /> property is not <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="startInfo" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
    /// <exception cref="T:System.IO.FileNotFoundException">The file specified in the <paramref name="startInfo" /> parameter's <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property could not be found.</exception>
    /// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when opening the associated file.
    /// -or-
    /// The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: "The data area passed to a system call is too small." or "Access is denied."</exception>
    /// <exception cref="T:System.PlatformNotSupportedException">Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
    internal static void ProcessStart_UseShellExecute(ProcessStartInfo startInfo, bool overrideUseShellExecuteToOn = true)
    {
        if (overrideUseShellExecuteToOn) startInfo.UseShellExecute = true;
        using (Process.Start(startInfo)) { }
    }
}
