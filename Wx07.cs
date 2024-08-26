using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCD07Test
{
    public class Wx07
    {
        static wcaes mSae;
        public static bool Init()
        {

            try
            {

                byte[] data = Core.ReadFile(AppDomain.CurrentDomain.BaseDirectory + "/07.dat");

                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中  
                    ms.Write(data, 0, data.Length);
                    //将流的位置归0  
                    ms.Position = 0;
                    //使用工具反序列化对象  
                    mSae = new wcaes();

                    mSae = ProtoBuf.Serializer.Deserialize<wcaes>(ms);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;


        }



        public static byte[] WcswBases(byte[] srcbuffer)
        {

            if (Init())
            {
                int srcbuffersize = srcbuffer.Length;

                int encryptdatalen = 0;

                byte[] encryptdatabuf;

                encryptdatalen = 16 - (srcbuffersize & 0xF) + srcbuffersize;

                encryptdatabuf = new byte[encryptdatalen];
                //先申请一段加密完成以后的大小内存
                Buffer.BlockCopy(srcbuffer, 0, encryptdatabuf, 0, srcbuffer.Length);
                //如果不是0x10的整数倍就后面补齐
                if (16 != (srcbuffersize & 0xF))
                {
                    for (int i = srcbuffersize; i < encryptdatalen; i++)
                    {
                        encryptdatabuf[i] = (byte)(16 - (srcbuffersize & 0xF));

                    }
                }


                return six(encryptdatabuf, mSae.ID2, mSae.ID9, mSae.ID10, mSae.ID11, mSae.ID12, mSae.ID18);


            }
            else
            {


                return null;
            }


        }



        private static byte[] six1(byte[] pb10, byte[] pb9)
        {
            int w11 = 0;

            int w16 = 5;

            for (int n = 0; n < 9; n++)
            {
                w11 = 0;

                for (int m = 0; m < 4; m++)
                {
                    w16 = 5;

                    for (int z = 0; z < 4; z++)
                    {
                        for (int j = 0; j < 0x100; j++)
                        {
                            for (int i = 0; i < 4; i++)
                            {

                                pb10[n * 0x4000 + m * 0x1000 + z * 0x400 + i + j * 4] = (byte)(pb10[n * 0x4000 + m * 0x1000 + z * 0x400 + i + j * 4] ^ pb9[(w16 + j + i) & 0x7f]);

                            }
                        }

                        w16 = w16 + w11;
                    }

                    w11 = w11 + n;

                }

            }

            return pb10;
        }



        private static byte[] six3(byte[] pb12, byte[] pb11, byte[] pb9)
        {
            for (int i = 0x3f; i >= 0; i--)
            {
                pb9[i] = pb11[i];
            }

            int x16 = 0x13;

            int x11 = 0;


            for (int n = 0; n < 9; n++)
            {
                x11 = 0;

                for (int m = 0; m < 4; m++)
                {
                    x16 = 0x13;

                    for (int z = 0; z < 4; z++)
                    {

                        for (int j = 0; j < 0x3; j++)
                        {

                            for (int y = 0; y < 2; y++)
                            {

                                for (int i = 0; i < 0x80; i++)
                                {

                                    pb12[n * 0x3000 + m * 0xc00 + z * 0x300 + j * 0x100 + y * 0x80 + i] = (byte)(pb12[n * 0x3000 + m * 0xc00 + z * 0x300 + j * 0x100 + y * 0x80 + i] ^ pb9[(j * y + x16 + i) & 0x3f]);

                                }

                            }

                        }




                        x16 = x16 + x11;
                    }

                    x11 = x11 + n;
                }

            }


            return pb12;
        }

        private static void savebyte(ref byte[] v6, uint m, int index)
        {



            byte[] data = BitConverter.GetBytes(m);

            for (int i = 0; i < 4; i++)
            {

                v6[i + index] = data[i];

            }

        }
        static long ROR(long s, int c)
        {
            uint n = (uint)s;


            return (uint)((n >> (c % 32)) | (n << (int)((32 - c) % 32)));

        }

        private static uint getuin1(byte[] v6, int m)
        {
            PackOperate operate = new PackOperate();

            operate.SetBytes(v6);

            operate.GetBytes(m, true);

            return operate.GetBytes(4).ReverseBytes().BytesToUInt();

        }
        private static byte[] Openbyte(byte[] data2)
        {
            byte w8 = data2[4];

            for (int i = 4; i < 7; i++)
            {

                data2[i] = data2[i + 1];
            }

            data2[7] = w8;


            uint w9 = getuin1(data2, 8);

            w9 = (uint)ROR(w9, 0x10);


            savebyte(ref data2, w9, 8);


            w8 = data2[0xf];

            for (int i = 0xf; i > 12; i--)
            {

                data2[i] = data2[i - 1];
            }


            data2[0xC] = w8;

            return data2;

        }

        private static byte[] six(byte[] sdata, byte[] pb2, byte[] pb9, byte[] pb10, byte[] pb11, byte[] pb12, byte[] pb18)
        {


            pb10 = six1(pb10, pb9);

            pb12 = six3(pb12, pb11, pb9);

            byte[] result = new byte[sdata.Length];


            byte[] data1 = new byte[16];

            Array.Copy(pb2, data1, 16);

            int len = sdata.Length / 16;

            byte[] data = new byte[16];

            for (int p = 0; p < len; p++)
            {
                Array.Copy(sdata, p * 16, data, 0, 16);

                byte[] data2 = new byte[16];

                for (int i = 0; i < 16; i++)
                {
                    data1[i] = (byte)(data[i] ^ data1[i]);
                }

                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        data2[i + j * 4] = data1[j + i * 4];

                    }
                }



                for (int i = 0; i < 9; i++)
                {


                    data2 = Openbyte(data2);

                    byte[] datat1 = new byte[0x4000];

                    Array.Copy(pb10, i * 0x4000, datat1, 0, 0x4000);


                    byte[] datat2 = new byte[0x3000];

                    Array.Copy(pb12, i * 0x3000, datat2, 0, 0x3000);

                    byte[] temp = sub_1095C63A8(data2, datat1);

                    data2 = sub_1095C6484(temp, datat2);

                }

                data2 = Openbyte(data2);

                data2 = sub_1095C68A8(data2, pb18);

                for (int j = 0; j < 4; j++)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        data1[j + i * 4] = data2[j * 4 + i];
                    }
                }


                Array.Copy(data1, 0, result, p * 16, 16);


            }


            return result;

        }


        private static byte[] sub_1095C63A8(byte[] a2, byte[] a3)
        {

            byte[] a1 = new byte[64];

            for (int z = 0; z < 4; z++)
            {

                for (int j = 0; j < 4; j++)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        //textBox4.Text += (z + j * 4 + i * 16) + "-" + ((z * 4096) + j * 1024 + 4 * a2[z * 4 + j]) + "\t\n";

                        a1[z + j * 4 + i * 16] = a3[i + (z * 4096) + j * 1024 + 4 * a2[z * 4 + j]];

                        // textBox4.Text += (z + j * 4 + i * 16) + "-" + ((z * 4096) + j * 1024 + 4 * a2[z * 4 + j]) +"-"+ a3[(z * 4096) + j * 1024 + 4 * a2[z * 4 + j]].ToString("X")+"\r\n";

                    }

                }

            }


            return a1;
        }


        private static byte[] sub_1095C6484(byte[] a2, byte[] a3)
        {
            byte[] a1 = new byte[0x10];

            int v3 = 0;
            int v4 = 0;
            int v5 = 0;
            int v6 = 0x200;
            int v7 = 0;

            long result = 0;


            do
            {
                int v8 = 0;

                int v14 = v6;

                int v9 = v7;

                do
                {
                    a1[v5 * 4 + v8] = a2[16 * v5 + 4 * v8 + 3];

                    int v12 = v6;

                    int v13 = 2;

                    do
                    {
                        byte[] data = new byte[a3.Length - v12];

                        Array.Copy(a3, v12, data, 0, a3.Length - v12);

                        // textBox1.Text += a2[v13].ToString("X") + "-" + a1[v5 * 4 + v8].ToString("X") + "-" + v12.ToString("X") + "\r\n";

                        a1[v5 * 4 + v8] = sub_1095C6344(a2[v9 + v13], a1[v5 * 4 + v8], data);

                        //textBox1.Text += a1[v5 * 4 + v8].ToString("X") + "\r\n";

                        v13--;

                        v12 -= 0x100;
                    }
                    while (v13 != -1);

                    v8++;

                    v9 += 4;

                    v6 += 768;
                }
                while (v8 != 4);
                ++v5;
                v7 += 16;
                v6 = v14 + 0xC00;
            }
            while (v5 != 4);
            return a1;

        }


        private static byte[] sub_1095C68A8(byte[] a2, byte[] a3)
        {

            byte[] a1 = new byte[0x10];

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    a1[i + j * 4] = a3[j * 0x400 + i * 0x100 + a2[j * 4 + i]];
                }
            }

            return a1;
        }


        private static byte sub_1095C6344(long a1, long a2, byte[] data)
        {


            long v3 = 0;

            long v4 = 0;

            long v5 = 0;



            if (((a1 & 0xF0 | (a2 >> 4)) & 0x80) > 127)
            {

                //MessageBox.Show(((a1 & 0xF0 | (a2 >> 4)) & 0x7F) + "");

                ;
                v3 = data[(a1 & 0xF0 | (a2 >> 4)) & 0x7F] >> 4;

                // MessageBox.Show(data[(a1 & 0xF0 | (a2 >> 4)) & 0x7F].ToString("X"));



            }
            else
            {
                v3 = data[(a1 & 0xF0 | (a2 >> 4))] & 0xF;

                // MessageBox.Show(((a1 & 0xF0 | (a2 >> 4)) & 0x7F) + ""); ;

            }


            v4 = (byte)(a2 & 0xF | 16 * a1);



            if (((a2 & 0xF | 16 * a1) & 0x80) > 127)
            {
                v5 = data[(v4 & 0x7F) + 128] >> 4;


                //MessageBox.Show(v5.ToString("X"));

            }
            else
            {


                //MessageBox.Show(data[v4 + 128].ToString("X") + ""); ;

                v5 = data[v4 + 128] & 0xF;

            }


            return (byte)(v5 & 0xFFFFFF0F | 16 * (v3 & 0xF));
        }







    }
}
