REM nuget pack Transformalize.Transform.Aws.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Transform.Aws.Autofac.nuspec -OutputDirectory "c:\temp\modules"

nuget pack Transformalize.Provider.Aws.CloudWatch.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Aws.CloudWatch.Autofac.nuspec -OutputDirectory "c:\temp\modules"

nuget pack Transformalize.Provider.Amazon.Connect.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Amazon.Connect.Autofac.nuspec -OutputDirectory "c:\temp\modules"


