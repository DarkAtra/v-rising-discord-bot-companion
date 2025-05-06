using System.Text.Json.Serialization;

namespace v_rising_discord_bot_companion.character;

[JsonConverter(typeof(VBloodConverter))]
public enum VBlood {

    [VBloodName("Alpha Wolf")]
    FOREST_WOLF = -1905691330,

    [VBloodName("Errol the Stonebreaker")]
    BANDIT_STONEBREAKER = -2025101517,

    [VBloodName("Keely the Frost Archer")]
    BANDIT_DEADEYE_FROSTARROW = 1124739990,

    [VBloodName("Rufus the Foreman")]
    BANDIT_FOREMAN = 2122229952,

    [VBloodName("Goreswine the Ravager")]
    UNDEAD_BISHOPOFDEATH = 577478542,

    [VBloodName("Grayson the Armourer")]
    BANDIT_STALKER = 1106149033,

    [VBloodName("Nibbles the Putrid Rat")]
    VERMIN_DIRERAT = -2039908510,

    [VBloodName("Lidia the Chaos Archer")]
    BANDIT_DEADEYE_CHAOSARROW = 763273073,

    [VBloodName("Clive the Firestarter")]
    BANDIT_BOMBER = 1896428751,

    [VBloodName("Kodia the Ferocious Bear")]
    FOREST_BEAR_DIRE = -1391546313,

    [VBloodName("Polora the Feywalker")]
    POLOMA = -484556888,

    [VBloodName("Nicholaus the Fallen")]
    UNDEAD_PRIEST = 153390636,

    [VBloodName("Quincey the Bandit King")]
    BANDIT_TOUROK = -1659822956,

    [VBloodName("Beatrice the Tailor")]
    VILLAGER_TAILOR = -1942352521,

    [VBloodName("Tristan the Vampire Hunter")]
    VHUNTER_LEADER = -1449631170,

    [VBloodName("Kriig the Undead General")]
    UNDEAD_LEADER = -1365931036,

    [VBloodName("Christina the Sun Priestess")]
    MILITIA_NUN = -99012450,

    [VBloodName("Vincent the Frostbringer")]
    MILITIA_GUARD = -29797003,

    [VBloodName("Bane the Shadowblade")]
    UNDEAD_INFILTRATOR = 613251918,

    [VBloodName("Grethel the Glassblower")]
    MILITIA_GLASSBLOWER = 910988233,

    [VBloodName("Leandra the Shadow Priestess")]
    UNDEAD_BISHOPOFSHADOWS = 939467639,

    [VBloodName("Maja the Dark Savant")]
    MILITIA_SCRIBE = 1945956671,

    [VBloodName("Terah the Geomancer")]
    GEOMANCER_HUMAN = -1065970933,

    [VBloodName("Meredith the Bright Archer")]
    MILITIA_LONGBOWMAN_LIGHTARROW = 850622034,

    [VBloodName("Jade the Vampire Hunter")]
    VHUNTER_JADE = -1968372384,

    [VBloodName("Raziel the Shepherd")]
    MILITIA_BISHOPOFDUNLEY = -680831417,

    [VBloodName("Frostmaw the Mountain Terror")]
    WENDIGO = 24378719,

    [VBloodName("Octavian the Militia Captain")]
    MILITIA_LEADER = 1688478381,

    [VBloodName("Domina the Blade Dancer")]
    GLOOMROT_VOLTAGE = -1101874342,

    [VBloodName("Angram the Purifier")]
    GLOOMROT_PURIFIER = 106480588,

    [VBloodName("Ziva the Engineer")]
    GLOOMROT_IVA = 172235178,

    [VBloodName("Ungora the Spider Queen")]
    SPIDER_QUEEN = -548489519,

    [VBloodName("Ben the Old Wanderer")]
    VILLAGER_CURSEDWANDERER = 109969450,

    [VBloodName("Foulrot the Soultaker")]
    UNDEAD_ZEALOUSCULTIST = -1208888966,

    [VBloodName("Willfred the Werewolf Chief")]
    WEREWOLFCHIEFTAIN = -1505705712,

    [VBloodName("Albert the Duke of Balaton")]
    CURSED_TOADKING = -203043163,

    [VBloodName("Cyril the Cursed Smith")]
    UNDEAD_CURSEDSMITH = 326378955,

    [VBloodName("Sir Magnus the Overseer")]
    CHURCHOFLIGHT_OVERSEER = -26105228,

    [VBloodName("Mairwyn the Elementalist")]
    ARCHMAGE = -2013903325,

    [VBloodName("Baron du Bouchon the Sommelier")]
    CHURCHOFLIGHT_SOMMELIER = 192051202,

    [VBloodName("Morian the Stormwing Matriarch")]
    HARPY_MATRIARCH = 685266977,

    [VBloodName("Terrorclaw the Ogre")]
    WINTER_YETI = -1347412392,

    [VBloodName("Azariel the Sunbringer")]
    CHURCHOFLIGHT_CARDINAL = 114912615,

    [VBloodName("Henry Blackbrew the Doctor")]
    GLOOMROT_THEPROFESSOR = 814083983,

    [VBloodName("Matka the Curse Weaver")]
    CURSED_WITCH = -910296704,

    [VBloodName("Voltatia the Power Master")]
    GLOOMROT_RAILGUNSERGEANT = 2054432370,

    [VBloodName("Nightmarshal Styx the Sunderer")]
    BATVAMPIRE = 1112948824,

    [VBloodName("Solarus the Immaculate")]
    CHURCHOFLIGHT_PALADIN = -740796338,

    [VBloodName("Adam the Firstborn")]
    GLOOMROT_MONSTER = 1233988687,

    [VBloodName("Gorecrusher the Behemoth")]
    CURSED_MOUNTAINBEAST = -1936575244,

    [VBloodName("Talzur the Winged Horror")]
    MANTICORE = -393555055,

    [VBloodName("Finn")]
    BANDIT_FISHERMAN = -2122682556,

    [VBloodName("Elena")]
    ICE_RANGER = 795262842,

    [VBloodName("Valencia")]
    BLOOD_KNIGHT = 495971434,

    [VBloodName("Cassius")]
    HIGH_LORD = -496360395,

    [VBloodName("Dracula")]
    DRACULA = -327335305,

    [VBloodName("Simon Belmont")]
    VHUNTER = 336560131,

    [VBloodName("Sir Erwin the Gallant Cavalier")]
    MILITIA_FABIAN = 619948378,

    [VBloodName("Gaius The Cursed Champion")]
    UNDEAD_ARENA_CHAMPION = -753453016,

    [VBloodName("Stavros the Carver")]
    BLACKFANG_CARVER = -1669199769,

    [VBloodName("Jakira the Shadow Huntress")]
    BLACKFANG_LIVITH = -1383529374,

    [VBloodName("Lucile the Venom Alchemist")]
    BLACKFANG_LUCIE = 1295855316,

    [VBloodName("Dantos the Forgebinder")]
    BLACKFANG_VALYR = 173259239,

    [VBloodName("Megara the Serpent Queen")]
    BLACKFANG_MORGANA = 591725925
}
