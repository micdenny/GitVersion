using GitVersion.Core.Tests.Helpers;
using GitVersion.Helpers;
using GitVersion.Logging;
using GitVersion.VersionCalculation;
using GitVersion.VersionConverters.WixUpdater;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace GitVersion.Core.Tests;

[TestFixture]
[Parallelizable(ParallelScope.None)]
internal class WixFileTests : TestBase
{
    [SetUp]
    public void Setup() => ShouldlyConfiguration.ShouldMatchApprovedDefaults.LocateTestMethodUsingAttribute<TestAttribute>();

    [Test]
    public void UpdateWixVersionFile()
    {
        var workingDir = Path.GetTempPath();
        var semVer = new SemanticVersion
        {
            Major = 1,
            Minor = 2,
            Patch = 3,
            BuildMetaData = "5.Branch.develop"
        };

        semVer.BuildMetaData.VersionSourceSha = "versionSourceSha";
        semVer.BuildMetaData.Sha = "commitSha";
        semVer.BuildMetaData.ShortSha = "commitShortSha";
        semVer.BuildMetaData.CommitDate = DateTimeOffset.Parse("2019-02-20 23:59:59Z");

        var config = new TestEffectiveConfiguration();

        var stringBuilder = new StringBuilder();
        void Action(string s) => stringBuilder.AppendLine(s);

        var logAppender = new TestLogAppender(Action);
        var log = new Log(logAppender);

        var sp = ConfigureServices(service => service.AddSingleton<ILog>(log));

        var fileSystem = sp.GetRequiredService<IFileSystem>();
        var variableProvider = sp.GetRequiredService<IVariableProvider>();
        var versionVariables = variableProvider.GetVariablesFor(semVer, config, false);

        using var wixVersionFileUpdater = sp.GetRequiredService<IWixVersionFileUpdater>();

        wixVersionFileUpdater.Execute(versionVariables, new WixVersionContext(workingDir));

        var file = PathHelper.Combine(workingDir, WixVersionFileUpdater.WixVersionFileName);
        fileSystem
            .ReadAllText(file)
            .ShouldMatchApproved(c => c.SubFolder(PathHelper.Combine("Approved")));
    }

    [Test]
    public void UpdateWixVersionFileWhenFileAlreadyExists()
    {
        var workingDir = Path.GetTempPath();
        var semVer = new SemanticVersion
        {
            Major = 1,
            Minor = 2,
            Patch = 3,
            BuildMetaData = "5.Branch.develop"
        };

        semVer.BuildMetaData.VersionSourceSha = "versionSourceSha";
        semVer.BuildMetaData.Sha = "commitSha";
        semVer.BuildMetaData.ShortSha = "commitShortSha";
        semVer.BuildMetaData.CommitDate = DateTimeOffset.Parse("2019-02-20 23:59:59Z");

        var config = new TestEffectiveConfiguration();

        var stringBuilder = new StringBuilder();
        void Action(string s) => stringBuilder.AppendLine(s);

        var logAppender = new TestLogAppender(Action);
        var log = new Log(logAppender);

        var sp = ConfigureServices(service => service.AddSingleton<ILog>(log));

        var fileSystem = sp.GetRequiredService<IFileSystem>();
        var variableProvider = sp.GetRequiredService<IVariableProvider>();
        var versionVariables = variableProvider.GetVariablesFor(semVer, config, false);

        using var wixVersionFileUpdater = sp.GetRequiredService<IWixVersionFileUpdater>();

        // fake an already existing file
        var file = PathHelper.Combine(workingDir, WixVersionFileUpdater.WixVersionFileName);
        fileSystem.WriteAllText(file, new string('x', 1024 * 1024));

        wixVersionFileUpdater.Execute(versionVariables, new WixVersionContext(workingDir));

        fileSystem
            .ReadAllText(file)
            .ShouldMatchApproved(c => c.SubFolder(PathHelper.Combine("Approved")));
    }

}
