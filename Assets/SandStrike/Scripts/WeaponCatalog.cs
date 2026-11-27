using UnityEngine;

namespace SandStrike
{
    public enum WeaponClass { Pistol, Smg, Rifle, Sniper, Shotgun, Knife }

    public sealed class WeaponDef
    {
        public string name;
        public string proto;
        public WeaponClass cls;
        public int damage;
        public int rpm;
        public int mag;
        public int reserve;
        public int spread;
        public int recoil;
        public int range;
        public float moveSpeed;
        public int archetype; // 0 secondary, 1 shotgun, 2 blaster, 3 launcher, 4 knife
        public Color body;
        public Color metal;
        public float scale;

        public string ClassName
        {
            get
            {
                if (cls == WeaponClass.Pistol) return "ПИСТОЛЕТ";
                if (cls == WeaponClass.Smg) return "ПП";
                if (cls == WeaponClass.Rifle) return "АВТОМАТ";
                if (cls == WeaponClass.Sniper) return "СНАЙПЕРКА";
                if (cls == WeaponClass.Shotgun) return "ДРОБОВИК";
                return "НОЖ";
            }
        }
    }

    /** 30 позиций. Прототипы из Standoff 2, названия — наши. */
    public static class WeaponCatalog
    {
        public static readonly WeaponDef[] All =
        {
            W("Кремень", "USP-S", WeaponClass.Pistol, 34, 285, 12, 4, 7, 14, 0, "3B4148", "A9AFB6", 0.95f),
            W("Гром", "P250", WeaponClass.Pistol, 31, 333, 13, 6, 9, 13, 0, "4A4F52", "BFC5C8", 0.95f),
            W("Песчанка", "Desert Eagle", WeaponClass.Pistol, 58, 143, 7, 9, 22, 18, 0, "6B6255", "D8D2C6", 1.12f),
            W("Искра", "Glock-18", WeaponClass.Pistol, 28, 375, 18, 7, 8, 12, 0, "2F3539", "9DA5A9", 0.9f),
            W("Шмель", "Five-SeveN", WeaponClass.Pistol, 29, 400, 20, 5, 7, 13, 0, "4C5147", "B9BEAF", 0.92f),
            W("Ятаган", "Tec-9", WeaponClass.Pistol, 33, 375, 18, 9, 11, 12, 0, "55504A", "C9BFB2", 1f),
            W("Двойка", "Dual Berettas", WeaponClass.Pistol, 26, 500, 30, 11, 10, 11, 0, "565B5E", "C6CCCE", 0.86f),

            W("Метла", "MP7", WeaponClass.Smg, 24, 667, 30, 7, 6, 15, 0, "343A3F", "AEB4B8", 1.1f),
            W("Оса", "MP9", WeaponClass.Smg, 23, 750, 30, 9, 6, 14, 0, "3D4245", "B6BCC0", 1.05f),
            W("Глыба", "UMP-45", WeaponClass.Smg, 31, 545, 25, 8, 9, 16, 1, "414744", "BEC4BC", 1.05f),
            W("Рой", "P90", WeaponClass.Smg, 22, 833, 50, 10, 5, 15, 2, "42484E", "B4BBC0", 1.15f),
            W("Кусок", "MAC-10", WeaponClass.Smg, 26, 800, 30, 11, 8, 12, 0, "4A4E4A", "C2C7C1", 1f),
            W("Стрела", "MP5", WeaponClass.Smg, 27, 632, 30, 7, 7, 16, 1, "373C40", "AEB5B9", 1.05f),

            W("Буря", "AK-47", WeaponClass.Rifle, 36, 571, 30, 7, 15, 22, 1, "6A4B32", "C8B79B", 1.2f),
            W("Клин", "M4A1", WeaponClass.Rifle, 33, 632, 30, 6, 12, 23, 1, "3A4147", "AEB6BB", 1.18f),
            W("Грань", "M4A1-S", WeaponClass.Rifle, 35, 571, 25, 5, 12, 24, 1, "42484B", "B7BDC0", 1.16f),
            W("Заря", "AKR", WeaponClass.Rifle, 34, 600, 30, 6, 14, 22, 1, "5E4632", "BFAC90", 1.18f),
            W("Заря-12", "AKR-12", WeaponClass.Rifle, 38, 522, 30, 8, 17, 21, 3, "57412F", "B9A487", 1.25f),
            W("Вестник", "M16", WeaponClass.Rifle, 32, 600, 30, 6, 11, 24, 1, "3F454A", "B2B8BD", 1.2f),
            W("Скальпель", "Galil", WeaponClass.Rifle, 35, 571, 30, 7, 14, 21, 1, "4D4A3E", "B9B49B", 1.15f),
            W("Копьё", "FAMAS", WeaponClass.Rifle, 34, 545, 25, 6, 13, 22, 1, "3F4446", "AEB4B6", 1.12f),
            W("Сфера", "AUG", WeaponClass.Rifle, 33, 632, 30, 5, 11, 24, 2, "414745", "B4BAB5", 1.1f),

            W("Глаз", "AWP", WeaponClass.Sniper, 115, 41, 5, 3, 34, 40, 3, "2F4A47", "B7C7C2", 1.35f),
            W("Даль", "AWM", WeaponClass.Sniper, 108, 38, 5, 3, 36, 42, 3, "3C4B3A", "BFC8B5", 1.35f),
            W("Точка", "SSG 08", WeaponClass.Sniper, 82, 50, 10, 4, 26, 32, 1, "4A4537", "C0B79D", 1.15f),
            W("Прицел", "SCAR-20", WeaponClass.Sniper, 80, 109, 20, 5, 24, 30, 3, "3E4448", "B0B7BC", 1.25f),

            W("Толкач", "XM1014", WeaponClass.Shotgun, 22, 71, 6, 24, 26, 9, 1, "5A4330", "C1AE92", 1.2f),
            W("Дробь", "Nova", WeaponClass.Shotgun, 26, 67, 8, 26, 28, 8, 1, "50483C", "B9B09E", 1.18f),
            W("Обрез", "Sawed-Off", WeaponClass.Shotgun, 32, 75, 6, 30, 30, 6, 1, "5C4632", "C6B393", 0.95f),

            W("Бабочка", "Butterfly Knife", WeaponClass.Knife, 55, 143, 1, 0, 0, 2, 4, "6E757A", "D8DEE0", 1f)
        };

        private static WeaponDef W(string name, string proto, WeaponClass cls, int damage,
            int rpm, int mag, int spread, int recoil, int range, int archetype,
            string body, string metal, float scale)
        {
            WeaponDef d = new WeaponDef();
            d.name = name;
            d.proto = proto;
            d.cls = cls;
            d.damage = damage;
            d.rpm = rpm;
            d.mag = mag;
            d.reserve = mag * 4;
            d.spread = spread;
            d.recoil = recoil;
            d.range = range;
            d.moveSpeed = cls == WeaponClass.Knife ? 1.12f : cls == WeaponClass.Sniper ? 0.78f :
                          cls == WeaponClass.Rifle ? 0.95f : cls == WeaponClass.Smg ? 1.03f : 1f;
            d.archetype = archetype;
            d.body = Hex(body);
            d.metal = Hex(metal);
            d.scale = scale;
            return d;
        }

        public static Color Hex(string hex)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + hex, out c)) return c;
            return Color.gray;
        }

        public static int IndexOf(string name)
        {
            for (int i = 0; i < All.Length; i++) if (All[i].name == name) return i;
            return 13;
        }
    }
}
