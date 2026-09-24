using System.Collections.Generic;
using UnityEngine;

// Sistema de localizacion simple: el ingles (tal cual esta hardcodeado en el
// resto del juego, en TowerData/EnemyData/AchievementData y en los textos de
// UI) funciona como "clave". Get(textoEnIngles) devuelve la traduccion al
// español si el idioma actual es Spanish, o el mismo texto si es English o
// si no hay traduccion cargada. Esto evita tener que tocar los .asset de
// torres/enemigos/logros o duplicar campos: alcanza con envolver cada texto
// que ya existe con Get()/Tr().
// DefaultExecutionOrder bien negativo: garantiza que este Awake() corra antes
// que el OnEnable de cualquier LocalizedText de la escena. Sin esto, Unity no
// asegura el orden entre Awake/OnEnable de objetos distintos, y algunos
// textos pueden quedar sin suscribirse a OnLanguageChanged (por eso el
// toggle "funcionaba" pero varios textos no cambiaban).
[DefaultExecutionOrder(-1000)]
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public enum Language { English, Spanish }

    public event System.Action OnLanguageChanged;

    private Language currentLanguage = Language.English;
    public Language CurrentLanguage => currentLanguage;

    private Dictionary<string, string> es;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Por si el objeto quedo como hijo de otro (ej. "GameManagers") en
            // vez de estar en la raiz: DontDestroyOnLoad solo persiste objetos
            // raiz, asi que lo desenganchamos primero para que sobreviva al
            // cambiar de escena (menu -> nivel) y las traducciones sigan
            // funcionando en Level1/2/3.
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            BuildTable();
            currentLanguage = (Language)PlayerPrefs.GetInt("language", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(Language lang)
    {
        if (currentLanguage == lang) return;
        currentLanguage = lang;
        PlayerPrefs.SetInt("language", (int)lang);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }

    public void ToggleLanguage()
    {
        SetLanguage(currentLanguage == Language.English ? Language.Spanish : Language.English);
    }

    // Uso normal: LocalizationManager.Instance.Get("Play")
    public string Get(string englishText)
    {
        if (string.IsNullOrEmpty(englishText) || currentLanguage == Language.English)
            return englishText;

        if (es.TryGetValue(englishText, out string translated))
            return translated;

        return englishText;
    }

    // Atajo estatico para no tener que chequear null de Instance en todos lados.
    // Si el manager todavia no existe en la escena, devuelve el texto en ingles tal cual.
    public static string Tr(string englishText)
    {
        return Instance != null ? Instance.Get(englishText) : englishText;
    }

    void BuildTable()
    {
        es = new Dictionary<string, string>
        {
            // --- Torres (nombres) ---
            { "Booster", "Potenciador" },
            { "Common", "Común" },
            { "Farm", "Granja" },
            { "Machinegun", "Ametralladora" },
            { "Melee", "Cuerpo a Cuerpo" },
            { "Sniper", "Francotirador" },

            // --- Torres (descripciones) ---
            { "Doesn't attack directly, but amplifies the damage, attack speed and range of nearby towers. A force multiplier — a well-placed Booster can turn a good defense into an impenetrable one.",
              "No ataca directamente, pero amplifica el daño, la velocidad de ataque y el alcance de las torres cercanas. Un multiplicador de fuerza: un Potenciador bien ubicado puede convertir una buena defensa en una inexpugnable." },
            { "The standard defensive unit. Balanced stats make it reliable in any situation. A solid choice for your first line of defense and the backbone of most strategies.",
              "La unidad defensiva estándar. Sus estadísticas equilibradas la hacen confiable en cualquier situación. Una opción sólida para tu primera línea de defensa y la columna vertebral de la mayoría de las estrategias." },
            { "Generates passive RAM at the start of each wave. The more you upgrade it, the more resources you have to spend. Be careful though — each additional Farm costs significantly more than the last.",
              "Genera RAM de forma pasiva al comienzo de cada oleada. Cuanto más la mejorás, más recursos tenés para gastar. Eso sí: cada Granja adicional cuesta considerablemente más que la anterior." },
            { "High fire rate, low damage per shot. Excels at shredding fast enemies before they slip past your defenses. Pairs well with a Booster to push its already impressive attack speed even further.",
              "Alta cadencia de disparo, poco daño por impacto. Destaca destrozando enemigos rápidos antes de que se cuelen por tus defensas. Combina muy bien con un Potenciador para llevar su ya impresionante velocidad de ataque aún más lejos." },
            { "A short-range tower that delivers rapid close-range strikes. What it lacks in range it makes up for in attack speed. Place it near tight corners for more hits.",
              "Una torre de corto alcance que asesta golpes rápidos cuerpo a cuerpo. Lo que le falta en alcance lo compensa en velocidad de ataque. Colocala cerca de curvas cerradas para conseguir más impactos." },
            { "Extreme range and devastating damage, but a very slow fire rate. A specialist unit — best used to pick off high-HP targets like Tanks and Bosses from across the board.",
              "Alcance extremo y daño devastador, pero una cadencia de disparo muy lenta. Una unidad especialista, ideal para eliminar objetivos de mucha vida como los Tanques y los Jefes desde el otro extremo del tablero." },

            // --- Enemigos (nombres) ---
            { "Boss", "Jefe" },
            { "Fast", "Rápido" },
            { "Normal", "Normal" },
            { "Tank", "Tanque" },

            // --- Enemigos (descripciones) ---
            { "An elite-tier threat unlike anything seen in standard waves. Its HP scales with each level, making it progressively more dangerous. Every level brings a new, more powerful variant. Take it down before it corrupts everything.",
              "Una amenaza de élite como ninguna otra vista en las oleadas estándar. Su vida escala con cada nivel, haciéndolo progresivamente más peligroso. Cada nivel trae una variante nueva y más poderosa. Eliminalo antes de que lo corrompa todo." },
            { "A lightweight virus optimized for speed. Low HP but moves faster than most towers can track. Best countered with high fire rate towers like the Machinegun.",
              "Un virus liviano optimizado para la velocidad. Poca vida, pero se mueve más rápido de lo que la mayoría de las torres pueden seguir. Se contrarresta mejor con torres de alta cadencia como la Ametralladora." },
            { "A standard malware strain. Balanced speed and durability make it a reliable threat in any wave. Don't underestimate it — in large groups, it can overwhelm even well-placed defenses.",
              "Una cepa de malware estándar. Su velocidad y resistencia equilibradas lo convierten en una amenaza confiable en cualquier oleada. No lo subestimes: en grupos grandes, puede desbordar incluso defensas bien ubicadas." },
            { "A heavily armored trojan designed to absorb punishment. Moves at a crawl but soaks up enormous amounts of damage. Prioritize it with Snipers or upgraded Melee towers before it reaches the Core.",
              "Un troyano fuertemente blindado diseñado para absorber castigo. Se mueve muy lento, pero soporta cantidades enormes de daño. Priorizalo con Francotiradores o torres Cuerpo a Cuerpo mejoradas antes de que llegue al Núcleo." },

            // --- Logros (nombres) ---
            { "Ads Watcher", "Espectador de Anuncios" },
            { "Boss Slayer", "Cazador de Jefes" },
            { "First Blood", "Primera Sangre" },
            { "Frugal Engineer", "Ingeniero Frugal" },
            { "Full Arsenal", "Arsenal Completo" },
            { "Level Completitionist", "Completista de Niveles" },
            { "Max Out", "Al Máximo" },
            { "No Damage Run", "Partida Sin Daño" },
            { "Overclock", "Overclock" },
            { "RAM Hoarder", "Acaparador de RAM" },
            { "Saver", "Ahorrador" },
            { "Speed Runner", "Velocista" },
            { "The Architect", "El Arquitecto" },
            { "Tower Maniac", "Maniático de las Torres" },
            { "Virus Slayer", "Cazador de Virus" },

            // --- Logros (descripciones) ---
            { "Ads Watched", "Anuncios vistos" },
            { "BossSlayer", "Jefes eliminados" },
            { "Slay your first enemy", "Elimina a tu primer enemigo" },
            { "Complete a match using less than 1000 ram", "Completa una partida usando menos de 1000 de RAM" },
            { "Place 1 of every turret in a single match", "Coloca 1 de cada torreta en una sola partida" },
            { "Levels Completed", "Niveles completados" },
            { "Lvl5 turret", "Torreta Nivel 5" },
            { "No damage run", "Completa una partida sin recibir daño" },
            { "Play 50 waves with x2", "Juega 50 oleadas a velocidad x2" },
            { "Get 5000 Ram in a match", "Consigue 5000 de RAM en una partida" },
            { "Sell x towers", "Torres vendidas" },
            { "Beat a level before 2 mins", "Supera un nivel en menos de 2 minutos" },
            { "Place a tower in all spots", "Coloca una torre en todos los espacios" },
            { "Towers Placed", "Torres colocadas" },
            { "EnemyKilled", "Enemigos eliminados" },

            // --- Tutorial ---
            { "Welcome to System Breach TD", "Bienvenido a System Breach TD" },
            { "Your computer is under attack by malware. Deploy defensive towers to stop waves of viruses, trojans and ransomware before they corrupt the Core.",
              "Tu computadora está siendo atacada por malware. Desplegá torres defensivas para detener oleadas de virus, troyanos y ransomware antes de que corrompan el Núcleo." },
            { "Waves & Enemies", "Oleadas y Enemigos" },
            { "Enemies come in waves, following the golden path across the board. Each enemy that reaches the Core deals damage equal to its remaining HP. Survive all waves to win!",
              "Los enemigos llegan en oleadas, siguiendo el camino dorado a través del tablero. Cada enemigo que llega al Núcleo inflige daño igual a su vida restante. ¡Sobrevive todas las oleadas para ganar!" },
            { "The Core", "El Núcleo" },
            { "The Core is what you're defending. It starts with 1000 HP. If it reaches zero, it's game over. Keep an eye on the HP bar at the bottom of the screen.",
              "El Núcleo es lo que estás defendiendo. Empieza con 1000 de vida. Si llega a cero, es game over. Vigilá la barra de vida en la parte inferior de la pantalla." },
            { "Money & Placement", "Dinero y Colocación" },
            { "You start each level with some RAM (money). Earn more by defeating enemies. Tap the Towers button to open the shop, select a tower and tap a green cell to place it. You can also sell towers for half their cost.",
              "Empezás cada nivel con algo de RAM (dinero). Conseguí más derrotando enemigos. Tocá el botón Torres para abrir la tienda, elegí una torre y tocá una celda verde para colocarla. También podés vender torres por la mitad de su costo." },
            { "Tower Types", "Tipos de Torres" },
            { "Attack towers deal damage to enemies. Booster towers increase the stats of nearby attack towers. Farm towers generate passive money at the start of each wave. Combine them wisely!",
              "Las torres de ataque infligen daño a los enemigos. Las torres Potenciador aumentan las estadísticas de las torres de ataque cercanas. Las torres Granja generan dinero pasivo al comienzo de cada oleada. ¡Combinalas con inteligencia!" },
            { "Tower Info", "Información de Torre" },
            { "Tap any placed tower to see its stats and range. From there you can change its targeting priority — First, Closest, Most HP and more — to adapt to each situation.",
              "Tocá cualquier torre colocada para ver sus estadísticas y alcance. Desde ahí podés cambiar su prioridad de objetivo (Primero, Más cercano, Más vida y más) para adaptarte a cada situación." },
            { "Upgrades", "Mejoras" },
            { "Each tower has 5 upgrade levels. Upgrading increases damage, attack speed and range. Invest in upgrades when you have spare RAM — a few upgraded towers beat many weak ones.",
              "Cada torre tiene 5 niveles de mejora. Mejorar aumenta el daño, la velocidad de ataque y el alcance. Invertí en mejoras cuando tengas RAM de sobra: unas pocas torres mejoradas superan a muchas débiles." },
            { "Score", "Puntaje" },
            { "You earn points for every enemy defeated. Bosses give bonus points. Try to clear waves efficiently to maximize your score. Your best score per level is saved automatically.",
              "Ganás puntos por cada enemigo derrotado. Los jefes dan puntos extra. Tratá de limpiar las oleadas de forma eficiente para maximizar tu puntaje. Tu mejor puntaje por nivel se guarda automáticamente." },
            { "Play", "Jugar" },
            { "Press the button below to start the level", "Presioná el botón de abajo para comenzar el nivel" },

            // --- HUD ---
            { "Wave {0}/{1}", "Oleada {0}/{1}" },
            { "WAVE {0} / {1}", "OLEADA {0} / {1}" },

            // --- Notificacion / panel de logros ---
            { "Tier {0} unlocked!", "¡Nivel {0} desbloqueado!" },
            { "Unlocked!", "¡Desbloqueado!" },
            { "Completed!", "¡Completado!" },
            { "Not yet...", "Todavía no..." },
            { "MAX", "MÁX" },

            // --- Panel de info de torre ---
            { "Damage bonus: +{0}%", "Bono de daño: +{0}%" },
            { "Damage bonus: -", "Bono de daño: -" },
            { "Speed bonus: +{0}%", "Bono de velocidad: +{0}%" },
            { "Speed bonus: -", "Bono de velocidad: -" },
            { "Range bonus: +{0}%", "Bono de alcance: +{0}%" },
            { "Range bonus: -", "Bono de alcance: -" },
            { "Level: {0} / {1}", "Nivel: {0} / {1}" },
            { "Upgrade ${0}", "Mejorar ${0}" },
            { "Max Level", "Nivel Máximo" },
            { "Sell ${0}", "Vender ${0}" },
            { "Money/Wave: ${0}", "Dinero/Oleada: ${0}" },
            { "Damage: {0}", "Daño: {0}" },
            { "Damage: {0} (+{1})", "Daño: {0} (+{1})" },
            { "  |  Crit: {0}%", "  |  Crítico: {0}%" },
            { "Speed: {0}", "Velocidad: {0}" },
            { "Speed: {0} (+{1})", "Velocidad: {0} (+{1})" },
            { "Range: {0}", "Alcance: {0}" },
            { "Range: {0} (+{1})", "Alcance: {0} (+{1})" },
            { "  |  Splash: {0}", "  |  Área: {0}" },
            { "  |  Slow: {0}%", "  |  Ralentización: {0}%" },

            // --- FocusMode ---
            { "Closest", "Más Cercano" },
            { "Farthest", "Más Lejano" },
            { "MostHP", "Más Vida" },
            { "LeastHP", "Menos Vida" },
            { "Fastest", "Más Rápido" },
            { "First", "Primero" },

            // --- Selección de nivel ---
            { "Level {0}", "Nivel {0}" },
            { "Level {0}\n<size=55%>Best: {1}</size>", "Nivel {0}\n<size=55%>Mejor: {1}</size>" },

            // --- Galería (torres/enemigos) ---
            { "Level {0} / {1}", "Nivel {0} / {1}" },
            { "Dmg: {0}", "Daño: {0}" },
            { "Fire rate: {0}/s", "Cadencia: {0}/s" },
            { "Bonus dmg: +{0}%", "Bono de daño: +{0}%" },
            { "Bonus firerate: +{0}%", "Bono de cadencia: +{0}%" },
            { "HP: {0}", "Vida: {0}" },
            { "Core Damage: {0}", "Daño al núcleo: {0}" },

            // --- Boss / oleadas ---
            { "BOSS", "JEFE" },
            { "Next wave in {0}s", "Próxima oleada en {0}s" },

            // --- Resultado de nivel (paneles de victoria/derrota) ---
            { "New Record!", "¡Nuevo récord!" },
            { "Record: {0}", "Récord: {0}" },
            { "VICTORY", "VICTORIA" },
            { "DEFEAT", "DERROTA" },
            { "MainMenu", "Menú Principal" },
            { "Restart", "Reiniciar" },
            { "NextLevel", "Siguiente Nivel" },

            // --- Pausa (in-game) ---
            { "Pause", "Pausa" },
            { "Skip", "Saltar" },
            { "Start", "Iniciar" },
            { "TOWERS", "TORRES" },

            // --- Menu principal / HUD estatico ---
            { "Towers", "Torres" },
            // "Info" queda igual en los dos idiomas a propósito
            { "Options", "Opciones" },
            { "Credits", "Créditos" },
            { "Gallery", "Galería" },
            { "Achievements", "Logros" },
            { "Music:", "Música:" },
            { "Sfx:", "Efectos:" },
            { "AutoSkipWaves", "Autosaltar Oleadas" },
            { "EnemyHealthBars", "Vida de Enemigos" },
            { "DamageText", "Texto de Daño" },
            { "Level Selection", "Selección de Nivel" },
            { "Enemies", "Enemigos" },
        };
    }
}
