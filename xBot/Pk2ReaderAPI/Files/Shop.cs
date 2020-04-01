using System.Collections.Generic;
namespace Pk2ReaderAPI.Files
{
    /// <summary>
    /// The shop structure that client uses to organize stores
    /// </summary>
    public class Shop
    {
        #region Public Properties
        public string StoreGroupName { get; set; }
        public string StoreName { get; set; }
        public string NPCServerName { get; set; }
        public List<Group> Groups { get; } = new List<Group>();
        #endregion
        
        public class Group
        {
            #region Public Properties
            public string Name { get; set; }
            public List<Tab> Tabs { get; } = new List<Tab>();
            #endregion

            public class Tab
            {
                #region Public Properties
                public string Name { get; set; }
                public string Title { get; set; }
                public List<Item> Items { get; } = new List<Item>();
                #endregion

                public class Item
                {
                    #region Public Properties
                    public string Name { get; set; }
                    public string Slot { get; set; }
                    public string Plus { get; set; }
                    public string RentType { get; set; }
                    public string Durability { get; set; }
                    public string MagicParams { get; set; }
                    #endregion
                }
            }
        }
    }
}
