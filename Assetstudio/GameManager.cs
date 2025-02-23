using System;
using System.Linq;
using System.Collections.Generic;
using static AssetStudio.Crypto;

namespace AssetStudio
{
    public static class GameManager
    {
        private static Dictionary<int, Game> Games = new Dictionary<int, Game>();
        static GameManager()
        {
            int index = 0;
            Games.Add(index++, new(GameType.正常));
            Games.Add(index++, new(GameType.UnityCN));
            Games.Add(index++, new Mhy(GameType.原神, GIMhyShiftRow, GIMhyKey, GIMhyMul, GIExpansionKey, GISBox, GIInitVector, GIInitSeed));
            Games.Add(index++, new Mr0k(GameType.GI_Pack, PackExpansionKey, blockKey: PackBlockKey));
            Games.Add(index++, new Mr0k(GameType.GI_CB1));
            Games.Add(index++, new Blk(GameType.GI_CB2, GI_CBXExpansionKey, initVector: GI_CBXInitVector, initSeed: GI_CBXInitSeed));
            Games.Add(index++, new Blk(GameType.GI_CB3, GI_CBXExpansionKey, initVector: GI_CBXInitVector, initSeed: GI_CBXInitSeed));
            Games.Add(index++, new Mhy(GameType.GI_CB3Pre, GI_CBXMhyShiftRow, GI_CBXMhyKey, GI_CBXMhyMul, GI_CBXExpansionKey, GI_CBXSBox, GI_CBXInitVector, GI_CBXInitSeed));
            Games.Add(index++, new Mr0k(GameType.崩坏三, BH3ExpansionKey, BH3SBox, BH3InitVector, BH3BlockKey));
            Games.Add(index++, new Mr0k(GameType.BH3Pre, PackExpansionKey, blockKey: PackBlockKey));
            Games.Add(index++, new Mr0k(GameType.BH3PrePre, PackExpansionKey, blockKey: PackBlockKey));
            Games.Add(index++, new Mr0k(GameType.SR_CB2, Mr0kExpansionKey, initVector: Mr0kInitVector, blockKey: Mr0kBlockKey));
            Games.Add(index++, new Mr0k(GameType.崩坏星穹铁道, Mr0kExpansionKey, initVector: Mr0kInitVector, blockKey: Mr0kBlockKey));
            Games.Add(index++, new Mr0k(GameType.绝区零, Mr0kExpansionKey, initVector: Mr0kInitVector, blockKey: Mr0kBlockKey));
            Games.Add(index++, new Mr0k(GameType.未定事件簿, Mr0kExpansionKey, initVector: Mr0kInitVector, blockKey: Mr0kBlockKey, postKey: ToTKey));
            Games.Add(index++, new Game(GameType.永劫无间));
            Games.Add(index++, new Game(GameType.偶像梦幻祭2));
            Games.Add(index++, new Game(GameType.航海王热血航线));
            Games.Add(index++, new Game(GameType.FakeHeader));
            Games.Add(index++, new Game(GameType.风之幻想));
            Games.Add(index++, new Game(GameType.胜利女神妮姬));
            Games.Add(index++, new Game(GameType.螺旋圆舞曲2蔷薇战争));
            Games.Add(index++, new Game(GameType.NetEase));
            Games.Add(index++, new Game(GameType.锚点降临));
            Games.Add(index++, new Game(GameType.梦间集天鹅座));
            Games.Add(index++, new Game(GameType.魔法禁书目录幻想收束));
            Games.Add(index++, new Game(GameType.机甲爱丽丝));
            Games.Add(index++, new Game(GameType.世界计划多彩舞台));
            Games.Add(index++, new Game(GameType.jump群星集结));
            Games.Add(index++, new Game(GameType.少女前线));
            Games.Add(index++, new Game(GameType.重返未来1999));
            Games.Add(index++, new Game(GameType.明日方舟));
            Games.Add(index++, new Game(GameType.咒术回战幻影夜行));
            Games.Add(index++, new Game(GameType.MuvLuv维度));
            Games.Add(index++, new Game(GameType.动物派对));
            Games.Add(index++, new Game(GameType.恋与深空));
            Games.Add(index++, new Game(GameType.学园少女突袭者));
            Games.Add(index++, new Game(GameType.来自星辰));
            Games.Add(index++, new Game(GameType.物华弥新));
        }
        public static Game GetGame(GameType gameType) => GetGame((int)gameType);
        public static Game GetGame(int index)
        {
            if (!Games.TryGetValue(index, out var format))
            {
                throw new ArgumentException("无效的格式!!");
            }

            return format;
        }

        public static Game GetGame(string name) => Games.FirstOrDefault(x => x.Value.Name == name).Value;
        public static Game[] GetGames() => Games.Values.ToArray();
        public static string[] GetGameNames() => Games.Values.Select(x => x.Name).ToArray();
        public static string SupportedGames() => $"支持的游戏:\n{string.Join("\n", Games.Values.Select(x => x.Name))}";
    }

    public record Game
    {
        public string Name { get; set; }
        public GameType Type { get; }

        public Game(GameType type)
        {
            Name = type.ToString();
            Type = type;
        }

        public sealed override string ToString() => Name;
    }

    public record Mr0k : Game
    {
        public byte[] ExpansionKey { get; }
        public byte[] SBox { get; }
        public byte[] InitVector { get; }
        public byte[] BlockKey { get; }
        public byte[] PostKey { get; }

        public Mr0k(GameType type, byte[] expansionKey = null, byte[] sBox = null, byte[] initVector = null, byte[] blockKey = null, byte[] postKey = null) : base(type)
        {
            ExpansionKey = expansionKey ?? Array.Empty<byte>();
            SBox = sBox ?? Array.Empty<byte>();
            InitVector = initVector ?? Array.Empty<byte>();
            BlockKey = blockKey ?? Array.Empty<byte>();
            PostKey = postKey ?? Array.Empty<byte>();
        }
    }

    public record Blk : Game
    {
        public byte[] ExpansionKey { get; }
        public byte[] SBox { get; }
        public byte[] InitVector { get; }
        public ulong InitSeed { get; }

        public Blk(GameType type, byte[] expansionKey = null, byte[] sBox = null, byte[] initVector = null, ulong initSeed = 0) : base(type)
        {
            ExpansionKey = expansionKey ?? Array.Empty<byte>();
            SBox = sBox ?? Array.Empty<byte>();
            InitVector = initVector ?? Array.Empty<byte>();
            InitSeed = initSeed;
        }
    }

    public record Mhy : Blk
    {
        public byte[] MhyShiftRow { get; }
        public byte[] MhyKey { get; }
        public byte[] MhyMul { get; }

        public Mhy(GameType type, byte[] mhyShiftRow, byte[] mhyKey, byte[] mhyMul, byte[] expansionKey = null, byte[] sBox = null, byte[] initVector = null, ulong initSeed = 0) : base(type, expansionKey, sBox, initVector, initSeed)
        {
            MhyShiftRow = mhyShiftRow;
            MhyKey = mhyKey;
            MhyMul = mhyMul;
        }
    }

    public enum GameType
    {
        正常,
        UnityCN,
        原神,
        GI_Pack,
        GI_CB1,
        GI_CB2,
        GI_CB3,
        GI_CB3Pre,
        崩坏三,
        BH3Pre,
        BH3PrePre,
        绝区零,
        SR_CB2,
        崩坏星穹铁道,
        未定事件簿,
        永劫无间,
        偶像梦幻祭2,
        航海王热血航线,
        FakeHeader,
        风之幻想,
        胜利女神妮姬,
        螺旋圆舞曲2蔷薇战争,
        NetEase,
        锚点降临,
        梦间集天鹅座,
        魔法禁书目录幻想收束,
        机甲爱丽丝,
        世界计划多彩舞台,
        jump群星集结,
        少女前线,
        重返未来1999,
        明日方舟,
        咒术回战幻影夜行,
        MuvLuv维度,
        动物派对,
        恋与深空,
        学园少女突袭者,
        来自星辰,
        物华弥新,   
    }

    public static class GameTypes
    {
        public static bool IsNormal(this GameType type) => type == GameType.正常;
        public static bool IsUnityCN(this GameType type) => type == GameType.UnityCN;
        public static bool IsGI(this GameType type) => type == GameType.原神;
        public static bool IsGIPack(this GameType type) => type == GameType.GI_Pack;
        public static bool IsGICB1(this GameType type) => type == GameType.GI_CB1;
        public static bool IsGICB2(this GameType type) => type == GameType.GI_CB2;
        public static bool IsGICB3(this GameType type) => type == GameType.GI_CB3;
        public static bool IsGICB3Pre(this GameType type) => type == GameType.GI_CB3Pre;
        public static bool IsBH3(this GameType type) => type == GameType.崩坏三;
        public static bool IsBH3Pre(this GameType type) => type == GameType.BH3Pre;
        public static bool IsBH3PrePre(this GameType type) => type == GameType.BH3PrePre;
        public static bool IsZZZCB1(this GameType type) => type == GameType.绝区零;
        public static bool IsSRCB2(this GameType type) => type == GameType.SR_CB2;
        public static bool IsSR(this GameType type) => type == GameType.崩坏星穹铁道;
        public static bool IsTOT(this GameType type) => type == GameType.未定事件簿;
        public static bool IsNaraka(this GameType type) => type == GameType.永劫无间;
        public static bool IsOPFP(this GameType type) => type == GameType.航海王热血航线;
        public static bool IsNetEase(this GameType type) => type == GameType.NetEase;
        public static bool IsArknightsEndfield(this GameType type) => type == GameType.明日方舟;
        public static bool IsLoveAndDeepspace(this GameType type) => type == GameType.恋与深空;
        public static bool IsExAstris(this GameType type) => type == GameType.来自星辰;
        public static bool IsPerpetualNovelty(this GameType type) => type == GameType.物华弥新;
        public static bool IsGIGroup(this GameType type) => type switch
        {
            GameType.原神 or GameType.GI_Pack or GameType.GI_CB1 or GameType.GI_CB2 or GameType.GI_CB3 or GameType.GI_CB3Pre => true,
            _ => false,
        };

        public static bool IsGISubGroup(this GameType type) => type switch
        {
            GameType.原神 or GameType.GI_CB2 or GameType.GI_CB3 or GameType.GI_CB3Pre => true,
            _ => false,
        };

        public static bool IsBH3Group(this GameType type) => type switch
        {
            GameType.崩坏三 or GameType.BH3Pre => true,
            _ => false,
        };

        public static bool IsSRGroup(this GameType type) => type switch
        {
            GameType.SR_CB2 or GameType.崩坏星穹铁道 => true,
            _ => false,
        };

        public static bool IsBlockFile(this GameType type) => type switch
        {
            GameType.崩坏三 or GameType.BH3Pre or GameType.崩坏星穹铁道 or GameType.GI_Pack or GameType.未定事件簿 or GameType.明日方舟 => true,
            _ => false,
        };

        public static bool IsMhyGroup(this GameType type) => type switch
        {
            GameType.原神 or GameType.GI_Pack or GameType.GI_CB1 or GameType.GI_CB2 or GameType.GI_CB3 or GameType.GI_CB3Pre or GameType.崩坏三 or GameType.BH3Pre or GameType.BH3PrePre or GameType.SR_CB2 or GameType.崩坏星穹铁道 or GameType.绝区零 or GameType.未定事件簿 => true,
            _ => false,
        };
    }
}
