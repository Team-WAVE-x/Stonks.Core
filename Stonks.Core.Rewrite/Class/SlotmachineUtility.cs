using Discord;
using System;
using System.Collections.Generic;

namespace Stonks.Core.Rewrite.Class
{
    public class SlotmachineUtility
    {
        public enum Item
        {
            Melon,
            Cherry,
            Lemon,
            Star,
            Bell,
            Seven
        }

        public static Emoji EnumToEmoji(Item item)
        {
            var items = new Dictionary<Item, Emoji>()
            {
                { Item.Melon, new Emoji("\U0001f348") },
                { Item.Cherry, new Emoji("\U0001f352") },
                { Item.Lemon, new Emoji("\U0001f34b") },
                { Item.Star, new Emoji("\u2B50") },
                { Item.Bell, new Emoji("\U0001f514") },
                { Item.Seven, new Emoji("7\u20E3") }
            };

            return items[item];
        }

        public static Item RandomEnum()
        {
            Random rd = new Random();
            int value = rd.Next(1, 101);

            if (value <= 30)                         //1 ~ 30
            {
                return Item.Melon;
            }
            else if (value > 30 && value <= 60)      //31 ~ 60
            {
                return Item.Cherry;
            }
            else if (value > 60 && value <= 90)      //61 ~ 90
            {
                return Item.Lemon;
            }
            else if (value > 90 && value <= 95)      //91 ~ 95
            {
                return Item.Star;
            }
            else if (value > 95 && value <= 99)      //96 ~ 99
            {
                return Item.Bell;
            }
            else if (value == 100)                   //100
            {
                return Item.Seven;
            }
            else                                     //Else
            {
                throw new Exception("The value is not in the range.");
            }
        }
    }
}