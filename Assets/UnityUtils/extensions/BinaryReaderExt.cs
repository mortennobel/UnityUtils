using System.IO;
using System;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

public static class BinaryReaderExt {
	public static int ReadInt32BE(this BinaryReader b)
	{
		var a32 = b.ReadBytes(4);
		Array.Reverse(a32);
		return BitConverter.ToInt32(a32,0);
	}

	public static Int16 ReadInt16BE(this BinaryReader b)
	{
		var a16 = b.ReadBytes(2);
		Array.Reverse(a16);
		return BitConverter.ToInt16(a16, 0);
	}
	public static Int64 ReadInt64BE(this BinaryReader b)
	{
		var a64 = b.ReadBytes(8);
		Array.Reverse(a64);
		return BitConverter.ToInt64(a64, 0);
	}
	public static UInt32 ReadUInt32BE(this BinaryReader b)
	{
		var a32 = b.ReadBytes(4);
		Array.Reverse(a32);
		return BitConverter.ToUInt32(a32, 0);
	}

	public static float ReadFloatBE(this BinaryReader b)
	{
		var a32 = b.ReadBytes(4);
		Array.Reverse(a32);
		return BitConverter.ToSingle(a32, 0);
	}
	public static double ReadDoubleBE(this BinaryReader b)
	{
		var a32 = b.ReadBytes(8);
		Array.Reverse(a32);
		return BitConverter.ToDouble(a32, 0);
	}

}
