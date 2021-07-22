using System.Text;
using GitVersion.Extensions;

namespace GitVersion.App.Tests
{
    public class ArgumentBuilder
    {
        public ArgumentBuilder(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;
        }

        public ArgumentBuilder(string workingDirectory, string exec, string execArgs, string projectFile, string projectArgs, string logFile)
        {
            this.workingDirectory = workingDirectory;
            this.exec = exec;
            this.execArgs = execArgs;
            this.projectFile = projectFile;
            this.projectArgs = projectArgs;
            this.logFile = logFile;
        }

        public ArgumentBuilder(string workingDirectory, string additionalArguments, string logFile)
        {
            this.workingDirectory = workingDirectory;
            this.additionalArguments = additionalArguments;
            this.logFile = logFile;
        }

        public string WorkingDirectory => workingDirectory;

        public string LogFile => logFile;

        public override string ToString()
        {
            var arguments = new StringBuilder();

            arguments.AppendFormat(" /targetpath \"{0}\"", workingDirectory);

            if (!exec.IsNullOrWhiteSpace())
            {
                arguments.AppendFormat(" /exec \"{0}\"", exec);
            }

            if (!execArgs.IsNullOrWhiteSpace())
            {
                arguments.AppendFormat(" /execArgs \"{0}\"", execArgs);
            }

            if (!projectFile.IsNullOrWhiteSpace())
            {
                arguments.AppendFormat(" /proj \"{0}\"", projectFile);
            }

            if (!projectArgs.IsNullOrWhiteSpace())
            {
                arguments.AppendFormat(" /projargs \"{0}\"", projectArgs);
            }

            if (!logFile.IsNullOrWhiteSpace())
            {
                arguments.AppendFormat(" /l \"{0}\"", logFile);
            }

            arguments.Append(additionalArguments);

            return arguments.ToString();
        }

        private readonly string additionalArguments;
        private readonly string exec;
        private readonly string execArgs;
        private readonly string logFile;
        private readonly string projectArgs;
        private readonly string projectFile;
        private readonly string workingDirectory;
    }
}
