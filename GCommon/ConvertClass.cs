using System.ComponentModel;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Numerics;
using Microsoft.AspNetCore.Components.Forms;

namespace GCommon
{
    public static class ConvertClass
    {
        public static string ToString(object obj, string nullValue)
        {
            try
            {
                return obj != null ? obj.ToString() : nullValue;
            }
            catch
            {
                return nullValue;
            }
        }
        public static string ToString(object obj)
        {
            if (obj == null) return null;
            return ToString(obj, "");
        }
        public static decimal ToDecimal(object obj, decimal nullValue)
        {
            try
            {
                if ((obj + "").IndexOf("E") >= 0)
                {
                    return obj + "" == "" ? nullValue : Decimal.Parse(obj + "", System.Globalization.NumberStyles.Any);
                }
                else
                {
                    return obj != null && obj != DBNull.Value ? Convert.ToDecimal(obj) : nullValue;
                }
            }
            catch
            {
                return nullValue;
            }
        }
        public static decimal? ToDecimal(object obj)
        {
            if (obj == null) return null;
            return ToDecimal(obj, 0);
        }
        public static double ToDouble(object obj, double nullValue)
        {
            try
            {
                return obj != null && obj != DBNull.Value ? Convert.ToDouble(obj) : nullValue;
            }
            catch
            {
                return nullValue;
            }
        }
        public static double? ToDouble(object obj)
        {
            if (obj == null) return null;
            return ToDouble(obj, 0);
        }
        public static long ToLong(object obj, long nullValue)
        {
            try
            {
                return obj != null && obj != DBNull.Value ? Convert.ToInt64(obj) : nullValue;
            }
            catch
            {
                return nullValue;
            }
        }
        public static long? ToLong(object obj)
        {
            if (obj == null) return null;
            return ToLong(obj, 0);
        }
        public static byte ToByte(object obj, byte nullValue)
        {
            try
            {
                return obj != null && obj != DBNull.Value ? Convert.ToByte(obj) : nullValue;
            }
            catch
            {
                return nullValue;
            }
        }
        public static double? ToByte(object obj)
        {
            if (obj == null) return null;
            return ToByte(obj, 0);
        }
        public static bool ToBoolean(object obj, bool nullValue)
        {
            try
            {
                return obj != null && obj != DBNull.Value ? Convert.ToBoolean(obj) : nullValue;
            }
            catch
            {
                return nullValue;
            }
        }
        public static bool? ToBoolean(object obj)
        {
            if (obj == null) return null;
            return ToBoolean(obj, false);
        }
        public static int ToInt(object obj, int nullValue)
        {
            try
            {
                return obj != null ? Convert.ToInt32(obj) : nullValue;
            }
            catch
            {
                return nullValue;
            }
        }
        public static int? ToInt(object obj)
        {
            if (obj == null) return null;
            return ToInt(obj, 0);
        }
        public static DateTime ToDateTime(object obj, DateTime nullValue)
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo()
            {
                ShortDatePattern = "dd/MM/yyyy",
                DateSeparator = "/"
            };
            return obj != null ? Convert.ToDateTime(obj, dtfi) : nullValue;
        }
        public static DateTime? ToDateTime(object obj)
        {
            if (obj == null) return null;
            return ToDateTime(obj, DateTime.Now);
        }

        public static DateTime ExcelToDateTime(string strDate)
        {
            double excelDate;
            try
            {
                excelDate = Convert.ToDouble(strDate.Trim());
            }
            catch
            {
                excelDate = 0;
            }
            if (excelDate <= 0)
            {
                DateTime dtmOut;
                //DateTime.TryParse(Convert.ToString(strDate), out dtmOut);
                DateTime date;
                bool success = DateTime.TryParseExact(Convert.ToString(strDate), "dd/MM/yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None, out dtmOut);
                if (!success)
                {
                    success = DateTime.TryParseExact(Convert.ToString(strDate), "dd/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtmOut);
                    if (!success)
                    {
                        DateTime.TryParse(Convert.ToString(strDate), out dtmOut);
                    }
                }
                return dtmOut;
            }
            else
            {
                DateTime dateOfReference = new DateTime(1900, 1, 1);
                if (excelDate > 60d)
                {
                    excelDate = excelDate - 2;
                }
                else
                {
                    excelDate = excelDate - 1;
                }
                return dateOfReference.AddDays(excelDate);
            }
        }

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        private static string RemoveSign4VietnameseString(this string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        public static string CleanUrl(this string url, bool slash = false)
        {
            url = url.RemoveSign4VietnameseString();
            url = url.Replace(" ", "-").ToLower();
            if (slash) url = url.Replace("/", "-");
            url = url.Replace("--", "-");
            url = url.Replace("--", "-");
            url = url.Replace("--", "-");
            url = Regex.Replace(url, @"[^-0-9a-zA-Z_/]+", "");
            return url.ToLower();
        }

        public static string CleanFileName(this string url)
        {
            return url.CleanUrl(true).Replace("-", "_");
        }

        public static string CleanKey(this string str)
        {
            str = str.RemoveSign4VietnameseString();
            str = Regex.Replace(str, @"[^-0-9a-zA-Z/]+", "");
            return str.ToLower();
        }

        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string EncodePassword(this string password)
        {
            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
            byte[] hashBytes = encoding.GetBytes(password);

            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            byte[] cryptPassword = sha1.ComputeHash(hashBytes);

            return BitConverter.ToString(cryptPassword);
        }

        public static string HasData(this string str)
        {
            byte[] hashBytes = System.Text.Encoding.Default.GetBytes(str);
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] cryptPassword = sha1.ComputeHash(hashBytes);
            return BitConverter.ToString(cryptPassword);
        }
        public static string HasData(this byte[] hashBytes)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] cryptPassword = sha1.ComputeHash(hashBytes);
            return BitConverter.ToString(cryptPassword);
        }

        public static string StripHTML(this string input)
        {
            input = Regex.Replace(input, "<.*?>", "").TrimEnd('\r', '\n');
            return input;
        }

        public static void SetValueByName(this object item, string name, object value)
        {
            var po = item.GetType().GetProperty(name);
            if (po == null) return;
            try
            {
                var t = Nullable.GetUnderlyingType(po.PropertyType) ?? po.PropertyType;
                po.SetValue(item, value.PropertyGetValue(t), null);
                return;
            }
            catch { return; }
        }

        public static object GetValueByName(this object item, string name)
        {
            var po = item.GetType().GetProperty(name);
            if (po == null) return null;
            try
            {
                var t = Nullable.GetUnderlyingType(po.PropertyType) ?? po.PropertyType;
                return po.GetValue(item, null).PropertyGetValue(t);
            }
            catch { return null; }
        }

        public static T GetValueByName<T>(this object item, string name)
        {
            var po = item.GetType().GetProperty(name);
            if (po == null) return default(T);
            try
            {
                var t = Nullable.GetUnderlyingType(po.PropertyType) ?? po.PropertyType;
                return (T)po.GetValue(item, null).PropertyGetValue(t);
            }
            catch { return default(T); }
        }

        public static object PropertyGetValue(this object val, Type type)
        {
            if (val == null) return null;
            object reVal = null;
            if (type == typeof(string))
            {
                return ToString(val, "");
            }
            else if (type == typeof(int))
            {
                return ToInt(val, 0);
            }
            else if (type == typeof(long))
            {
                return ToLong(val, 0);
            }
            else if (type == typeof(decimal))
            {
                return ToDecimal(val, 0);
            }
            else if (type == typeof(DateTime))
            {
                return ToDateTime(val, DateTime.Now);
            }
            else if (type == typeof(Boolean))
            {
                if (val + "" == "1") return true;
                return ToBoolean(val, false);
            }
            return null;
        }
        //public static T PropertyGetValue<T>(this object val)
        //{
        //    if (val == null) return default(T);
        //    if (typeof(T) == typeof(string))
        //    {
        //        return (T)(object)ToString(val, "");
        //    }
        //    else if (typeof(T) == typeof(int))
        //    {
        //        return (T)(object)ToInt(val, 0);
        //    }
        //    else if (typeof(T) == typeof(decimal))
        //    {
        //        return (T)(object)ToDecimal(val, 0);
        //    }
        //    else if (typeof(T) == typeof(DateTime))
        //    {
        //        return (T)(object)ToDateTime(val, DateTime.Now);
        //    }
        //    else if (typeof(T) == typeof(Boolean))
        //    {
        //        if (val + "" == "1") return (T)(object)true;
        //        return (T)(object)ToBoolean(val, false);
        //    }
        //    return default(T);
        //}

        public static T Clone<T>(this T item)
        {
            if (item == null) return item;
            return JObject.FromObject(item).ToObject<T>();
            //var strItem = JsonConvert.SerializeObject(item);
            //return JsonConvert.DeserializeObject<T>(strItem);
        }

        public static T DeepClone<T>(this T source)
        {
            //if (!typeof(T).IsSerializable)
            //{
            //    throw new ArgumentException("The type must be serializable.", "source");
            //}

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            using (Stream stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream, Encoding.UTF8, false);
                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                js.WriteObject(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)js.ReadObject(stream);
            }
        }

        public static string GetLastDayOfMonth(int nam, int thang)
        {
            DateTime tg = new DateTime(nam, thang, DateTime.DaysInMonth(nam, thang));
            return tg.ToString("dd/MM/yyyy");
        }
        public static string GetFirstDayOfMonth(int nam, int thang)
        {
            DateTime tg = new DateTime(nam, thang, 1);
            return tg.ToString("dd/MM/yyyy");
        }

        public static DateTime GetFirstDayOfMonth(this DateTime ngay)
        {
            return new DateTime(ngay.Year, ngay.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(this DateTime ngay)
        {
            return new DateTime(ngay.Year, ngay.Month, DateTime.DaysInMonth(ngay.Year, ngay.Month));
        }

        public static string Base64Encode(this string plainText, bool zip = true)
        {
            var plainTextBytes = zip ? Zip(plainText) : Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData, bool zip = true)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return zip ? Unzip(base64EncodedBytes) : Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static object GetValueByPath(this JObject item, params string[] pars)
        {
            if (pars.Length == 0) return null;
            if (pars.Length == 1) return item[pars[0]];
            if (item[pars[0]] == null) return null;
            if (item[pars[0]] + "" == "") return null;
            var check = (JObject)item[pars[0]];
            return ((JObject)item[pars[0]]).GetValueByPath(pars.Skip(1).ToArray());
        }

        private static Random random = new Random();
        public static string RandomString(int length, bool isNumber = false)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            if (isNumber) chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static string ChuyenSo(this decimal so)
        {
            string number = so.ToString();
            string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
            string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string doc;
            int i, j, k, n, len, found, ddv, rd;
            string am = "";
            if (number.Substring(0, 1) == "-")
            {
                am = "Âm ";
                number = number.Substring(1);
            }
            len = number.Length;
            number += "ss";
            doc = "";
            found = 0;
            ddv = 0;
            rd = 0;

            i = 0;
            while (i < len)
            {
                //So chu so o hang dang duyet
                n = (len - i + 2) % 3 + 1;

                //Kiem tra so 0
                found = 0;
                for (j = 0; j < n; j++)
                {
                    if (number[i + j] != '0')
                    {
                        found = 1;
                        break;
                    }
                }

                //Duyet n chu so
                if (found == 1)
                {
                    rd = 1;
                    for (j = 0; j < n; j++)
                    {
                        ddv = 1;
                        switch (number[i + j])
                        {
                            case '0':
                                if (n - j == 3) doc += cs[0] + " ";
                                if (n - j == 2)
                                {
                                    if (number[i + j + 1] != '0') doc += "lẻ ";
                                    ddv = 0;
                                }
                                break;
                            case '1':
                                if (n - j == 3) doc += cs[1] + " ";
                                if (n - j == 2)
                                {
                                    doc += "mười ";
                                    ddv = 0;
                                }
                                if (n - j == 1)
                                {
                                    if (i + j == 0) k = 0;
                                    else k = i + j - 1;

                                    if (number[k] != '1' && number[k] != '0')
                                        doc += "mốt ";
                                    else
                                        doc += cs[1] + " ";
                                }
                                break;
                            case '5':
                                if (i + j == len - 1)
                                    doc += "lăm ";
                                else
                                    doc += cs[5] + " ";
                                break;
                            default:
                                doc += cs[(int)number[i + j] - 48] + " ";
                                break;
                        }

                        //Doc don vi nho
                        if (ddv == 1)
                        {
                            doc += dv[n - j - 1] + " ";
                        }
                    }
                }

                //Doc don vi lon
                if (len - i - n > 0)
                {
                    if ((len - i - n) % 9 == 0)
                    {
                        if (rd == 1)
                            for (k = 0; k < (len - i - n) / 9; k++)
                                doc += "tỉ ";
                        rd = 0;
                    }
                    else
                        if (found != 0) doc += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
                }

                i += n;
            }

            if (len == 1)
                if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];
            doc = am + (am == "" ? doc.Substring(0, 1).ToUpper() + doc.Substring(1) : doc);
            return doc;
        }
    }
}
