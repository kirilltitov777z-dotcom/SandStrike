using UnityEngine;

namespace SandStrike
{
    /** Визуальные модельки: оружие, нож-бабочка и стоящий персонаж. */
    public sealed class SandButterfly : MonoBehaviour
    {
        public Transform left;
        public Transform right;
        public float speed = 1f;
        public bool auto = true;
        private float t;

        private void Update()
        {
            if (auto) t += Time.deltaTime * 1.7f;
            else t += Time.deltaTime * 3.5f;
            float a = Mathf.Sin(t * speed) * 150f;
            if (left != null) left.localRotation = Quaternion.Euler(0f, 0f, a);
            if (right != null) right.localRotation = Quaternion.Euler(0f, 0f, 180f - a);
        }

        public void Kick()
        {
            auto = false;
            t = 0f;
            Invoke(nameof(Restore), 1.2f);
        }

        private void Restore() { auto = true; }
    }

    public sealed class SandCharacterAnim : MonoBehaviour
    {
        public Transform torso;
        public Transform head;
        private float t;

        private void Update()
        {
            t += Time.deltaTime * 1.6f;
            float b = Mathf.Sin(t) * 0.012f;
            if (torso != null) torso.localPosition = new Vector3(0f, b, 0f);
            if (head != null) head.localPosition = new Vector3(0f, 1.62f + b * 1.4f, 0f);
        }
    }

    public static class SandVisuals
    {
        public static Material Mat(Color color, float metallic)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Diffuse");
            Material m = new Material(shader);
            m.color = color;
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.45f);
            return m;
        }

        /** Создаёт вьюмодель. Берём FBX из Resources, если он есть, иначе собираем из примитивов. */
        public static GameObject Weapon(WeaponDef def, SkinDef skin, Transform parent)
        {
            GameObject go;
            string res = null;
            if (def.archetype == 0) res = "Weapons/Weapon_Blaster";
            else if (def.archetype == 1) res = "Weapons/Weapon_Shotgun";
            else if (def.archetype == 3) res = "Weapons/Weapon_Launcher";

            GameObject prefab = res == null ? null : Resources.Load<GameObject>(res);
            if (prefab != null)
            {
                go = Object.Instantiate(prefab, parent, false);
                MonoBehaviour[] scripts = go.GetComponentsInChildren<MonoBehaviour>(true);
                for (int i = 0; i < scripts.Length; i++) scripts[i].enabled = false;
                Recolor(go, skin, def);
            }
            else
            {
                go = Procedural(def, skin, parent);
            }

            go.name = def.name;
            go.transform.localScale = Vector3.one * def.scale;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
            SetLayer(go, 0);
            return go;
        }

        private static void SetLayer(GameObject go, int layer)
        {
            Transform[] all = go.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++) all[i].gameObject.layer = layer;
        }

        private static void Recolor(GameObject go, SkinDef skin, WeaponDef def)
        {
            Color body = Color.Lerp(def.body, skin.a, 0.68f);
            Color metal = Color.Lerp(def.metal, skin.c, 0.45f);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer r = renderers[i];
                Material m = new Material(r.sharedMaterial != null ? r.sharedMaterial :
                    new Material(Shader.Find("Standard")));
                if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", i % 2 == 0 ? body : metal);
                else m.color = i % 2 == 0 ? body : metal;
                r.sharedMaterial = m;
            }
        }

        private static GameObject Procedural(WeaponDef def, SkinDef skin, Transform parent)
        {
            GameObject root = new GameObject(def.name);
            root.transform.SetParent(parent, false);
            Color body = Color.Lerp(def.body, skin.a, 0.68f);
            Color metal = Color.Lerp(def.metal, skin.c, 0.45f);
            Material bm = Mat(body, 0.2f);
            Material mm = Mat(metal, 0.8f);
            Material am = Mat(skin.c, 0.4f);

            float len = def.cls == WeaponClass.Sniper ? 1.35f : def.cls == WeaponClass.Pistol ? 0.52f : 0.95f;
            Box(root.transform, "corpus", new Vector3(0, 0, 0), new Vector3(0.16f, 0.17f, len * 0.55f), bm);
            Cyl(root.transform, "barrel", new Vector3(0, 0.02f, len * 0.5f), new Vector3(0.045f, len * 0.5f, 0.045f), mm);
            Box(root.transform, "grip", new Vector3(0, -0.2f, -0.12f), new Vector3(0.1f, 0.32f, 0.12f), bm);
            Box(root.transform, "mag", new Vector3(0, -0.2f, 0.12f), new Vector3(0.11f, 0.34f, 0.14f), mm);
            if (def.cls != WeaponClass.Pistol && def.cls != WeaponClass.Knife)
                Box(root.transform, "stock", new Vector3(0, 0, -len * 0.52f), new Vector3(0.12f, 0.14f, len * 0.4f), bm);
            Box(root.transform, "sight", new Vector3(0, 0.13f, 0.08f), new Vector3(0.05f, 0.06f, 0.12f), am);
            if (def.cls == WeaponClass.Sniper)
                Cyl(root.transform, "scope", new Vector3(0, 0.19f, 0.08f), new Vector3(0.07f, 0.3f, 0.07f), mm);
            return root;
        }

        private static GameObject Box(Transform p, string n, Vector3 pos, Vector3 scale, Material m)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = n;
            g.transform.SetParent(p, false);
            g.transform.localPosition = pos;
            g.transform.localScale = scale;
            g.GetComponent<Renderer>().sharedMaterial = m;
            return g;
        }

        private static GameObject Cyl(Transform p, string n, Vector3 pos, Vector3 scale, Material m)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            g.name = n;
            g.transform.SetParent(p, false);
            g.transform.localPosition = pos;
            g.transform.localScale = scale;
            g.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            g.GetComponent<Renderer>().sharedMaterial = m;
            return g;
        }

        /** Нож-бабочка с анимацией вращения половинок. */
        public static GameObject Butterfly(SkinDef skin, Transform parent)
        {
            GameObject root = new GameObject("Бабочка");
            root.transform.SetParent(parent, false);
            Material metal = Mat(Color.Lerp(WeaponCatalog.Hex("D8DEE0"), skin.c, 0.4f), 0.85f);
            Material grip = Mat(skin.b, 0.1f);
            Material accent = Mat(skin.c, 0.5f);

            GameObject left = new GameObject("half L");
            left.transform.SetParent(root.transform, false);
            left.transform.localPosition = new Vector3(-0.14f, 0, 0);
            GameObject right = new GameObject("half R");
            right.transform.SetParent(root.transform, false);
            right.transform.localPosition = new Vector3(0.14f, 0, 0);

            Half(left.transform, metal, grip, accent, -1f);
            Half(right.transform, metal, grip, accent, 1f);

            SandButterfly anim = root.AddComponent<SandButterfly>();
            anim.left = left.transform;
            anim.right = right.transform;
            return root;
        }

        private static void Half(Transform t, Material metal, Material grip, Material accent, float dir)
        {
            Box(t, "handle", new Vector3(dir * 0.18f, 0, 0), new Vector3(0.34f, 0.07f, 0.08f), grip);
            Box(t, "inlay", new Vector3(dir * 0.12f, 0.04f, 0), new Vector3(0.06f, 0.02f, 0.085f), accent);
            GameObject blade = Box(t, "blade", new Vector3(dir * 0.58f, 0, 0), new Vector3(0.5f, 0.025f, 0.09f), metal);
            blade.transform.localRotation = Quaternion.Euler(0f, 0f, dir * 2f);
            Cyl(t, "pivot", new Vector3(0, 0, 0), new Vector3(0.05f, 0.05f, 0.05f), metal);
        }

        /** Стоящий персонаж в меню. */
        public static GameObject Character(SkinDef skin, Transform parent)
        {
            GameObject root = new GameObject("Персонаж");
            root.transform.SetParent(parent, false);
            Material cloth = Mat(skin.a, 0.05f);
            Material vest = Mat(skin.b, 0.15f);
            Material accent = Mat(skin.c, 0.35f);
            Material boot = Mat(WeaponCatalog.Hex("24272B"), 0.2f);

            GameObject torso = new GameObject("torso");
            torso.transform.SetParent(root.transform, false);
            torso.transform.localPosition = new Vector3(0, 1.02f, 0);
            Box(torso.transform, "body", Vector3.zero, new Vector3(0.66f, 0.82f, 0.36f), cloth);
            Box(torso.transform, "plate", new Vector3(0, 0.04f, 0.2f), new Vector3(0.6f, 0.6f, 0.1f), vest);
            Box(torso.transform, "belt", new Vector3(0, -0.37f, 0), new Vector3(0.7f, 0.1f, 0.4f), accent);
            Box(torso.transform, "arm L", new Vector3(-0.43f, 0, 0), new Vector3(0.16f, 0.75f, 0.18f), cloth);
            Box(torso.transform, "arm R", new Vector3(0.43f, 0, 0), new Vector3(0.16f, 0.75f, 0.18f), cloth);

            GameObject head = new GameObject("head");
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0, 1.62f, 0);
            GameObject skull = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skull.name = "head mesh";
            skull.transform.SetParent(head.transform, false);
            skull.transform.localScale = new Vector3(0.34f, 0.38f, 0.34f);
            skull.GetComponent<Renderer>().sharedMaterial = cloth;
            Box(head.transform, "visor", new Vector3(0, 0.02f, 0.17f), new Vector3(0.3f, 0.1f, 0.07f), accent);
            Box(head.transform, "helmet", new Vector3(0, 0.17f, 0), new Vector3(0.38f, 0.12f, 0.38f), vest);

            Box(root.transform, "leg L", new Vector3(-0.18f, 0.42f, 0), new Vector3(0.22f, 0.85f, 0.24f), cloth);
            Box(root.transform, "leg R", new Vector3(0.18f, 0.42f, 0), new Vector3(0.22f, 0.85f, 0.24f), cloth);
            Box(root.transform, "boot L", new Vector3(-0.18f, 0.06f, 0.06f), new Vector3(0.24f, 0.14f, 0.36f), boot);
            Box(root.transform, "boot R", new Vector3(0.18f, 0.06f, 0.06f), new Vector3(0.24f, 0.14f, 0.36f), boot);

            SandCharacterAnim anim = root.AddComponent<SandCharacterAnim>();
            anim.torso = torso.transform;
            anim.head = head.transform;
            return root;
        }

        public static GameObject Bot(int index, Color team, Transform parent)
        {
            GameObject root = new GameObject("Бот " + (index + 1));
            root.transform.SetParent(parent, false);
            Material body = Mat(team, 0.05f);
            Material dark = Mat(team * 0.55f, 0.2f);
            Material visor = Mat(WeaponCatalog.Hex("E2B866"), 0.4f);
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.SetParent(root.transform, false);
            capsule.transform.localPosition = new Vector3(0, 0.9f, 0);
            capsule.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);
            capsule.GetComponent<Renderer>().sharedMaterial = body;
            Box(root.transform, "vest", new Vector3(0, 1.05f, 0.18f), new Vector3(0.52f, 0.5f, 0.14f), dark);
            Box(root.transform, "visor", new Vector3(0, 1.62f, 0.16f), new Vector3(0.3f, 0.08f, 0.08f), visor);
            Box(root.transform, "gun", new Vector3(0.34f, 1.05f, 0.3f), new Vector3(0.1f, 0.1f, 0.6f), dark);
            return root;
        }
    }
}
