using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SandStrike
{
    public enum ScreenMode { Menu, Weapons, Skins, Settings, Play, Pause }

    public sealed class SandBotAgent : MonoBehaviour
    {
        public SandGame game;
        public float hp = 100f;
        public float accuracy = 0.32f;
        public float nextShot;
        public char[][] map;
        private float strafe;

        private void Update()
        {
            if (game == null || game.mode != ScreenMode.Play) return;
            Vector3 to = game.PlayerPosition - transform.position;
            float dist = to.magnitude;
            if (dist > 0.1f)
            {
                Vector3 dir = to / dist;
                float side = Mathf.Sin(Time.time * 0.8f + transform.GetInstanceID() * 0.01f) * 0.5f;
                Vector3 move = (dir + Vector3.Cross(Vector3.up, dir) * side).normalized;
                Vector3 target = transform.position + move * (dist > 8f ? 1f : 0.35f) * Time.deltaTime * 2.6f;
                if (game.CanStand(target)) transform.position = target;
                transform.rotation = Quaternion.LookRotation(new Vector3(to.x, 0f, to.z));
            }
            if (dist < 22f && Time.time > nextShot && HasLineOfSight())
            {
                nextShot = Time.time + (1.6f - game.Difficulty * 0.35f);
                if (Random.value < accuracy) game.BotHit(this, Random.value < 0.12f ? 32f : 13f);
            }
        }

        private bool HasLineOfSight()
        {
            Vector3 from = transform.position + Vector3.up * 1.35f;
            Vector3 target = game.PlayerPosition + Vector3.up * 1.3f;
            RaycastHit hit;
            if (Physics.Raycast(from, (target - from).normalized, out hit, (target - from).magnitude + 0.5f))
            {
                if (hit.collider == null) return true;
                return hit.collider.GetComponentInParent<SandBotAgent>() != null || hit.collider.GetComponent<CharacterController>() != null;
            }
            return true;
        }

        public void Hurt(float amount)
        {
            hp -= amount;
            if (hp <= 0f) game.BotKilled(this);
        }
    }

    /** Главный контроллер: меню, лобби, матч, боты и HUD. */
    public sealed class SandGame : MonoBehaviour
    {
        public ScreenMode mode = ScreenMode.Menu;
        public Vector3 PlayerPosition { get { return player != null ? player.position : Vector3.zero; } }
        public int Difficulty { get { return SkinCatalog.Diff; } }

        private Camera cam;
        private GameObject lobbyRoot, mapRoot, viewRoot;
        private GameObject character, previewWeapon, previewButterfly, viewWeapon;
        private Transform player;
        private CharacterController controller;
        private char[][] grid;
        private readonly List<SandBotAgent> bots = new List<SandBotAgent>();

        private WeaponDef weapon;
        private int ammo, reserve;
        private float nextShot, reloadEnd, recoil, pitch, yaw, health = 100f, armor = 50f;
        private float roundTime = 115f, matchScore, enemyScore;
        private bool fireHeld, jumpHeld;
        private float fireUntil;
        private string feed = "";
        private float feedTime;
        private Vector2 joyOrigin, joyDelta;
        private int joyId = -1, lookId = -1;
        private Vector2 lastLook;

        private GUIStyle titleStyle, headerStyle, buttonStyle, smallStyle, statStyle, hudStyle;
        private Texture2D white, dark, sandTex, panelTex;
        private float unit = 1f;

        private void Awake()
        {
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
                if (roots[i] != gameObject) roots[i].SetActive(false);

            white = Tex(new Color(1f, 1f, 1f, 1f));
            dark = Tex(new Color(0.06f, 0.07f, 0.08f, 0.94f));
            sandTex = Tex(new Color(0.886f, 0.718f, 0.4f, 1f));
            panelTex = Tex(new Color(0.09f, 0.1f, 0.115f, 0.9f));

            GameObject camObj = new GameObject("SandStrike Camera");
            camObj.transform.SetParent(transform, false);
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.11f, 0.12f);
            cam.fieldOfView = 70f;
            cam.nearClipPlane = 0.02f;
            cam.farClipPlane = 200f;

            GameObject lightObj = new GameObject("Sun");
            lightObj.transform.SetParent(transform, false);
            lightObj.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            Light sun = lightObj.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.25f;
            RenderSettings.ambientLight = new Color(0.55f, 0.53f, 0.5f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.42f, 0.38f, 0.32f);
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 28f;
            RenderSettings.fogEndDistance = 85f;

            weapon = WeaponCatalog.All[SkinCatalog.Weapon];
            ammo = weapon.mag;
            reserve = weapon.reserve;
            BuildLobby();
            EnterMenu();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (mode == ScreenMode.Play) Pause();
                else if (mode == ScreenMode.Pause) Resume();
                else if (mode != ScreenMode.Menu) EnterMenu();
            }
            if (mode == ScreenMode.Play)
            {
                HandleInput();
                UpdateMatch();
            }
            if (recoil > 0f) recoil = Mathf.Max(0f, recoil - Time.deltaTime * 5f);
            if (viewWeapon != null)
            {
                Vector3 p = viewWeapon.transform.localPosition;
                p.z = -0.62f + recoil * 0.12f;
                p.y = -0.34f - recoil * 0.05f + Mathf.Sin(Time.time * 2f) * 0.004f;
                viewWeapon.transform.localPosition = p;
                viewWeapon.transform.localRotation = Quaternion.Euler(-recoil * 12f, Mathf.Sin(Time.time * 1.5f) * 0.6f, 0f);
            }
        }

        private void HandleInput()
        {
            float mx = 0f, mz = 0f;
            if (Input.GetKey(KeyCode.W)) mz += 1f;
            if (Input.GetKey(KeyCode.S)) mz -= 1f;
            if (Input.GetKey(KeyCode.D)) mx += 1f;
            if (Input.GetKey(KeyCode.A)) mx -= 1f;
            if (Input.GetKey(KeyCode.LeftShift)) mz *= 1.25f;

            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.fingerId == joyId)
                    {
                        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) { joyId = -1; joyDelta = Vector2.zero; }
                        else joyDelta = Vector2.ClampMagnitude((t.position - joyOrigin) / 70f, 1f);
                    }
                    else if (t.fingerId == lookId)
                    {
                        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) lookId = -1;
                        else
                        {
                            Vector2 d = t.position - lastLook;
                            lastLook = t.position;
                            ApplyLook(d.x * 0.16f * SkinCatalog.Sens / 100f, -d.y * 0.13f * SkinCatalog.Sens / 100f);
                        }
                    }
                    else if (t.phase == TouchPhase.Began && t.position.x < Screen.width * 0.45f)
                    {
                        joyId = t.fingerId;
                        joyOrigin = t.position;
                    }
                    else if (t.phase == TouchPhase.Began && t.position.x >= Screen.width * 0.45f)
                    {
                        lookId = t.fingerId;
                        lastLook = t.position;
                    }
                }
            }
            if (Cursor.lockState == CursorLockMode.Locked && Input.mousePresent)
                ApplyLook(Input.GetAxis("Mouse X") * 2.2f, -Input.GetAxis("Mouse Y") * 1.8f);

            mx += joyDelta.x;
            mz += joyDelta.y;
            Vector3 wish = (cam.transform.forward * mz + cam.transform.right * mx);
            wish.y = 0f;
            if (wish.sqrMagnitude > 1f) wish.Normalize();
            float speed = 4.6f * weapon.moveSpeed;
            if (controller.isGrounded)
            {
                Vector3 vel = wish * speed;
                if (jumpHeld) { vel.y = 5.4f; jumpHeld = false; }
                else vel.y = -1f;
                controller.Move(vel * Time.deltaTime);
            }
            else
            {
                Vector3 vel = wish * speed * 0.55f + Vector3.up * controller.velocity.y;
                controller.Move(vel * Time.deltaTime);
            }
            controller.Move(Vector3.down * 18f * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.R)) StartReload();
            if (Input.GetKeyDown(KeyCode.Q)) EquipKnife();
            if (Input.GetKeyDown(KeyCode.Space)) jumpHeld = true;
            if (Time.time < fireUntil || Input.GetKey(KeyCode.F) || (Input.mousePresent && Input.GetMouseButton(0))) Shoot();
        }

        private void ApplyLook(float dx, float dy)
        {
            yaw += dx;
            pitch = Mathf.Clamp(pitch + dy, -55f, 55f);
            if (player != null) player.rotation = Quaternion.Euler(0f, yaw, 0f);
            cam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void Shoot()
        {
            if (Time.time < nextShot || reloadEnd > Time.time || ammo <= 0) return;
            nextShot = Time.time + 60f / Mathf.Max(60, weapon.rpm);
            ammo--;
            recoil = 1f;
            int pellets = weapon.cls == WeaponClass.Shotgun ? 8 : 1;
            for (int i = 0; i < pellets; i++)
            {
                Vector3 dir = cam.transform.forward;
                float spread = weapon.spread * (weapon.cls == WeaponClass.Sniper ? 0.12f : 1f) * 0.0025f;
                dir += cam.transform.right * Random.Range(-spread, spread);
                dir += cam.transform.up * Random.Range(-spread, spread);
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, dir, out hit, weapon.range * 2f))
                {
                    SandBotAgent bot = hit.collider.GetComponentInParent<SandBotAgent>();
                    if (bot != null)
                    {
                        bool head = hit.point.y > bot.transform.position.y + 1.35f;
                        bot.Hurt(weapon.damage * (head ? 2.6f : 1f));
                    }
                }
            }
            if (ammo == 0) StartReload();
        }

        private void StartReload()
        {
            if (reloadEnd > Time.time || reserve <= 0 || ammo >= weapon.mag) return;
            reloadEnd = Time.time + 1.55f;
            Invoke(nameof(FinishReload), 1.55f);
        }

        private void FinishReload()
        {
            int need = weapon.mag - ammo;
            int take = Mathf.Min(need, reserve);
            ammo += take;
            reserve -= take;
        }

        private void EquipKnife()
        {
            SelectWeapon(29);
            SandButterfly flip = viewWeapon != null ? viewWeapon.GetComponentInChildren<SandButterfly>(true) : null;
            if (flip != null) flip.Kick();
        }

        public void BotHit(SandBotAgent bot, float damage)
        {
            armor -= damage * 0.45f;
            health -= damage * (armor < 0f ? 0.8f : 0.55f);
            if (armor < 0f) armor = 0f;
            if (health <= 0f) { health = 100f; armor = 50f; RespawnPlayer(); Feed("ВЫ УБИТЫ"); }
        }

        public void BotKilled(SandBotAgent bot)
        {
            matchScore++;
            SkinCatalog.Kills = SkinCatalog.Kills + 1;
            Feed("УБИЙСТВО  ·  " + weapon.name);
            Vector3 p = bot.transform.position;
            bot.transform.position = FarSpawn();
            bot.hp = 100f;
        }

        private void Feed(string text) { feed = text; feedTime = Time.time + 2.2f; }

        private void UpdateMatch()
        {
            roundTime -= Time.deltaTime;
            if (roundTime <= 0f)
            {
                roundTime = 115f;
                if (matchScore >= enemyScore) SkinCatalog.Wins = SkinCatalog.Wins + 1;
                matchScore = 0;
                enemyScore = 0;
                Feed("РАУНД ЗАВЕРШЁН");
            }
        }

        public bool CanStand(Vector3 p)
        {
            Vector2 c = SandMap.WorldToCell(p);
            int x = Mathf.RoundToInt(c.x), y = Mathf.RoundToInt(c.y);
            if (x < 1 || y < 1 || x >= SandMap.Size - 1 || y >= SandMap.Size - 1) return false;
            return SandMap.Walkable(grid[y][x]);
        }

        private Vector3 FarSpawn()
        {
            List<Vector3> spawns = SandMap.Spawns(grid, 'T');
            Vector3 best = spawns[0];
            float bestDist = -1f;
            for (int i = 0; i < spawns.Count; i++)
            {
                float d = Vector3.Distance(spawns[i], PlayerPosition);
                if (d > bestDist) { bestDist = d; best = spawns[i]; }
            }
            return best;
        }

        private void RespawnPlayer()
        {
            List<Vector3> spawns = SandMap.Spawns(grid, 'P');
            if (spawns.Count > 0 && controller != null)
            {
                controller.enabled = false;
                player.position = spawns[Random.Range(0, spawns.Count)] + Vector3.up * 0.1f;
                controller.enabled = true;
            }
        }

        // ---------- сцены ----------

        private void BuildLobby()
        {
            if (lobbyRoot != null) Destroy(lobbyRoot);
            lobbyRoot = new GameObject("Lobby");
            lobbyRoot.transform.SetParent(transform, false);
            character = SandVisuals.Character(SkinCatalog.Characters[SkinCatalog.CharSkin], lobbyRoot.transform);
            character.transform.position = new Vector3(0f, 0f, 0f);
            character.transform.rotation = Quaternion.Euler(0f, 165f, 0f);

            Transform hands = character.transform.Find("torso");
            if (weapon.cls == WeaponClass.Knife)
                previewWeapon = SandVisuals.Butterfly(SkinCatalog.Guns[SkinCatalog.GunSkin], hands != null ? hands : lobbyRoot.transform);
            else
                previewWeapon = SandVisuals.Weapon(weapon, SkinCatalog.Guns[SkinCatalog.GunSkin], hands != null ? hands : lobbyRoot.transform);
            previewWeapon.transform.localPosition = new Vector3(0f, -0.1f, 0.52f);
            previewWeapon.transform.localRotation = Quaternion.Euler(0f, -90f, 12f);

            previewButterfly = SandVisuals.Butterfly(SkinCatalog.Guns[SkinCatalog.GunSkin], lobbyRoot.transform);
            previewButterfly.transform.position = new Vector3(-1.5f, 1.1f, 0.2f);
            previewButterfly.transform.localScale = Vector3.one * 1.2f;
            previewButterfly.transform.rotation = Quaternion.Euler(0f, -25f, 12f);
        }

        private void EnterMenu()
        {
            mode = ScreenMode.Menu;
            if (mapRoot != null) mapRoot.SetActive(false);
            if (lobbyRoot != null) lobbyRoot.SetActive(true);
            if (player != null) player.gameObject.SetActive(false);
            if (mapRoot != null) { Destroy(mapRoot); mapRoot = null; }
            cam.transform.position = new Vector3(0f, 1.15f, -3.4f);
            cam.transform.rotation = Quaternion.Euler(4f, 0f, 0f);
            cam.fieldOfView = 52f;
            Screen.orientation = ScreenOrientation.Portrait;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void StartMatch()
        {
            if (mapRoot == null) BuildMap();
            else mapRoot.SetActive(true);
            lobbyRoot.SetActive(false);
            mode = ScreenMode.Play;
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;

            if (player == null)
            {
                GameObject p = new GameObject("Player");
                p.transform.SetParent(transform, false);
                controller = p.AddComponent<CharacterController>();
                controller.height = 1.75f;
                controller.radius = 0.32f;
                controller.center = new Vector3(0f, 0.88f, 0f);
                player = p.transform;
                viewRoot = new GameObject("Viewmodel");
                viewRoot.transform.SetParent(cam.transform, false);
            }
            player.gameObject.SetActive(true);
            List<Vector3> spawns = SandMap.Spawns(grid, 'P');
            player.position = spawns.Count > 0 ? spawns[0] + Vector3.up * 0.1f : Vector3.up * 0.1f;
            yaw = 0f;
            pitch = 0f;
            player.rotation = Quaternion.identity;
            cam.transform.localRotation = Quaternion.identity;
            health = 100f;
            armor = 50f;
            roundTime = 115f;
            matchScore = 0;
            enemyScore = 0;
            ammo = weapon.mag;
            reserve = weapon.reserve;
            SpawnBots();
            BuildViewmodel();
            cam.fieldOfView = 74f;
            cam.backgroundColor = new Color(0.42f, 0.38f, 0.32f);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void BuildMap()
        {
            grid = SandMap.Build(SkinCatalog.Map);
            mapRoot = SandMap.BuildScene(SkinCatalog.Map, grid, transform);
            Physics.gravity = new Vector3(0f, -18f, 0f);
        }

        private void SpawnBots()
        {
            for (int i = bots.Count - 1; i >= 0; i--) if (bots[i] != null) Destroy(bots[i].gameObject);
            bots.Clear();
            List<Vector3> spawns = SandMap.Spawns(grid, 'T');
            int count = Mathf.Min(SkinCatalog.Bots, spawns.Count);
            for (int i = 0; i < count; i++)
            {
                Color team = i % 2 == 0 ? new Color(0.72f, 0.51f, 0.3f) : new Color(0.3f, 0.45f, 0.62f);
                GameObject b = SandVisuals.Bot(i, team, transform);
                b.transform.position = spawns[i] + Vector3.up * 0.05f;
                SandBotAgent agent = b.AddComponent<SandBotAgent>();
                agent.game = this;
                agent.map = grid;
                agent.accuracy = 0.18f + SkinCatalog.Diff * 0.09f;
                bots.Add(agent);
            }
        }

        private void BuildViewmodel()
        {
            if (viewWeapon != null) Destroy(viewWeapon);
            viewRoot.gameObject.SetActive(true);
            if (weapon.cls == WeaponClass.Knife)
                viewWeapon = SandVisuals.Butterfly(SkinCatalog.Guns[SkinCatalog.GunSkin], viewRoot);
            else
                viewWeapon = SandVisuals.Weapon(weapon, SkinCatalog.Guns[SkinCatalog.GunSkin], viewRoot);
            viewWeapon.transform.localPosition = new Vector3(0f, -0.34f, -0.62f);
            viewWeapon.transform.localRotation = Quaternion.identity;
        }

        public void SelectWeapon(int index)
        {
            weapon = WeaponCatalog.All[Mathf.Clamp(index, 0, WeaponCatalog.All.Length - 1)];
            SkinCatalog.Weapon = index;
            SkinCatalog.Save();
            ammo = weapon.mag;
            reserve = weapon.reserve;
            BuildLobby();
        }

        private void Pause()
        {
            mode = ScreenMode.Pause;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Resume()
        {
            mode = ScreenMode.Play;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // ---------- GUI ----------

        private void EnsureStyles()
        {
            if (titleStyle != null && Mathf.Abs(unit - CurrentUnit()) < 0.01f) return;
            unit = CurrentUnit();
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = Mathf.RoundToInt(62 * unit);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(0.886f, 0.718f, 0.4f);
            headerStyle = new GUIStyle(titleStyle);
            headerStyle.fontSize = Mathf.RoundToInt(38 * unit);
            headerStyle.normal.textColor = Color.white;
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(28 * unit);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = new Color(0.886f, 0.718f, 0.4f);
            buttonStyle.normal.background = panelTex;
            buttonStyle.hover.background = dark;
            buttonStyle.active.background = sandTex;
            smallStyle = new GUIStyle(buttonStyle);
            smallStyle.fontSize = Mathf.RoundToInt(18 * unit);
            statStyle = new GUIStyle(titleStyle);
            statStyle.fontSize = Mathf.RoundToInt(20 * unit);
            statStyle.alignment = TextAnchor.MiddleLeft;
            statStyle.normal.textColor = new Color(0.85f, 0.86f, 0.84f);
            hudStyle = new GUIStyle(titleStyle);
            hudStyle.fontSize = Mathf.RoundToInt(26 * unit);
            hudStyle.alignment = TextAnchor.MiddleLeft;
            hudStyle.normal.textColor = Color.white;
        }

        private float CurrentUnit()
        {
            if (mode == ScreenMode.Play || mode == ScreenMode.Pause)
                return Mathf.Max(0.55f, Screen.height / 720f);
            return Mathf.Max(0.55f, Mathf.Min(Screen.width / 720f, Screen.height / 1280f));
        }

        private Rect R(float x, float y, float w, float h)
        {
            return new Rect(x * unit, y * unit, w * unit, h * unit);
        }

        private void Bar(Rect r, Texture2D tex) { GUI.DrawTexture(r, tex); }

        private bool Btn(float x, float y, float w, float h, string text, GUIStyle style)
        {
            return GUI.Button(R(x, y, w, h), text, style);
        }

        private void OnGUI()
        {
            EnsureStyles();
            if (mode == ScreenMode.Play) { DrawHud(); return; }
            if (mode == ScreenMode.Pause) { DrawPause(); return; }

            if (mode == ScreenMode.Menu) DrawHome();
            else Bar(new Rect(0, 0, Screen.width, Screen.height), dark);
            else if (mode == ScreenMode.Weapons) DrawWeapons();
            else if (mode == ScreenMode.Skins) DrawSkins();
            else DrawSettings();
        }

        private void DrawHome()
        {
            GUI.Label(R(0, 56, 720, 90), "SANDFALL", titleStyle);
            GUI.Label(R(0, 132, 720, 44), "ПЕСЧАНЫЙ ШТОРМ  ·  OFFLINE FPS", statStyle);
            GUI.Label(R(0, 178, 720, 40), "УБИЙСТВА " + SkinCatalog.Kills + "     ПОБЕДЫ " + SkinCatalog.Wins, statStyle);

            if (Btn(80, 880, 560, 96, "ИГРАТЬ", buttonStyle)) StartMatch();
            if (Btn(80, 996, 270, 82, "ОРУЖИЕ " + weapon.name, smallStyle)) mode = ScreenMode.Weapons;
            if (Btn(370, 996, 270, 82, "СКИНЫ", smallStyle)) mode = ScreenMode.Skins;
            if (Btn(80, 1098, 270, 82, "НАСТРОЙКИ", smallStyle)) mode = ScreenMode.Settings;
            if (Btn(370, 1098, 270, 82, "ВЫХОД", smallStyle)) Application.Quit();

            GUI.Label(R(0, 1220, 720, 36), "v1.0  ·  30 ОРУЖИЙ  ·  КАРТА SANDSTONE", statStyle);
        }

        private void DrawWeapons()
        {
            GUI.Label(R(0, 40, 720, 70), "ОРУЖИЕ  ·  30", headerStyle);
            for (int i = 0; i < WeaponCatalog.All.Length; i++)
            {
                int col = i % 5;
                int row = i / 5;
                float x = 26 + col * 136;
                float y = 150 + row * 104;
                GUIStyle st = new GUIStyle(smallStyle);
                st.fontSize = Mathf.RoundToInt(16 * unit);
                if (i == SkinCatalog.Weapon) st.normal.background = sandTex;
                if (Btn(x, y, 128, 90, WeaponCatalog.All[i].name, st)) SelectWeapon(i);
            }
            WeaponDef d = weapon;
            Bar(R(26, 820, 668, 300), panelTex);
            GUI.Label(R(50, 840, 620, 50), d.name + "  /  " + d.proto, headerStyle);
            GUI.Label(R(50, 900, 620, 40), d.ClassName + "     УРОН " + d.damage + "     МАГ. " + d.mag, statStyle);
            GUI.Label(R(50, 946, 620, 40), "СКОРОСТРЕЛЬНОСТЬ " + d.rpm + "/мин     ДАЛЬНОСТЬ " + d.range + " м", statStyle);
            GUI.Label(R(50, 992, 620, 40), "РАЗБРОС " + d.spread + "     ОТДАЧА " + d.recoil, statStyle);
            if (Btn(80, 1060, 560, 80, "В МЕНЮ", buttonStyle)) EnterMenu();
        }

        private void DrawSkins()
        {
            GUI.Label(R(0, 40, 720, 70), "СКИНЫ", headerStyle);
            GUI.Label(R(30, 130, 330, 40), "ПЕРСОНАЖ", statStyle);
            GUI.Label(R(390, 130, 330, 40), "ОРУЖИЕ", statStyle);
            for (int i = 0; i < SkinCatalog.Characters.Length; i++)
            {
                GUIStyle st = new GUIStyle(smallStyle);
                st.fontSize = Mathf.RoundToInt(16 * unit);
                st.normal.textColor = SkinCatalog.Characters[i].RarityColor;
                if (i == SkinCatalog.CharSkin) st.normal.background = panelTex;
                if (Btn(24, 182 + i * 68, 320, 58, SkinCatalog.Characters[i].name, st))
                { SkinCatalog.CharSkin = i; SkinCatalog.Save(); BuildLobby(); }
            }
            for (int i = 0; i < SkinCatalog.Guns.Length; i++)
            {
                GUIStyle st = new GUIStyle(smallStyle);
                st.fontSize = Mathf.RoundToInt(16 * unit);
                st.normal.textColor = SkinCatalog.Guns[i].RarityColor;
                if (i == SkinCatalog.GunSkin) st.normal.background = panelTex;
                if (Btn(376, 182 + i * 68, 320, 58, SkinCatalog.Guns[i].name, st))
                { SkinCatalog.GunSkin = i; SkinCatalog.Save(); BuildLobby(); }
            }
            if (Btn(80, 1130, 560, 80, "В МЕНЮ", buttonStyle)) EnterMenu();
        }

        private void DrawSettings()
        {
            GUI.Label(R(0, 40, 720, 70), "НАСТРОЙКИ", headerStyle);
            GUI.Label(R(40, 150, 640, 40), "БОТЫ: " + SkinCatalog.Bots, statStyle);
            if (Btn(40, 200, 200, 70, "−", buttonStyle)) SkinCatalog.Bots--;
            if (Btn(480, 200, 200, 70, "+", buttonStyle)) SkinCatalog.Bots++;
            GUI.Label(R(40, 310, 640, 40), "СЛОЖНОСТЬ: " + (SkinCatalog.Diff == 0 ? "ЛЁГКАЯ" : SkinCatalog.Diff == 1 ? "СРЕДНЯЯ" : "СЛОЖНАЯ"), statStyle);
            if (Btn(40, 360, 640, 70, "СМЕНИТЬ", buttonStyle)) SkinCatalog.Diff = (SkinCatalog.Diff + 1) % 3;
            GUI.Label(R(40, 470, 640, 40), "ЧУВСТВИТЕЛЬНОСТЬ: " + SkinCatalog.Sens + "%", statStyle);
            if (Btn(40, 520, 200, 70, "−", buttonStyle)) SkinCatalog.Sens -= 10;
            if (Btn(480, 520, 200, 70, "+", buttonStyle)) SkinCatalog.Sens += 10;
            GUI.Label(R(40, 630, 640, 40), "КАРТА: " + SandMap.Names[SkinCatalog.Map], statStyle);
            if (Btn(40, 680, 640, 70, "СМЕНИТЬ", buttonStyle)) SkinCatalog.Map = (SkinCatalog.Map + 1) % SandMap.Names.Length;
            if (Btn(40, 790, 640, 70, "ЗВУК: " + (SkinCatalog.Sound ? "ВКЛ" : "ВЫКЛ"), buttonStyle)) SkinCatalog.Sound = !SkinCatalog.Sound;
            if (Btn(80, 1080, 560, 80, "В МЕНЮ", buttonStyle)) EnterMenu();
            SkinCatalog.Save();
        }

        private void DrawHud()
        {
            GUI.Label(R(28, 20, 300, 50), "ЗДОРОВЬЕ " + Mathf.RoundToInt(health), hudStyle);
            GUI.Label(R(28, 60, 300, 50), "БРОНЯ " + Mathf.RoundToInt(armor), hudStyle);
            GUI.Label(R(980, 20, 270, 50), Mathf.RoundToInt(roundTime / 60f) + ":" + Mathf.RoundToInt(roundTime % 60f).ToString("00"), hudStyle);
            GUI.Label(R(980, 60, 270, 50), matchScore + " : " + enemyScore, hudStyle);
            GUI.Label(R(28, 620, 500, 50), weapon.name + "   " + ammo + " / " + reserve, hudStyle);
            if (feedTime > Time.time)
                GUI.Label(R(490, 100, 300, 50), feed, hudStyle);

            // перекрестие
            float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;
            float g = 9f * unit;
            Bar(new Rect(cx - g, cy - 1f, g * 2f, 2f), white);
            Bar(new Rect(cx - 1f, cy - g, 2f, g * 2f), white);

            // кнопки управления
            if (GUI.RepeatButton(R(1040, 470, 200, 90), "ОГОНЬ", buttonStyle)) fireUntil = Time.time + 0.08f;
            if (GUI.Button(R(1040, 580, 200, 80), "ПРЫЖОК", smallStyle)) jumpHeld = true;
            if (GUI.Button(R(1040, 670, 200, 80), "ПЕРЕЗ.", smallStyle)) StartReload();
            if (GUI.Button(R(1040, 760, 200, 80), "НОЖ", smallStyle)) EquipKnife();
            if (GUI.Button(R(60, 60, 110, 60), "II", smallStyle)) Pause();

            // джойстик
            if (joyId >= 0)
            {
                Vector2 o = new Vector2(joyOrigin.x, Screen.height - joyOrigin.y) * unit;
                Bar(new Rect(o.x - 42 * unit, o.y - 42 * unit, 84 * unit, 84 * unit), panelTex);
                Vector2 k = joyDelta * 42f * unit;
                Bar(new Rect(o.x + k.x - 14 * unit, o.y - k.y - 14 * unit, 28 * unit, 28 * unit), sandTex);
            }
        }

        private void DrawPause()
        {
            Bar(new Rect(0, 0, Screen.width, Screen.height), dark);
            GUI.Label(R(0, 130, 1280, 90), "ПАУЗА", titleStyle);
            if (Btn(440, 270, 400, 90, "ПРОДОЛЖИТЬ", buttonStyle)) Resume();
            if (Btn(440, 380, 400, 90, "ОРУЖИЕ", buttonStyle))
            {
                Screen.orientation = ScreenOrientation.Portrait;
                EnterMenu();
                mode = ScreenMode.Weapons;
            }
            if (Btn(440, 490, 400, 90, "СКИНЫ", buttonStyle))
            {
                Screen.orientation = ScreenOrientation.Portrait;
                EnterMenu();
                mode = ScreenMode.Skins;
            }
            if (Btn(440, 600, 400, 90, "В МЕНЮ", buttonStyle)) EnterMenu();
        }

        private static Texture2D Tex(Color c)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }
    }
}
