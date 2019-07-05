using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace VSIXProject1.Settings.Nuget
{
    public class NugetUpdate
    {

        const string INITIALVERSION = "1.0.0";
        NugetSetting _nugetSetting;
        string _currentVersion = null;
        public void Execute(NugetSetting nugetSetting)
        {
            _nugetSetting = nugetSetting;
            if (nugetSetting.IncrementarVersion)
            {
                IncrementVersion();
            }

            CreatePackage();
            UpdatePackageInNuget();


        }
        void IncrementVersion()
        {
            XElement xElement = XElement.Load(_nugetSetting.Package);
            XElement propertyGroupElement = xElement.Element("PropertyGroup");
            if (propertyGroupElement.HasElements)
            {
                var elementVersion = propertyGroupElement.Elements("Version").FirstOrDefault();
                if (elementVersion != null)
                {
                    Version version = new Version(elementVersion.Value);
                    string newValue = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build + 1);
                    elementVersion.Value = newValue;
                    _currentVersion = newValue;
                }
                else
                {
                    XElement elementVersionNew = new XElement("Version", INITIALVERSION);
                    propertyGroupElement.Add(elementVersionNew);
                    _currentVersion = INITIALVERSION;
                }
                xElement.Save(_nugetSetting.Package);

            }
        }

        void CreatePackage()
        {
            StringBuilder command = new StringBuilder();
            command.Append("pack ");
            command.Append(_nugetSetting.Package);
            command.Append(" -c Release ");

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.ErrorDataReceived += Process_ErrorDataReceived1;
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "dotnet.exe";
            startInfo.Arguments = command.ToString();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception("No se puedo crear el paquete.");
            }
        }

        private void Process_ErrorDataReceived1(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            throw new Exception(e.Data);
        }

        void UpdatePackageInNuget()
        {
            FileInfo file = new FileInfo(_nugetSetting.Package);
            FileInfo nugetFile = new FileInfo(Path.Combine(file.Directory.FullName, "bin/Release", Path.GetFileNameWithoutExtension(file.Name) + "." + _currentVersion + ".nupkg"));

            StringBuilder command = new StringBuilder();
            command.Append("nuget push -s ");
            command.Append(_nugetSetting.Server);
            if (!string.IsNullOrWhiteSpace(_nugetSetting.ApiKey))
            {
                command.Append(" -k ");
                command.Append(_nugetSetting.ApiKey);

            }

            command.Append(" ");
            command.Append(nugetFile);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.ErrorDataReceived += Process_ErrorDataReceived;
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "dotnet.exe";
            startInfo.Arguments = command.ToString();
            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception("Hubo un error al querer subir el paquete al servidor de Nuget.");
            }


        }

        private void Process_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            throw new Exception(e.Data);
        }
    }
}
