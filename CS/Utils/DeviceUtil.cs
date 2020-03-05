// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Devices.Enumeration;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Security.Cryptography;
using Windows.System;

namespace DeviceInfo.Utils
{
    public static class DeviceUtil
    {
        private const string Localhost = "localhost";
        private const string DeviceUserName = "administrator";
        private const string DevicePassword = "p@ssw0rd";// based on User setting your device password. ex: I set my device password is  p@ssw0rd
        private const string DefaultProtocol = "http";
        private const string DefaultPort = "8080";
        private const string WdpRunCommandApi = "/api/iot/processmanagement/runcommand";
        private const string WdpRunCommandWithOutputApi = "/api/iot/processmanagement/runcommandwithoutput";
        private const string GuidDevInterfaceUsbDevice = "A5DCBF10-6530-11D2-901F-00C04FB951ED";
        private const string UsbDevicesSelector = "(System.Devices.InterfaceClassGuid:=\"{" + GuidDevInterfaceUsbDevice + "}\")";
        private static string errorExcpetion = "";

        public static async Task<string> getOSInfo()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/os/info";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    output += "  ComputerName: " + root.XPathSelectElement("//ComputerName").Value + "\n";
                    output += "  Language: " + root.XPathSelectElement("//Language").Value + "\n";
                    output += "  OsEdition: " + root.XPathSelectElement("//OsEdition").Value + "\n";
                    output += "  OsEditionId: " + root.XPathSelectElement("//OsEditionId").Value + "\n";
                    output += "  OsVersion: " + root.XPathSelectElement("//OsVersion").Value + "\n";
                    output += "  Platform: " + root.XPathSelectElement("//Platform").Value + "\n";
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getDeviceType()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/os/devicefamily";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    output += "  DeviceType: " + root.XPathSelectElement("//DeviceType").Value + "\n";
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getCPULoad()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/resourcemanager/systemperf";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    var AvailableAdapters = root.XPathSelectElement("//GPUData/AvailableAdapters");

                    output += "CPU usage: " + root.XPathSelectElement("//CpuLoad").Value + " %\n";
  
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getSystemPerformance()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/resourcemanager/systemperf";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    var AvailableAdapters = root.XPathSelectElement("//GPUData/AvailableAdapters");

                    output += "  AvailablePages: " + root.XPathSelectElement("//AvailablePages").Value + "\n";
                    output += "  CommitLimit: " + root.XPathSelectElement("//CommitLimit").Value + "\n";
                    output += "  CommittedPages: " + root.XPathSelectElement("//CommittedPages").Value + "\n";
                    output += "  CpuLoad: " + root.XPathSelectElement("//CpuLoad").Value + " %\n";
                    output += "  IOOtherSpeed: " + root.XPathSelectElement("//IOOtherSpeed").Value + "\n";
                    output += "  IOReadSpeed: " + root.XPathSelectElement("//IOReadSpeed").Value + "\n";
                    output += "  IOWriteSpeed: " + root.XPathSelectElement("//IOWriteSpeed").Value + "\n";
                    output += "  NonPagedPoolPages: " + root.XPathSelectElement("//NonPagedPoolPages").Value + "\n";
                    output += "  PagedPoolPages: " + root.XPathSelectElement("//PagedPoolPages").Value + "\n";
                    output += "  PageSize: " + root.XPathSelectElement("//PageSize").Value + "\n";
                    output += "  TotalPages: " + root.XPathSelectElement("//TotalPages").Value + "\n";

                    output += "\n[GPUData]\n";
                    output += "  AvailableAdapters:\n";
                    output += "    DedicatedMemory: " + AvailableAdapters.XPathSelectElement("//DedicatedMemory").Value + "\n";
                    output += "    DedicatedMemoryUsed: " + AvailableAdapters.XPathSelectElement("//DedicatedMemoryUsed").Value + "\n";
                    output += "    Description: " + AvailableAdapters.XPathSelectElement("//Description").Value + "\n";
                    output += "    SystemMemory: " + AvailableAdapters.XPathSelectElement("//SystemMemory").Value + "\n";
                    output += "    SystemMemoryUsed: " + AvailableAdapters.XPathSelectElement("//SystemMemoryUsed").Value + "\n";
                    output += "    EnginesUtilization: " + AvailableAdapters.XPathSelectElement("//EnginesUtilization").Value + "\n";

                    output += "\n[NetworkingData]\n";
                    output += "  NetworkInBytes: " + root.XPathSelectElement("//NetworkingData/NetworkInBytes").Value + "\n";
                    output += "  NetworkOutBytes: " + root.XPathSelectElement("//NetworkingData/NetworkOutBytes").Value;
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getPowerBattery()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/power/battery";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    output += "  AcOnline: " + root.XPathSelectElement("//AcOnline").Value + "\n";
                    output += "  BatteryPresent: " + root.XPathSelectElement("//BatteryPresent").Value + "\n";
                    output += "  Charging: " + root.XPathSelectElement("//Charging").Value + "\n";
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getPowerState()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/power/state";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    output += "  LowPowerStateAvailable: " + root.XPathSelectElement("//LowPowerStateAvailable").Value + "\n";
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getNetworkIpConfig()
        {
            string output = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string url = DefaultProtocol + "://" + Localhost + ":" + DefaultPort + "/api/networking/ipconfig";
                    HttpResponseMessage response = await client.GetAsync(url);

                    string res = await response.Content.ReadAsStringAsync();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res), new System.Xml.XmlDictionaryReaderQuotas());
                    var root = XElement.Load(jsonReader);

                    var adapter = root.XPathSelectElement("//Adapters");
                    output += "  Description: " + adapter.XPathSelectElement("//Description").Value + "\n";
                    output += "  HardwareAddress: " + adapter.XPathSelectElement("//HardwareAddress").Value + "\n";
                    output += "  Index: " + adapter.XPathSelectElement("//Index").Value + "\n";
                    output += "  Name: " + adapter.XPathSelectElement("//Name").Value + "\n";
                    output += "  Type: " + adapter.XPathSelectElement("//Type").Value + "\n";

                    output += "  DHCP LeaseExpires: " + adapter.XPathSelectElement("//DHCP/LeaseExpires").Value + "\n";
                    output += "  DHCP LeaseObtained: " + adapter.XPathSelectElement("//DHCP/LeaseObtained").Value + "\n";
                    output += "  DHCP Address IpAddress: " + adapter.XPathSelectElement("//DHCP/Address/IpAddress").Value + "\n";
                    output += "  DHCP Address Mask: " + adapter.XPathSelectElement("//DHCP/Address/Mask").Value + "\n";

                    output += "  Gateways IpAddress: " + adapter.XPathSelectElement("//Gateways//IpAddress").Value + "\n";
                    output += "  Gateways Mask: " + adapter.XPathSelectElement("//Gateways//Mask").Value + "\n";

                    output += "  IpAddresses IpAddress: " + adapter.XPathSelectElement("//IpAddresses//IpAddress").Value + "\n";
                    output += "  IpAddresses Mask: " + adapter.XPathSelectElement("//IpAddresses//Mask").Value + "\n";
                }
                catch (Exception e)
                {
                    output = e.Message.ToString();
                }
            }
            return output;
        }

        public static async Task<string> getNetworkStatus()
        {
            string output = "";
            var hostNamesList = NetworkInformation.GetHostNames();

            foreach (var hostName in hostNamesList)
            {
                if (hostName.Type != HostNameType.Ipv4 && hostName.Type != HostNameType.Ipv6)
                {
                    continue;
                }

                var adapter2 = hostName?.IPInformation?.NetworkAdapter;
                if (adapter2 == null)
                {
                    continue;
                }

                var profile = await adapter2.GetConnectedProfileAsync();
                if (profile == null)
                {
                    continue;
                }

                NetworkConnectivityLevel statusTag = profile.GetNetworkConnectivityLevel();
                switch (statusTag)
                {
                    case NetworkConnectivityLevel.None:
                        output += "  Network status: 沒有連線能力";
                        break;
                    case NetworkConnectivityLevel.LocalAccess:
                        output += "  Network status: 僅限本機網路存取";
                        break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess:
                        output += "  Network status: 受限的網際網路存取";
                        break;
                    case NetworkConnectivityLevel.InternetAccess:
                        output += "  Network status: 本機和網際網路存取";
                        break;
                }

            }
            return output;
        }

        public static async Task<string> getUSBconnectedDevice()
        {
            string output = "";
            var deviceInformationCollection = await DeviceInformation.FindAllAsync(UsbDevicesSelector);
            if (deviceInformationCollection == null || deviceInformationCollection.Count == 0)
            {
                output += "  No Devices!!";
            }
            else
            {
                // If devices are found, enumerate them and add only enabled ones
                foreach (var device in deviceInformationCollection)
                {
                    if (device.IsEnabled)
                    {
                        output += "  " + device.Name + "\n";
                    }
                }
            }
            return output;
        }

        public static async Task<string> getCpuInfo()
        {
            return await getResultOutputAsync("wmic cpu list brief");
        }

        public static async Task<string> getPhysicalDiskInfo()
        {
            return await getResultOutputAsync("wmic diskdrive list brief");
        }

        public static async Task<string> getDiskDriveInfo()
        {
            return await getResultOutputAsync("wmic logicaldisk get caption,VolumeName");
        }

        public static async Task<string> getMemoryInfo()
        {
            return await getResultOutputAsync("wmic MEMORYCHIP get BankLabel, DeviceLocator, MemoryType, TypeDetail, Capacity, Speed");
        }

        private static async Task<string> getResultOutputAsync(string command)
        {
            string outputText = string.Empty;
            bool isError = false;
            var response = await ExecuteCommandUsingRESTApi(Localhost, DeviceUserName, DevicePassword, command);
            if (response == null)
            {
                return errorExcpetion;
            }

            if (response.IsSuccessStatusCode)
            {
                JsonObject jsonOutput = null;
                if (JsonObject.TryParse(await response.Content.ReadAsStringAsync(), out jsonOutput))
                {
                    if (jsonOutput.ContainsKey("output"))
                    {
                        outputText = jsonOutput["output"].GetString();
                    }
                    else
                    {
                        isError = true;
                        outputText = "CouldNotParseOutputFailure";
                    }
                }
                else
                {
                    isError = true;
                    outputText = "CouldNotParseOutputFailure";
                }
            }
            return outputText;
        }

        private static async Task<HttpResponseMessage> ExecuteCommandUsingRESTApi(string ipAddress, string username, string password, string runCommand, bool isOutputRequired = true)
        {
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var command = CryptographicBuffer.ConvertStringToBinary(runCommand, BinaryStringEncoding.Utf8);
                    var runAsDefaultAccountFalse = CryptographicBuffer.ConvertStringToBinary("false", BinaryStringEncoding.Utf8);
                    var timeout = CryptographicBuffer.ConvertStringToBinary(string.Format("{0}", TimeSpan.FromSeconds(15).TotalMilliseconds), BinaryStringEncoding.Utf8);

                    var urlContent = new Windows.Web.Http.HttpFormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string,string>("command", CryptographicBuffer.EncodeToBase64String(command)),
                    new KeyValuePair<string,string>("runasdefaultaccount", CryptographicBuffer.EncodeToBase64String(runAsDefaultAccountFalse)),
                    new KeyValuePair<string,string>("timeout", CryptographicBuffer.EncodeToBase64String(timeout)),
                });

                    var wdpCommand = isOutputRequired ? WdpRunCommandWithOutputApi : WdpRunCommandApi;
                    var uriString = string.Format("{0}://{1}:{2}{3}?{4}", DefaultProtocol, ipAddress, DefaultPort, wdpCommand, await urlContent.ReadAsStringAsync());
                    var uri = new Uri(uriString);

                    var byteArray = Encoding.ASCII.GetBytes(DeviceUserName + ":" + DevicePassword);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    response = await client.PostAsync(uri, null);
                }
                 catch  (Exception e)
                {
                    errorExcpetion = e.Message.ToString();
                }
            }
            return response;
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            string packageName = package.DisplayName;
            PackageVersion version = package.Id.Version;

            return string.Format("{0} {1}.{2}.{3}.{4}", packageName, version.Major, version.Minor, version.Build, version.Revision);
        }

        public static string GetOSVersionString()
        {
            if (!ulong.TryParse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion, out ulong version))
            {
                return "OSVersionNotAvailable";
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}",
                    (version & 0xFFFF000000000000) >> 48,
                    (version & 0x0000FFFF00000000) >> 32,
                    (version & 0x00000000FFFF0000) >> 16,
                    (version & 0x000000000000FFFF));
            }
        }

        public static void Shutdown(bool restart = false)
        {
            ShutdownManager.BeginShutdown(restart ? ShutdownKind.Restart : ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
        }
    }
}
