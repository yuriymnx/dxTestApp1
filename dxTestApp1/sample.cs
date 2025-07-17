using System.Xml.Linq;
using System.Text;

class Program
{
    const string InputFile = "merged_resources.xml";
    const string OutputDir = "GeneratedProjects";
    const string TargetFramework = "net40";
    const string Version = "22.2.0.0";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (!File.Exists(InputFile))
        {
            Console.WriteLine($"Файл не найден: {InputFile}");
            return;
        }

        Console.WriteLine("Загрузка XML...");
        var doc = XDocument.Load(InputFile);

        var projectElements = doc.Root?.Elements("project");
        if (projectElements == null)
        {
            Console.WriteLine("Не найдены <project> элементы");
            return;
        }

        Directory.CreateDirectory(OutputDir);

        foreach (var projectEl in projectElements)
        {
            var fullProjectName = projectEl.Attribute("name")?.Value;
            if (string.IsNullOrWhiteSpace(fullProjectName)) continue;

            // Выделяем "чистое" имя проекта — без версии и .resources
            var shortProjectName = ExtractShortProjectName(fullProjectName);

            var projectFolder = Path.Combine(OutputDir, fullProjectName);
            Directory.CreateDirectory(projectFolder);

            Console.WriteLine($"Генерация: {fullProjectName}");

            foreach (var resxEl in projectEl.Elements("resx"))
            {
                var resxName = resxEl.Attribute("name")?.Value ?? "Resources.resx";
                var safeResxName = Path.GetFileName(resxName);
                var resxPath = Path.Combine(projectFolder, safeResxName);

                var resxDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("root", resxEl.Elements("data"))
                );

                resxDoc.Save(resxPath);

                // AssemblyInfo.cs
                File.WriteAllText(Path.Combine(projectFolder, "AssemblyInfo.cs"), $@"
using System.Reflection;

[assembly: AssemblyTitle(""{shortProjectName}"")]
[assembly: AssemblyDescription(null)]
[assembly: AssemblyCompany(""Developer Express Inc."")]
[assembly: AssemblyProduct(""{shortProjectName}"")]
[assembly: AssemblyCopyright(""Copyright © 2000-2022 Developer Express Inc."")]
[assembly: AssemblyTrademark(null)]
[assembly: AssemblyVersion(""{Version}"")]
");

                // .csproj
                File.WriteAllText(Path.Combine(projectFolder, $"{fullProjectName}.csproj"), $@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <AssemblyName>{shortProjectName}</AssemblyName>
    <GenerateAssemblyInfo>False</GenerateAssemblyInfo>
    <TargetFramework>{TargetFramework}</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <EmbeddedResource Include=""{safeResxName}"" />
    <Compile Include=""AssemblyInfo.cs"" />
  </ItemGroup>
</Project>
");
            }
        }

        Console.WriteLine("Готово.");
    }

    static string ExtractShortProjectName(string fullName)
    {
        var name = fullName;
        
        if (name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - ".resources".Length);
        
        var versionIndex = name.IndexOf(".v", StringComparison.OrdinalIgnoreCase);
        if (versionIndex >= 0)
        {
            var prefix = name.Substring(0, versionIndex);
            var suffixStart = name.IndexOf('.', versionIndex + 2); // после .v22.2
            if (suffixStart > 0)
                name = prefix + name.Substring(suffixStart);
            else
                name = prefix;
        }

        return name;
    }
}
