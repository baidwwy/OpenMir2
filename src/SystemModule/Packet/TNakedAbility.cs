﻿using ProtoBuf;
using System.IO;

namespace SystemModule
{
    [ProtoContract]
    public class TNakedAbility : Packets
    {
        [ProtoMember(1)]
        public ushort DC;
        [ProtoMember(2)]
        public ushort MC;
        [ProtoMember(3)]
        public ushort SC;
        [ProtoMember(4)]
        public ushort AC;
        [ProtoMember(5)]
        public ushort MAC;
        [ProtoMember(6)]
        public ushort HP;
        [ProtoMember(7)]
        public ushort MP;
        [ProtoMember(8)]
        public byte Hit;
        [ProtoMember(9)]
        public int Speed;
        [ProtoMember(10)]
        public byte X2;

        protected override void ReadPacket(BinaryReader reader)
        {
            DC = reader.ReadUInt16();
            MC = reader.ReadUInt16();
            SC = reader.ReadUInt16();
            AC = reader.ReadUInt16();
            MAC = reader.ReadUInt16();
            HP = reader.ReadUInt16();
            MP = reader.ReadUInt16();
            Hit = reader.ReadByte();
            Speed = reader.ReadInt32();
            X2 = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(DC);
            writer.Write(MC);
            writer.Write(SC);
            writer.Write(AC);
            writer.Write(MAC);
            writer.Write(HP);
            writer.Write(MP);
            writer.Write(Hit);
            writer.Write(Speed);
            writer.Write(X2);
            writer.Write(0);
        }
    }
}