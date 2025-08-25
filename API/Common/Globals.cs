using GCommon;
using GDB;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;

namespace API.Common
{
    public static class Globals
    {
        public static string NameSpace = "API.Controllers";

        public static object CallFunction(this Type t, string f, params object[] p)
        {
            return t.GetMethod(f).Invoke(null, p);
        }

        public static object CallFunctionByName(this string name, params object[] p)
        {
            List<string> callBack = name.Split('.').ToList();
            string fnc = callBack[callBack.Count - 1];
            callBack.Remove(fnc);
            Type t = Type.GetType(Globals.NameSpace + "." + string.Join(".", callBack));
            return t.CallFunction(fnc, p);
        }

        public static List<Type> GetAllClass(string nsp = "")
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(c => (c.Namespace + "").IndexOf(nsp!=""?nsp:Globals.NameSpace) >= 0).ToList();
        }

        public static string GetMimeTypeByWindowsRegistry(this string fileNameOrExtension)
        {
            string mimeType = "application/unknown";
            string ext = (fileNameOrExtension.Contains(".")) ? System.IO.Path.GetExtension(fileNameOrExtension).ToLower() : "." + fileNameOrExtension;
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null) mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
