using System.Diagnostics;

namespace Game.LevelEditor.Services;

public class PlayModeService
{
    public async Task RunLevel(string path)
    {
        Debug.Log("Starting play session");

        ProcessStartInfo info = new ProcessStartInfo(Paths.ApplicationExecutable, $"--level \"{path}\"");

        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        info.CreateNoWindow = false;

        using (Process process = new Process())
        {
            process.StartInfo = info;
            process.Start();

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = await outputTask;
            string error = await errorTask;

            if (!string.IsNullOrEmpty(output))
            {
                Debug.Log(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Debug.Log(error, LogLevel.Error);
            }

            Debug.Log("End play session");
            Debug.Log($"Exit code: {process.ExitCode}");
        }
    }
}