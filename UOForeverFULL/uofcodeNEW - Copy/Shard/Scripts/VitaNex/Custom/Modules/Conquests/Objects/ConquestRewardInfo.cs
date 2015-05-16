#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CustomTitles;
using Server.Engines.XmlSpawner2;
using VitaNex;

#endregion

namespace Server.Engines.Conquests
{
    public sealed class ConquestRewardInfo
    {
        public static Dictionary<Conquest, Dictionary<Type, ConquestRewardInfo>> RewardCache { get; private set; }

        static ConquestRewardInfo()
        {
            RewardCache = new Dictionary<Conquest, Dictionary<Type, ConquestRewardInfo>>();
        }

        public static ConquestRewardInfo EnsureInfo(Conquest c, Type t)
        {
            if (t == null)
            {
                return null;
            }

            Dictionary<Type, ConquestRewardInfo> list;

            if (!RewardCache.TryGetValue(c, out list))
            {
                RewardCache.Add(c, list = new Dictionary<Type, ConquestRewardInfo>());
            }
            else if (list == null)
            {
                RewardCache[c] = list = new Dictionary<Type, ConquestRewardInfo>();
            }

            ConquestRewardInfo info;

            if (!list.TryGetValue(t, out info))
            {
                info = CreateInstance(t);

                if (info != null)
                {
                    list.Add(t, info);
                }
            }
            else if (info == null)
            {
                info = CreateInstance(t);

                if (info != null)
                {
                    list[t] = info;
                }
                else
                {
                    list.Remove(t);
                }
            }

            return info;
        }

        private static ConquestRewardInfo CreateInstance(Type t)
        {
            if (t == null)
            {
                return null;
            }

            return VitaNexCore.TryCatchGet(
                () =>
                {
                    ConquestRewardInfo info;

                    if (t.IsEqualOrChildOf<TitleScroll>())
                    {
                        var scroll = t.CreateInstanceSafe<TitleScroll>();

                        if (scroll != null)
                        {
                            string name = scroll.ResolveName();

                            if (scroll.Title != null)
                            {
                                name += " - " +
                                        (scroll.Title.MaleTitle == scroll.Title.FemaleTitle
                                            ? scroll.Title.MaleTitle
                                            : (scroll.Title.MaleTitle + ":" + scroll.Title.FemaleTitle));
                            }

                            info = new ConquestRewardInfo(
                                t, scroll.LabelNumber, name, scroll.Amount, scroll.Stackable, scroll.Hue, scroll.ItemID);

                            scroll.Delete();
                            return info;
                        }
                    }
                    else if (t.IsEqualOrChildOf<HueScroll>())
                    {
                        var scroll = t.CreateInstanceSafe<HueScroll>();

                        if (scroll != null)
                        {
                            string name = scroll.ResolveName();

                            if (scroll.TitleHue != null)
                            {
                                name += ": #" + scroll.TitleHue;
                            }

                            info = new ConquestRewardInfo(
                                t, scroll.LabelNumber, name, scroll.Amount, scroll.Stackable, scroll.Hue, scroll.ItemID);

                            scroll.Delete();
                            return info;
                        }
                    }
                    else if (t.IsEqualOrChildOf<Item>())
                    {
                        var item = t.CreateInstanceSafe<Item>();

                        if (item != null)
                        {
                            info = new ConquestRewardInfo(
                                t, item.LabelNumber, item.ResolveName().ToUpperWords(), item.Amount, item.Stackable,
                                item.Hue, item.ItemID);

                            item.Delete();
                            return info;
                        }
                    }
                    else if (t.IsEqualOrChildOf<Mobile>())
                    {
                        var mob = t.CreateInstanceSafe<Mobile>();

                        if (mob != null)
                        {
                            info = new ConquestRewardInfo(t, 0, mob.RawName.ToUpperWords(), 1, false, mob.Hue,
                                ShrinkTable.Lookup(mob));

                            mob.Delete();

                            return info;
                        }
                    }
                    else if (t.IsEqualOrChildOf<IEntity>())
                    {
                        var ent = t.CreateInstanceSafe<IEntity>();

                        if (ent != null)
                        {
                            info = new ConquestRewardInfo(t, 0, t.Name.SpaceWords());

                            ent.Delete();
                            return info;
                        }
                    }
                    else if (t.IsEqualOrChildOf<XmlAttachment>())
                    {
                        var xml = t.CreateInstanceSafe<XmlAttachment>();

                        if (xml != null)
                        {
                            info = new ConquestRewardInfo(t, 0, xml.Name.ToUpperWords());

                            xml.Delete();
                            return info;
                        }
                    }

                    info = new ConquestRewardInfo(t, 0, t.Name.SpaceWords());

                    return info;
                },
                Conquests.CMOptions.ToConsole);
        }

        public static void Defragment()
        {
            RewardCache.Values.Where(l => l != null).ForEach(
                l =>
                {
                    l.RemoveKeyRange(t => !t.IsConstructable());
                    l.RemoveValueRange(i => i == null || i.TypeOf == null || i.Amount <= 0);
                });
            RewardCache.RemoveValueRange(l => l == null || l.Count == 0);
            RewardCache.RemoveKeyRange(c => c.Deleted || !c.Enabled || c.Rewards.Count == 0);
        }

        public Type TypeOf { get; private set; }
        public int Label { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public bool Stackable { get; set; }
        public int Hue { get; set; }
        public int ItemID { get; set; }

        public ConquestRewardInfo(
            Type type, int label, string name, int amount = 1, bool stackable = false, int hue = 0, int itemID = 0)
        {
            TypeOf = type;
            Name = name;
            Label = label;
            Amount = amount;
            Stackable = stackable;
            Hue = hue;
            ItemID = itemID;
        }

        public override string ToString()
        {
            return String.Format("{0:#,0} x {1}", Amount, Name);
        }
    }
}