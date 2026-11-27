using UnityEngine;

namespace SandStrike
{
    public sealed class SkinDef
    {
        public string name;
        public int rarity; // 0 обычный, 1 редкий, 2 эпический, 3 легендарный
        public Color a;
        public Color b;
        public Color c;

        public Color RarityColor
        {
            get
            {
                if (rarity == 3) return WeaponCatalog.Hex("FFC857");
                if (rarity == 2) return WeaponCatalog.Hex("C77DE8");
                if (rarity == 1) return WeaponCatalog.Hex("63B9F5");
                return WeaponCatalog.Hex("A7ADB4");
            }
        }
    }

    /** Скины персонажа и оружия. Выбор сохраняется через PlayerPrefs. */
    public static class SkinCatalog
    {
        private const string KWeapon = "ss_weapon";
        private const string KGunSkin = "ss_gun_skin";
        private const string KCharSkin = "ss_char_skin";
        private const string KBots = "ss_bots";
        private const string KDiff = "ss_diff";
        private const string KSens = "ss_sens";
        private const string KSound = "ss_sound";
        private const string KMap = "ss_map";
        private const string KKills = "ss_kills";
        private const string KWins = "ss_wins";

        public static readonly SkinDef[] Characters =
        {
            S("Песчаный дозор", 0, "C2A878", "6E6047", "E2B866"),
            S("Графит", 0, "4A5056", "2C3136", "9AA3AA"),
            S("Хаки", 0, "7C8468", "4B523F", "C3C9A7"),
            S("Кобальт", 1, "355A8C", "1F3A60", "7CB2E8"),
            S("Лесник", 1, "3E6148", "263D2D", "8FC79B"),
            S("Красный песок", 1, "9B4B3C", "5E2D25", "E58368"),
            S("Полярный", 2, "D7DEE2", "8FA2AF", "FFFFFF"),
            S("Тень", 2, "2E3033", "17191B", "666B70"),
            S("Золото", 3, "C9A44A", "7C6123", "FFE29A"),
            S("Неон", 3, "2CD3C0", "126D68", "A6FFF4"),
            S("Кварц", 2, "D8A7AE", "8B666D", "FFD6DC"),
            S("Радар", 3, "E07A32", "7A3F1A", "FFC08A")
        };

        public static readonly SkinDef[] Guns =
        {
            S("Сталь", 0, "6E767D", "3A4045", "D5DBDE"),
            S("Песок", 0, "B59B70", "6D5C41", "EBD6A8"),
            S("Карбон", 1, "3D4246", "202427", "7A8288"),
            S("Хром", 1, "BCC6CB", "748085", "FFFFFF"),
            S("Золото", 2, "CEAA4F", "806327", "FFE7A6"),
            S("Неон", 2, "23C9B6", "116A62", "9DFFF1"),
            S("Коралл", 2, "C75B4B", "74352B", "FFA38E"),
            S("Лёд", 2, "9FC9DE", "5B7E96", "E8F7FF"),
            S("Яд", 3, "8DB93F", "42601C", "D6FF92"),
            S("Плазма", 3, "9B5BD6", "4C2A75", "E0BAFF"),
            S("Заря", 3, "E5622F", "7C3317", "FFBE8C"),
            S("Графит", 1, "4B4F54", "272A2D", "8C9297")
        };

        private static SkinDef S(string name, int rarity, string a, string b, string c)
        {
            SkinDef s = new SkinDef();
            s.name = name;
            s.rarity = rarity;
            s.a = WeaponCatalog.Hex(a);
            s.b = WeaponCatalog.Hex(b);
            s.c = WeaponCatalog.Hex(c);
            return s;
        }

        public static int Weapon { get { return Clamp(PlayerPrefs.GetInt(KWeapon, 13), 0, WeaponCatalog.All.Length - 1); } set { PlayerPrefs.SetInt(KWeapon, Clamp(value, 0, WeaponCatalog.All.Length - 1)); } }
        public static int GunSkin { get { return Clamp(PlayerPrefs.GetInt(KGunSkin, 0), 0, Guns.Length - 1); } set { PlayerPrefs.SetInt(KGunSkin, Clamp(value, 0, Guns.Length - 1)); } }
        public static int CharSkin { get { return Clamp(PlayerPrefs.GetInt(KCharSkin, 0), 0, Characters.Length - 1); } set { PlayerPrefs.SetInt(KCharSkin, Clamp(value, 0, Characters.Length - 1)); } }
        public static int Bots { get { return Clamp(PlayerPrefs.GetInt(KBots, 4), 1, 9); } set { PlayerPrefs.SetInt(KBots, Clamp(value, 1, 9)); } }
        public static int Diff { get { return Clamp(PlayerPrefs.GetInt(KDiff, 1), 0, 2); } set { PlayerPrefs.SetInt(KDiff, Clamp(value, 0, 2)); } }
        public static int Sens { get { return Clamp(PlayerPrefs.GetInt(KSens, 100), 40, 220); } set { PlayerPrefs.SetInt(KSens, Clamp(value, 40, 220)); } }
        public static bool Sound { get { return PlayerPrefs.GetInt(KSound, 1) == 1; } set { PlayerPrefs.SetInt(KSound, value ? 1 : 0); } }
        public static int Map { get { return Clamp(PlayerPrefs.GetInt(KMap, 0), 0, 2); } set { PlayerPrefs.SetInt(KMap, Clamp(value, 0, 2)); } }
        public static int Kills { get { return PlayerPrefs.GetInt(KKills, 0); } set { PlayerPrefs.SetInt(KKills, value); } }
        public static int Wins { get { return PlayerPrefs.GetInt(KWins, 0); } set { PlayerPrefs.SetInt(KWins, value); } }

        private static int Clamp(int v, int min, int max) { return v < min ? min : (v > max ? max : v); }

        public static void Save() { PlayerPrefs.Save(); }
    }
}
