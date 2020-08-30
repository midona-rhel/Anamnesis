﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace Anamnesis.Memory.Marshalers
{
	using System;
	using Anamnesis.Memory.Offsets;

	internal class UShortMarshaler : MarshalerBase<ushort>
	{
		public UShortMarshaler(params IMemoryOffset[] offsets)
			: base(offsets, 2)
		{
		}

		protected override ushort Read(ref byte[] data)
		{
			return BitConverter.ToUInt16(data, 0);
		}

		protected override void Write(ushort value, ref byte[] data)
		{
			// TODO: GetBytes creates a new 4 byte array.
			// consider getting the ushort into data directly... somehow.
			Array.Copy(BitConverter.GetBytes(value), data, 2);
		}
	}
}
