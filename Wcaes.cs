using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCD07Test
{
    [ProtoContract]
    public class wcaes
    {
        [ProtoMember(1)]
        public string ID1 { get; set; }

        [ProtoMember(2)]
        public byte[] ID2 { get; set; }

        [ProtoMember(3)]
        public int ID3 { get; set; }

        [ProtoMember(9)]
        public byte[] ID9 { get; set; }

        [ProtoMember(10)]
        public byte[] ID10 { get; set; }

        [ProtoMember(11)]
        public byte[] ID11 { get; set; }


        [ProtoMember(12)]
        public byte[] ID12 { get; set; }

        [ProtoMember(18)]
        public byte[] ID18 { get; set; }

        [ProtoMember(50)]
        public int ID50 { get; set; }


    }
}
