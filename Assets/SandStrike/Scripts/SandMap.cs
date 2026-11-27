using System.Collections.Generic;
using UnityEngine;

namespace SandStrike
{
    /** Карты, включая основную Sandstone. Геометрия строится во время 실행. */
    public static class SandMap
    {
        public const int Size = 28;
        public const float Cell = 2f;

        public static readonly string[] Names = { "Песчаник  /  SANDSTONE", "Каньон", "Руины" };

        private static readonly string Open = ".PTAB~";

        public static bool Walkable(char ch) { return Open.IndexOf(ch) >= 0; }

        public static char[][] Build(int index)
        {
            char[][] m = new char[Size][];
            for (int y = 0; y < Size; y++)
            {
                m[y] = new char[Size];
                for (int x = 0; x < Size; x++) m[y][x] = '#';
            }
            if (index == 0) Sandstone(m);
            else if (index == 1) Canyon(m);
            else Ruins(m);
            return m;
        }

        private static void Room(char[][] m, int x1, int y1, int x2, int y2)
        {
            for (int y = y1; y <= y2; y++)
                for (int x = x1; x <= x2; x++)
                    if (x > 0 && y > 0 && x < Size - 1 && y < Size - 1) m[y][x] = '.';
        }

        private static void HCorr(char[][] m, int y, int x1, int x2)
        {
            for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++) m[y][x] = '.';
        }

        private static void VCorr(char[][] m, int x, int y1, int y2)
        {
            for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++) m[y][x] = '.';
        }

        private static void Put(char[][] m, int x, int y, char ch) { m[y][x] = ch; }

        private static void Sandstone(char[][] m)
        {
            Room(m, 2, 2, 8, 7); Room(m, 19, 2, 25, 7); Room(m, 11, 11, 16, 16);
            Room(m, 2, 20, 8, 25); Room(m, 19, 20, 25, 25); Room(m, 2, 12, 7, 15);
            Room(m, 20, 12, 25, 15); Room(m, 12, 2, 15, 8); Room(m, 12, 19, 15, 25);
            HCorr(m, 4, 8, 12); HCorr(m, 4, 15, 19); HCorr(m, 23, 8, 12); HCorr(m, 23, 15, 19);
            VCorr(m, 5, 7, 12); VCorr(m, 22, 7, 12); VCorr(m, 5, 15, 20); VCorr(m, 22, 15, 20);
            HCorr(m, 13, 7, 11); HCorr(m, 13, 16, 20);
            Put(m, 5, 4, 'A'); Put(m, 22, 23, 'B');
            Put(m, 14, 14, 'P');
            Put(m, 5, 5, 'T'); Put(m, 22, 22, 'T'); Put(m, 14, 4, 'T');
            Put(m, 14, 23, 'T'); Put(m, 4, 13, 'T'); Put(m, 23, 13, 'T');
            int[] cx = { 4, 7, 23, 20, 4, 7, 23, 20, 13, 14 };
            int[] cy = { 6, 3, 4, 6, 22, 25, 21, 24, 12, 15 };
            for (int i = 0; i < cx.Length; i++) Put(m, cx[i], cy[i], 'X');
            Put(m, 12, 4, '='); Put(m, 15, 23, '='); Put(m, 6, 13, '='); Put(m, 21, 13, '=');
        }

        private static void Canyon(char[][] m)
        {
            Room(m, 2, 2, 9, 8); Room(m, 18, 2, 25, 8); Room(m, 2, 19, 9, 25);
            Room(m, 18, 19, 25, 25); Room(m, 10, 10, 17, 17);
            HCorr(m, 5, 9, 18); HCorr(m, 22, 9, 18);
            VCorr(m, 5, 8, 19); VCorr(m, 22, 8, 19);
            VCorr(m, 13, 2, 10); VCorr(m, 14, 17, 25);
            Put(m, 5, 5, 'A'); Put(m, 22, 22, 'B'); Put(m, 13, 13, 'P');
            Put(m, 6, 5, 'T'); Put(m, 21, 22, 'T'); Put(m, 5, 22, 'T');
            Put(m, 22, 5, 'T'); Put(m, 15, 12, 'T');
            int[] cx = { 3, 8, 19, 24, 3, 8, 19, 24 };
            int[] cy = { 3, 7, 7, 3, 24, 21, 21, 24 };
            for (int i = 0; i < cx.Length; i++) Put(m, cx[i], cy[i], 'X');
            Put(m, 12, 5, '='); Put(m, 16, 22, '='); Put(m, 5, 12, '='); Put(m, 22, 16, '=');
        }

        private static void Ruins(char[][] m)
        {
            Room(m, 2, 2, 12, 12); Room(m, 15, 2, 25, 12);
            Room(m, 2, 15, 12, 25); Room(m, 15, 15, 25, 25); Room(m, 11, 11, 16, 16);
            HCorr(m, 7, 12, 15); HCorr(m, 20, 12, 15);
            VCorr(m, 7, 12, 15); VCorr(m, 20, 12, 15);
            Put(m, 5, 5, 'A'); Put(m, 22, 22, 'B'); Put(m, 13, 13, 'P');
            Put(m, 6, 6, 'T'); Put(m, 20, 20, 'T'); Put(m, 6, 20, 'T');
            Put(m, 20, 6, 'T'); Put(m, 12, 12, 'T'); Put(m, 16, 15, 'T');
            for (int y = 4; y <= 10; y += 3)
            {
                Put(m, 5, y, 'X'); Put(m, 9, y, 'X'); Put(m, 18, y, 'X'); Put(m, 22, y, 'X');
            }
            for (int y = 17; y <= 23; y += 3)
            {
                Put(m, 5, y, 'X'); Put(m, 9, y, 'X'); Put(m, 18, y, 'X'); Put(m, 22, y, 'X');
            }
            Put(m, 13, 9, '='); Put(m, 13, 18, '='); Put(m, 9, 13, '='); Put(m, 18, 13, '=');
        }

        public static Vector3 CellToWorld(int x, int y)
        {
            return new Vector3((x - Size / 2f) * Cell, 0f, (y - Size / 2f) * Cell);
        }

        public static Vector2 WorldToCell(Vector3 p)
        {
            return new Vector2(Mathf.RoundToInt(p.x / Cell + Size / 2f), Mathf.RoundToInt(p.z / Cell + Size / 2f));
        }

        public static List<Vector3> Spawns(char[][] m, char type)
        {
            List<Vector3> list = new List<Vector3>();
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    if (m[y][x] == type) list.Add(CellToWorld(x, y));
            return list;
        }

        public static GameObject BuildScene(int index, char[][] m, Transform parent)
        {
            GameObject root = new GameObject("Map_" + index);
            root.transform.SetParent(parent, false);

            Material wall = MakeMaterial(WeaponCatalog.Hex("B99A6C"));
            Material wallDark = MakeMaterial(WeaponCatalog.Hex("8D714C"));
            Material floor = MakeMaterial(WeaponCatalog.Hex("C7A97B"));
            Material crate = MakeMaterial(WeaponCatalog.Hex("6B5134"));
            Material siteA = MakeMaterial(WeaponCatalog.Hex("E2B866"));
            Material siteB = MakeMaterial(WeaponCatalog.Hex("6FBBD1"));

            Cube(root.transform, "Floor", new Vector3(0, -0.26f, 0), new Vector3(Size * Cell, 0.5f, Size * Cell), floor);

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    char ch = m[y][x];
                    if (ch == '.' || ch == 'P' || ch == 'T') continue;
                    Vector3 p = CellToWorld(x, y);
                    if (ch == '#')
                    {
                        float h = 3f + (((x * 7 + y * 13) % 5) * 0.18f);
                        Cube(root.transform, "Wall", new Vector3(p.x, h * 0.5f, p.z),
                            new Vector3(Cell, h, Cell), ((x + y) % 3 == 0) ? wallDark : wall);
                    }
                    else if (ch == 'X')
                    {
                        Cube(root.transform, "Crate", new Vector3(p.x, 0.8f, p.z),
                            new Vector3(1.5f, 1.6f, 1.5f), crate);
                    }
                    else if (ch == '=')
                    {
                        Cube(root.transform, "Cover", new Vector3(p.x, 0.6f, p.z),
                            new Vector3(Cell, 1.2f, Cell * 0.5f), crate);
                    }
                    else if (ch == 'A' || ch == 'B')
                    {
                        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        disc.name = "Site " + ch;
                        disc.transform.SetParent(root.transform, false);
                        disc.transform.position = new Vector3(p.x, 0.03f, p.z);
                        disc.transform.localScale = new Vector3(2.6f, 0.04f, 2.6f);
                        disc.GetComponent<Renderer>().sharedMaterial = ch == 'A' ? siteA : siteB;
                        Object.Destroy(disc.GetComponent<Collider>());
                    }
                }
            }
            StaticBatchingUtility.Combine(root);
            return root;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name;
            g.transform.SetParent(parent, false);
            g.transform.position = pos;
            g.transform.localScale = scale;
            g.GetComponent<Renderer>().sharedMaterial = mat;
            return g;
        }

        private static Material MakeMaterial(Color color)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Diffuse");
            Material m = new Material(shader);
            m.color = color;
            return m;
        }
    }
}
