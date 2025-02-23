using System;
using System.IO;

namespace AssetStudio
{
    public class ObjectReader : EndianBinaryReader
    {
        public SerializedFile assetsFile;
        public Game Game;
        public long m_PathID;
        public long byteStart;
        public uint byteSize;
        public ClassIDType type;
        public SerializedType serializedType;
        public BuildTarget platform;
        public SerializedFileFormatVersion m_Version;

        public int[] version => assetsFile.version;
        public BuildType buildType => assetsFile.buildType;

        public ObjectReader(EndianBinaryReader reader, SerializedFile assetsFile, ObjectInfo objectInfo, Game game) : base(reader.BaseStream, reader.Endian)
        {
            this.assetsFile = assetsFile;
            Game = game;
            m_PathID = objectInfo.m_PathID;
            byteStart = objectInfo.byteStart;
            byteSize = objectInfo.byteSize;
            type = Enum.IsDefined(typeof(ClassIDType), objectInfo.classID)
               ? (ClassIDType)objectInfo.classID
                : ClassIDType.UnknownType;
            serializedType = objectInfo.serializedType;
            platform = assetsFile.m_TargetPlatform;
            m_Version = assetsFile.header.m_Version;

            Logger.Verbose($"从{type}初始化读取器,object with {m_PathID} in file {assetsFile.fileName} !!");
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            var pos = Position - byteStart;
            if (pos + count > byteSize)
            {
                throw new EndOfStreamException("无法读取超出流的末尾.");
            }
            return base.Read(buffer, index, count);
        }

        public void Reset()
        {
            Logger.Verbose($"将读取器重置为对象偏移位置0x{byteStart:X8}...");
            Position = byteStart;
        }

        public Vector3 ReadVector3()
        {
            return version[0] > 5 || (version[0] == 5 && version[1] >= 4)
               ? new Vector3(ReadSingle(), ReadSingle(), ReadSingle())
                : new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public XForm ReadXForm()
        {
            var t = ReadVector3();
            var q = ReadQuaternion();
            var s = ReadVector3();
            return new XForm(t, q, s);
        }

        public XForm ReadXForm4()
        {
            var t = ReadVector4();
            var q = ReadQuaternion();
            var s = ReadVector4();
            return new XForm(t, q, s);
        }

        public Vector3[] ReadVector3Array(int length = 0)
        {
            if (length == 0)
            {
                length = ReadInt32();
            }
            return ReadArray(ReadVector3, length);
        }

        public XForm[] ReadXFormArray()
        {
            return ReadArray(ReadXForm, ReadInt32());
        }
    }
}