﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace Anamnesis.Memory.Marshalers
{
	using System;
	using Anamnesis.Memory.Offsets;

	internal class QuaternionMarshaler : MarshalerBase<Quaternion>
	{
		public QuaternionMarshaler(params IMemoryOffset[] offsets)
			: base(offsets, 16)
		{
		}

		protected override Quaternion Read(ref byte[] data)
		{
			Quaternion value = default;
			value.X = BitConverter.ToSingle(data, 0);
			value.Y = BitConverter.ToSingle(data, 4);
			value.Z = BitConverter.ToSingle(data, 8);
			value.W = BitConverter.ToSingle(data, 12);
			return value;
		}

		protected override void Write(Quaternion value, ref byte[] data)
		{
			Array.Copy(BitConverter.GetBytes(value.X), data, 4);
			Array.Copy(BitConverter.GetBytes(value.Y), 0, data, 4, 4);
			Array.Copy(BitConverter.GetBytes(value.Z), 0, data, 8, 4);
			Array.Copy(BitConverter.GetBytes(value.W), 0, data, 12, 4);
		}
	}
}
