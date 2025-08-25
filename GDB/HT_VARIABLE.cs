using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_VARIABLE : BaseTable
    {
        [Key]
        public string TEN { get; set; }
        public string GIATRI { get; set; }
    }

    public class HT_VARIABLECollection : TableCollection
    {
        private static List<HT_VARIABLE> lstHT_VARIABLE;
        public HT_VARIABLECollection(GDBContext db) : base(db) { }

        public IQueryable<HT_VARIABLE> Get(bool reset = false)
        {
            if (lstHT_VARIABLE == null || reset) lstHT_VARIABLE = _db.HT_VARIABLE.ToList();
            return lstHT_VARIABLE.AsQueryable();
        }

        public List<HT_VARIABLE> GetList(bool reset = false)
        {
            if (lstHT_VARIABLE == null || reset) lstHT_VARIABLE = _db.HT_VARIABLE.ToList();
            return lstHT_VARIABLE;
        }

        public HT_VARIABLE GetById(string ten)
        {
            return GetList().FirstOrDefault(c => c.TEN == ten);
        }

        public void Add(HT_VARIABLE item)
        {
            _db.HT_VARIABLE.Add(item);
            _db.SaveChanges();
            GetList(true);
        }

        public void Update(HT_VARIABLE item)
        {
            _db.HT_VARIABLE.Update(item);
            _db.SaveChanges();
            GetList(true);
        }

        public void Remove(HT_VARIABLE item)
        {
            if (item == null) return;
            _db.HT_VARIABLE.Remove(item);
            _db.SaveChanges();
            GetList(true);
        }

        public void Remove(string name)
        {
            Remove(GetById(name));
        }
        //data
        public void AddData(string ten, object data)
        {
            Add(new HT_VARIABLE { TEN = ten, GIATRI = JsonConvert.SerializeObject(data) });
        }
        public void UpdateObject(string name, object data)
        {
            HT_VARIABLE item = GetById(name);
            if (item != null) UpdateObject(item, data);
            else AddData(name, data);
        }
        public void UpdateObject(HT_VARIABLE item, object data)
        {
            item.GIATRI = JsonConvert.SerializeObject(data);
            Update(item);
        }
        public T GetData<T>(string name)
        {
            HT_VARIABLE item = GetById(name);
            if (item == null || (item != null && item.GIATRI == "{}")) return default(T);
            return JsonConvert.DeserializeObject<T>(item.GIATRI);
        }
        //object in data
        public void AddObjectList(string name, string key, object data)
        {
            Dictionary<string, object> items = new Dictionary<string, object>();
            HT_VARIABLE item = GetById(name);
            if (item == null)
            {
                items.Add(key, data);
                AddData(name, items);
            }
            else
            {
                items = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.GIATRI);
                if (items.ContainsKey(key))
                {
                    items[key] = data;
                }
                else
                {
                    items.Add(key, data);
                }
                UpdateObject(item, items);
            }
        }
        public void UpdateObjectList(string name, string key, object data)
        {
            HT_VARIABLE item = GetById(name);
            if (item != null)
            {
                Dictionary<string, object> items = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.GIATRI);
                if (items != null)
                {
                    if (items.ContainsKey(key))
                    {
                        items[key] = data;
                    }
                    else
                    {
                        items.Add(key, data);
                    }
                    UpdateObject(item, items);
                }
            }
        }
        public void RemoveObjectList(string name, string key)
        {
            HT_VARIABLE item = GetById(name);
            if (item != null)
            {
                Dictionary<string, object> items = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.GIATRI);
                if (items != null)
                {
                    items.Remove(key);
                    UpdateObject(item, items);
                }
            }
        }
        public T GetObjectList<T>(string name, string key)
        {
            Dictionary<string, object> items = GetData<Dictionary<string, object>>(name);
            if (items == null) return default(T);
            if (!items.ContainsKey(key) || (items.ContainsKey(key) && items[key] == null)) return default(T);
            JObject j;
            if (typeof(T) == typeof(DateTime))
            {
                return (T)items[key];
            }
            else if (typeof(T) == typeof(String))
            {
                return (T)items[key];
            }
            else if (typeof(T) == typeof(JArray))
            {
                return (T)items[key];
            }
            else if (typeof(T).Name == typeof(List<>).Name)
            {
                return ((JArray)items[key]).ToObject<T>();
            }
            else
            {
                j = JObject.FromObject(items[key]);
            }
            return j.ToObject<T>();
        }

        public string FindObjectList(string name, string key)
        {
            Dictionary<string, object> items = GetData<Dictionary<string, object>>(name);
            if (items == null) return "";
            if (items[key] == null) return "";
            return Convert.ToString(items[key]);
        }

        public string GetVariableName(string type, params string[] args)
        {
            string baseName = "";
            switch (type)
            {
                case "Menu":
                    baseName = "menu.settings";
                    break;
                //case "Roles":
                //    baseName = "roles.settings";
                //    break;
                case "System":
                    baseName = "system.settings";
                    break;
                case "Variable":
                    baseName = "variable.settings";
                    break;
            }

            return baseName + (args.Length > 0 ? "." + String.Join(".", args) : "");
        }

        ////HT_NHOMQUYEN
        //public IQueryable<HT_NHOMQUYEN> HT_NHOMQUYEN_Get()
        //{
        //    Dictionary<string, object> items = GetData<Dictionary<string, object>>(GetVariableName("Roles"));
        //    return items?.Select(s => new HT_NHOMQUYEN(s.Value)).AsQueryable();
        //}
        //public IQueryable<HT_NHOMQUYEN> HT_NHOMQUYEN_Get(long DVQL_ID)
        //{
        //    return HT_NHOMQUYEN_Get()?.Where(c => c.DVQL_ID == DVQL_ID);
        //}
        //public IQueryable<HT_NHOMQUYEN> HT_NHOMQUYEN_Get(long DVQL_ID, long DVSD_ID)
        //{
        //    return HT_NHOMQUYEN_Get()?.Where(c => c.DVQL_ID == DVQL_ID && c.DVSD_ID == DVSD_ID);
        //}
        //public void HT_NHOMQUYEN_Add(HT_NHOMQUYEN item)
        //{
        //    item.ID = HT_NHOMQUYEN_LastID() + 1;
        //    AddObjectList(GetVariableName("Roles"), item.ID.ToString(), item);
        //}
        //public void HT_NHOMQUYEN_Delete(long rid)
        //{
        //    RemoveObjectList(GetVariableName("Roles"), rid + "");
        //}
        //public HT_NHOMQUYEN HT_NHOMQUYEN_GetByID(long ID)
        //{
        //    return HT_NHOMQUYEN_Get()?.FirstOrDefault(c => c.ID == ID);
        //}
        //public HT_NHOMQUYEN HT_NHOMQUYEN_GetTENQUYEN(string TEN_NHOM)
        //{
        //    return HT_NHOMQUYEN_Get()?.FirstOrDefault(c => c.TEN_NHOM.ToLower() == TEN_NHOM.ToLower());
        //}
        //public void HT_NHOMQUYEN_Update(HT_NHOMQUYEN item)
        //{
        //    UpdateObjectList(GetVariableName("Roles"), item.ID.ToString(), item);
        //}
        //public long HT_NHOMQUYEN_LastID()
        //{
        //    var r = HT_NHOMQUYEN_Get()?.OrderBy(o => o.ID).Last();
        //    return r == null ? 0 : r.ID;
        //}
        //menu
        public IQueryable<Menus> MenusGet()
        {
            Dictionary<string, object> items = GetData<Dictionary<string, object>>(GetVariableName("Menu"));
            return items?.Select(s => new Menus(s.Value)).AsQueryable();
        }
        public void MenusAdd(Menus item)
        {
            AddObjectList(GetVariableName("Menu"), item.MID, item);
        }
        public void MenusDelete(string mid)
        {
            MenuItemDelete(mid);
            RemoveObjectList(GetVariableName("Menu"), mid);
        }
        public Menus MenusGet(string mid)
        {
            var items = MenusGet();
            return items?.FirstOrDefault(c => c.MID == mid);
        }
        public void MenusUpdate(string mid, Menus item)
        {
            UpdateObjectList(GetVariableName("Menu"), mid, item);
        }
        //MenuItem
        public IQueryable<MenuItem> MenuItemGetAll(params string[] args)
        {
            Dictionary<string, object> items = GetData<Dictionary<string, object>>(GetVariableName("Menu", args));
            return items?.Select(s => new MenuItem(s.Value)).AsQueryable();
        }
        public void MenuItemAdd(MenuItem item, params string[] args)
        {
            item.ID = MenuItemLastID(args) + 1;
            AddObjectList(GetVariableName("Menu", args), item.ID.ToString(), item);
        }
        public void MenuItemDelete(int id, params string[] args)
        {
            RemoveObjectList(GetVariableName("Menu", args), id + "");
        }
        public void MenuItemDelete(params string[] args)
        {
            Remove(GetVariableName("Menu", args));
        }
        public MenuItem MenuItemGet(int id, params string[] args)
        {
            var items = MenuItemGetAll(args);
            return items?.FirstOrDefault(c => c.ID == id);
        }
        public MenuItem MenuItemGetByPath(string href, params string[] args)
        {
            var items = MenuItemGetAll(args);
            return items?.FirstOrDefault(c => c.HREF.ToLower() == href.ToLower());
        }
        public void MenuItemUpdate(MenuItem item, params string[] args)
        {
            UpdateObjectList(GetVariableName("Menu", args), item.ID.ToString(), item);
        }
        public int MenuItemLastID(params string[] args)
        {
            var f = MenuItemGetAll(args)?.OrderBy(o => o.ID).Last();
            return f == null ? 0 : f.ID;
        }
        //variable
        public void VariableSet(string name, string key, object item)
        {
            AddObjectList(GetVariableName("Variable", name), key, item);
        }
        public T VariableGet<T>(string name, string key, object defaultValue = null)
        {
            var item = GetObjectList<T>(GetVariableName("Variable", name), key);
            if (item == null) return (T)defaultValue;
            return item;
        }
    }

    //public class HT_NHOMQUYEN : BaseTable
    //{
    //    [Key]
    //    public long ID { get; set; }
    //    [Key]
    //    public long DVQL_ID { get; set; }
    //    public long DVSD_ID { get; set; }
    //    public string TEN_NHOM { get; set; }
    //    public long? SAP_XEP { get; set; }
    //    public HT_NHOMQUYEN() { }
    //    public HT_NHOMQUYEN(object item) : base(item) { }
    //}

    public class Menus : BaseTable
    {
        public string MID { get; set; }
        public string NAME { get; set; }
        public Menus() { }
        public Menus(object item) : base(item) { }
    }
    public class MenuItem : BaseTable
    {
        public int ID { get; set; }
        public int PID { get; set; }
        public string NAME { get; set; }
        public string HREF { get; set; }
        public string PERM { get; set; }
        public int WEIGHT { get; set; }
        public bool HIDEN { get; set; }
        public string ICON { get; set; }
        public int? DON_VI { get; set; }
        public MenuItem() { }
        public MenuItem(object item) : base(item) { }
    }

    public class MenusTree : MenuItem
    {
        public List<MenuItem> Nodes { get; set; }
        public MenusTree() { }
        public MenusTree(MenuItem item, List<MenuItem> list) : base(item)
        {
            Nodes = new List<MenuItem>();
            foreach (var c in list.Where(s => s.PID == item.ID).ToList())
            {
                var n = new MenusTree(c, list);
                Nodes.Add(n);
            }
        }

        public static List<MenusTree> BuildToData(List<MenuItem> list)
        {
            var nested = new List<MenusTree>();
            foreach (var c in list.Where(s => s.PID == 0).ToList())
            {
                if (c == null) continue;
                var n = new MenusTree(c, list);
                nested.Add(n);
            }
            return nested;
        }
    }
}
